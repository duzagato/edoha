using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using System.Runtime.CompilerServices;

namespace Edoha.Infraestructure.Helpers
{
    public static class RepositoryHelper
    {
        public static string GetSchema<T>() where T : class
        {
            var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
            return tableAttribute?.Schema ?? "EDOHA";
        }

        public static string GetTableName<T>() where T : class
        {
            var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();
            return tableAttribute?.Name ?? typeof(T).Name;
        }

        public static IEnumerable<PropertyInfo> GetProperties<T>(string idColumnName) where T : class
        {
            return typeof(T).GetProperties().Where(p => p.Name != idColumnName);
        }

        public static string GetIdColumnName(string tableName)
        {
            return $"Id{tableName}";
        }
    }
}