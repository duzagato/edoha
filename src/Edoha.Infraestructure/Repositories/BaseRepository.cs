using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Humanizer;
using Edoha.Infrastructure.Data.Context;
using Edoha.Infraestructure.Helpers;
using Edoha.Domain.Interfaces;
using static Dapper.SqlMapper;
using System.Reflection;
using Edoha.Infraestructure.Repositories;
using System.Net.Http.Headers;

namespace Edoha.Infrastructure.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly IDbConnection _connection;
        protected readonly string _tableName;
        protected readonly string _schema;
        protected readonly string _idColumnPascalCase;
        protected readonly string _idColumnSnakeCase;
        protected readonly IEnumerable<PropertyInfo> _properties;

        public BaseRepository(IDbConnection connection)
        {
            string tableName = RepositoryHelper.GetTableName<T>();

            _connection = connection;
            _schema = RepositoryHelper.GetSchema<T>();
            _tableName = tableName.ToLower();
            _idColumnPascalCase = RepositoryHelper.GetIdColumnName(tableName);
            _idColumnSnakeCase = StringHelper.PascalToSnakeCase(_idColumnPascalCase);
            _properties = RepositoryHelper.GetProperties<T>(_idColumnPascalCase);
        }

        public void CheckConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public async Task<T> SelectById(int id)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true; 
            this.CheckConnection();
            var query = $"SELECT * FROM {_schema}.{_tableName} WHERE {_idColumnSnakeCase} = @Id";
            return await _connection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });
        }

        // Seleciona todos os registros
        public async Task<IEnumerable<T>> SelectAll()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            this.CheckConnection();
            var query = $"SELECT * FROM {_schema}.{_tableName}";
            
            return await _connection.QueryAsync<T>(query);
        }

        public async Task Update(T entity)
        {
            this.CheckConnection();
            var setClause = string.Join(", ", _properties.Select(p => $"{StringHelper.PascalToSnakeCase(p.Name)} = @{p.Name}"));
            var query = $"UPDATE {_schema}.{_tableName.ToLower()} SET {setClause} WHERE {_idColumnSnakeCase} = @{_idColumnPascalCase}";
            Console.WriteLine(query);

            try
            {
                await _connection.ExecuteAsync(query, entity);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public async Task Insert(T entity)
        {
            try
            {
                this.CheckConnection();
                var columns = string.Join(", ", _properties.Select(p => StringHelper.PascalToSnakeCase(p.Name)));
                var values = string.Join(", ", _properties.Select(p => $"@{p.Name}"));

                var query = $"INSERT INTO {_schema}.{_tableName} ({columns}) VALUES ({values})";
                await _connection.ExecuteAsync(query, entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task DeleteById(int id)
        {
            this.CheckConnection();
            var query = $"DELETE FROM {_schema}.{_tableName} WHERE {_idColumnSnakeCase} = @Id";
            Console.WriteLine(query);
            await _connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<int> SelectCountById(int id)
        {
            this.CheckConnection();
            var query = $"SELECT COUNT(*) FROM {_schema}.{_tableName} WHERE {_idColumnSnakeCase} = @Id";
            Console.WriteLine(query);
            var count = await _connection.ExecuteScalarAsync<int>(query, new { Id = id });

            return count;
        }

        public async Task<T> ValidateId(int id)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            if (id > 0)
            {
                var entity = await this.SelectById(id);
                if (entity != null)
                {
                    return entity;
                }
                else
                {
                    throw new KeyNotFoundException("Rifa não encontrada");
                }
            }
            else
            {
                throw new ArgumentException("O ID enviado está inválido");
            }
        }
    }
}