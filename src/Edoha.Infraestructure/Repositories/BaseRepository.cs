using Dapper;
using Edoha.Domain.Helpers;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace Edoha.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IDbConnectionFactory _connectionFactory;
        protected readonly IDbConnection _connection;
        protected readonly string _tableName;
        protected readonly string _schema;
        protected readonly string _idColumnPascalCase;
        protected readonly string _idColumnSnakeCase;
        protected readonly IEnumerable<PropertyInfo> _properties;
        protected readonly ILogger? _logger;

        protected BaseRepository(IDbConnectionFactory connectionFactory, ILogger? logger = null)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.CreateConnection();
            _logger = logger;

            var tableName = GetTableName<T>();
            _schema = GetSchema<T>();
            _tableName = StringHelper.PascalToSnakeCase(tableName);
            _idColumnPascalCase = "Id";
            _idColumnSnakeCase = "id";
            _properties = GetProperties<T>(_idColumnPascalCase);
        }

        protected void CheckConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public async Task<T> SelectById(Guid? id)
        {
            _logger?.LogInformation("Iniciando método SelectById (BaseRepository)");
            _logger?.LogInformation("Parâmetros recebidos - id: {Id}, tabela: {TableName}", id, _tableName);

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            if (id == null || id == Guid.Empty)
            {
                _logger?.LogError("ID inválido recebido: {Id}", id);
                throw new ArgumentException("O ID enviado está inválido");
            }

            var query = $@"
                SELECT * FROM ""{_schema}"".""{_tableName}""
                WHERE ""{_idColumnSnakeCase}"" = @Id";

            _logger?.LogInformation("Executando query para selecionar entidade por id");
            var entity = await _connection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });

            if (entity == null)
            {
                _logger?.LogWarning("Entidade não encontrada para id {Id} na tabela {TableName}", id, _tableName);
            }
            else
            {
                _logger?.LogInformation("Entidade encontrada para id {Id}. Retornando resultado", id);
            }

            return entity ?? throw new KeyNotFoundException("Entidade não encontrada");
        }

        public async Task<IEnumerable<T>> SelectAll()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            var query = $@"SELECT * FROM ""{_schema}"".""{_tableName}""";

            return await _connection.QueryAsync<T>(query);
        }

        public async Task Insert(DTO dto)
        {
            _logger?.LogInformation("Iniciando método Insert (BaseRepository)");
            _logger?.LogInformation("Parâmetros recebidos - dto: {@Dto}, tabela: {TableName}", dto, _tableName);

            CheckConnection();

            var props = dto.GetProperties(_idColumnPascalCase);
            var columns = string.Join(", ", props.Select(p => $@"""{StringHelper.PascalToSnakeCase(p.Name)}"""));
            var values = string.Join(", ", props.Select(p => $"@{p.Name}"));

            var query = $@"
                INSERT INTO ""{_schema}"".""{_tableName}"" ({columns})
                VALUES ({values})";

            _logger?.LogInformation("Executando query de inserção na tabela {TableName}", _tableName);
            await _connection.ExecuteAsync(query, dto);

            _logger?.LogInformation("Entidade inserida com sucesso na tabela {TableName}", _tableName);
        }

        public async Task<Guid> InsertAndReturnId(DTO dto)
        {
            CheckConnection();

            var props = dto.GetProperties(_idColumnPascalCase);
            var columns = string.Join(", ", props.Select(p => $@"""{StringHelper.PascalToSnakeCase(p.Name)}"""));
            var values = string.Join(", ", props.Select(p => $"@{p.Name}"));

            var query = $@"
            INSERT INTO ""{_schema}"".""{_tableName}"" ({columns})
            VALUES ({values})
            RETURNING ""{StringHelper.PascalToSnakeCase(_idColumnPascalCase)}""";

            var id = await _connection.QuerySingleAsync<Guid>(query, dto);

            return id;
        }

        public async Task<Guid> InsertOrGetId(DTO dto)
        {
            _logger?.LogInformation("Iniciando método InsertOrGetId (BaseRepository)");
            _logger?.LogInformation("Parâmetros recebidos - dto: {@Dto}, tabela: {TableName}", dto, _tableName);

            CheckConnection();

            var props = dto.GetProperties(_idColumnPascalCase);
            var columnNames = props.Select(p => $@"""{StringHelper.PascalToSnakeCase(p.Name)}""").ToList();
            var paramNames = props.Select(p => $"@{p.Name}").ToList();

            var columns = string.Join(", ", columnNames);
            var values = string.Join(", ", paramNames);

            var conflictColumns = string.Join(", ", columnNames);
            conflictColumns = "phone";

            var whereConditions = string.Join(" AND ", columnNames.Zip(paramNames, (col, param) => $"{col} = {param}"));

            var query = $@"
        WITH insercao AS (
            INSERT INTO ""{_schema}"".""{_tableName}"" ({columns})
            VALUES ({values})
            ON CONFLICT ({conflictColumns}) DO NOTHING
            RETURNING ""id""
        )
        SELECT ""id"" FROM insercao
        UNION ALL
        SELECT ""id"" FROM ""{_schema}"".""{_tableName}"" 
        WHERE {whereConditions}
        LIMIT 1;";

            _logger?.LogInformation("Executando query de inserção ou recuperação de id na tabela {TableName}", _tableName);
            var id = await _connection.ExecuteScalarAsync<Guid>(query, dto);

            _logger?.LogInformation("InsertOrGetId finalizado. Id retornado: {Id}", id);
            return id;
        }

        public async Task Update(DTO dto)
        {
            _logger?.LogInformation("Iniciando método Update (BaseRepository)");
            _logger?.LogInformation("Parâmetros recebidos - dto: {@Dto}, tabela: {TableName}", dto, _tableName);

            CheckConnection();

            var props = dto.GetProperties(_idColumnPascalCase);
            var setClause = string.Join(", ", props.Select(p =>
                $@"""{StringHelper.PascalToSnakeCase(p.Name)}"" = @{p.Name}"
            ));

            var query = $@"
                UPDATE ""{_schema}"".""{_tableName}""
                SET {setClause}
                WHERE ""{_idColumnSnakeCase}"" = @{_idColumnPascalCase}";

            _logger?.LogInformation("Executando query de atualização na tabela {TableName}", _tableName);
            await _connection.ExecuteAsync(query, dto);

            _logger?.LogInformation("Entidade atualizada com sucesso na tabela {TableName}", _tableName);
        }

        public async Task DeleteById(Guid id)
        {
            _logger?.LogInformation("Iniciando método DeleteById (BaseRepository)");
            _logger?.LogInformation("Parâmetros recebidos - id: {Id}, tabela: {TableName}", id, _tableName);

            CheckConnection();

            var query = $@"
                DELETE FROM ""{_schema}"".""{_tableName}""
                WHERE ""{_idColumnSnakeCase}"" = @Id";

            _logger?.LogInformation("Executando query de deleção na tabela {TableName}", _tableName);
            await _connection.ExecuteAsync(query, new { Id = id });

            _logger?.LogInformation("Entidade com id {Id} deletada com sucesso da tabela {TableName}", id, _tableName);
        }

        public async Task<int> SelectCountById(Guid id)
        {
            CheckConnection();

            var query = $@"
                SELECT COUNT(*) FROM ""{_schema}"".""{_tableName}""
                WHERE ""{_idColumnSnakeCase}"" = @Id";

            return await _connection.ExecuteScalarAsync<int>(query, new { Id = id });
        }

        public async Task IdExists(Guid? id)
        {
            _logger?.LogInformation("Iniciando método IdExists (BaseRepository) com Guid");
            _logger?.LogInformation("Parâmetros recebidos - id: {Id}, tabela: {TableName}", id, _tableName);

            if (id == null || id == Guid.Empty)
            {
                _logger?.LogError("ID inválido recebido: {Id}", id);
                throw new ArgumentException("O ID enviado está inválido");
            }

            _logger?.LogInformation("Verificando existência da entidade com id {Id}", id);
            var entity = await SelectById(id.Value);
            if (entity == null)
            {
                _logger?.LogError("Entidade com id {Id} não encontrada na tabela {TableName}", id, _tableName);
                throw new KeyNotFoundException("Entidade não encontrada");
            }

            _logger?.LogInformation("Entidade com id {Id} existe na tabela {TableName}", id, _tableName);
        }

        public async Task IdExists(int? id)
        {
            _logger?.LogInformation("Iniciando método IdExists (BaseRepository) com int");
            _logger?.LogInformation("Parâmetros recebidos - id: {Id}, tabela: {TableName}", id, _tableName);

            if (id == null || id == 0)
            {
                _logger?.LogError("ID inválido recebido: {Id}", id);
                throw new ArgumentException("O ID enviado está inválido");
            }

            CheckConnection();

            var query = $@"
                SELECT COUNT(*) FROM ""{_schema}"".""{_tableName}""
                WHERE ""{_idColumnSnakeCase}"" = @Id";

            _logger?.LogInformation("Verificando existência da entidade com id {Id}", id);
            var count = await _connection.ExecuteScalarAsync<int>(query, new { Id = id });
            
            if (count == 0)
            {
                _logger?.LogError("Entidade com id {Id} não encontrada na tabela {TableName}", id, _tableName);
                throw new KeyNotFoundException("Entidade não encontrada");
            }

            _logger?.LogInformation("Entidade com id {Id} existe na tabela {TableName}", id, _tableName);
        }

        public async Task<bool> IsUnique(string column, string value)
        {
            CheckConnection();

            var query = $@"
                SELECT COUNT(*) FROM ""{_schema}"".""{_tableName}""
                WHERE ""{column}"" = @Value";

            var count = await _connection.ExecuteScalarAsync<int>(query, new { column = value });

            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected static string GetSchema<T>() where T : class
        {
            var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
            return tableAttribute?.Schema ?? "edoha";
        }

        protected static string GetTableName<T>() where T : class
        {
            var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
            return tableAttribute?.Name ?? typeof(T).Name;
        }

        protected static IEnumerable<PropertyInfo> GetProperties<T>(string idColumnName) where T : class
        {
            return typeof(T).GetProperties().Where(p => p.Name != idColumnName);
        }

        protected static string GetIdColumnName(string tableName)
        {
            return $"Id{tableName}";
        }
    }
}