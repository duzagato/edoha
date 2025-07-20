using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Reflection;
using System.Net.Http.Headers;
using Edoha.Domain.Interfaces;
using Edoha.Domain.Models.DTOs;
using Edoha.Infraestructure.Factory;
using Edoha.Shared.Helpers;
using Edoha.Domain.Interfaces.Repositories;

namespace Edoha.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly IDbConnection _connection;
        protected readonly string _tableName;
        protected readonly string _schema;
        protected readonly string _idColumnPascalCase;
        protected readonly string _idColumnSnakeCase;
        protected readonly IEnumerable<PropertyInfo> _properties;

        protected BaseRepository(IDbConnection connection)
        {
            _connection = connection;

            var tableName = RepositoryHelper.GetTableName<T>();
            _schema = RepositoryHelper.GetSchema<T>();
            _tableName = StringHelper.PascalToSnakeCase(tableName);
            _idColumnPascalCase = "Id";
            _idColumnSnakeCase = "id";
            _properties = RepositoryHelper.GetProperties<T>(_idColumnPascalCase);
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
    }
}