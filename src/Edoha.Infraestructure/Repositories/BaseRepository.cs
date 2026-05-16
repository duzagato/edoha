using Dapper;
using Edoha.Domain.Helpers;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs;
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

        protected BaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.CreateConnection();

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
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            if (id == null || id == Guid.Empty)
                throw new ArgumentException("O ID enviado está inválido");


            var query = $@"
                SELECT * FROM ""{_schema}"".""{_tableName}""
                WHERE ""{_idColumnSnakeCase}"" = @Id";

            var entity = await _connection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });

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
            CheckConnection();

            var props = dto.GetProperties(_idColumnPascalCase);
            var columns = string.Join(", ", props.Select(p => $@"""{StringHelper.PascalToSnakeCase(p.Name)}"""));
            var values = string.Join(", ", props.Select(p => $"@{p.Name}"));

            var query = $@"
                INSERT INTO ""{_schema}"".""{_tableName}"" ({columns})
                VALUES ({values})";

            await _connection.ExecuteAsync(query, dto);
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

            return await _connection.ExecuteScalarAsync<Guid>(query, dto);
        }

        public async Task Update(DTO dto)
        {
            CheckConnection();

            var props = dto.GetProperties(_idColumnPascalCase);
            var setClause = string.Join(", ", props.Select(p =>
                $@"""{StringHelper.PascalToSnakeCase(p.Name)}"" = @{p.Name}"
            ));

            var query = $@"
                UPDATE ""{_schema}"".""{_tableName}""
                SET {setClause}
                WHERE ""{_idColumnSnakeCase}"" = @{_idColumnPascalCase}";

            await _connection.ExecuteAsync(query, dto);
        }

        public async Task DeleteById(Guid id)
        {
            CheckConnection();

            var query = $@"
                DELETE FROM ""{_schema}"".""{_tableName}""
                WHERE ""{_idColumnSnakeCase}"" = @Id";

            await _connection.ExecuteAsync(query, new { Id = id });
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
            if (id == null || id == Guid.Empty)
                throw new ArgumentException("O ID enviado está inválido");

            var entity = await SelectById(id.Value);
            if (entity == null)
                throw new KeyNotFoundException("Entidade não encontrada");
        }

        public async Task IdExists(int? id)
        {
            if (id == null || id == 0)
                throw new ArgumentException("O ID enviado está inválido");

            CheckConnection();

            var query = $@"
                SELECT COUNT(*) FROM ""{_schema}"".""{_tableName}""
                WHERE ""{_idColumnSnakeCase}"" = @Id";

            var count = await _connection.ExecuteScalarAsync<int>(query, new { Id = id });
            
            if (count == 0)
                throw new KeyNotFoundException("Entidade não encontrada");
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