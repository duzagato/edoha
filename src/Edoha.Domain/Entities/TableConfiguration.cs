namespace Edoha.Domain.Entities
{
    public class TableConfiguration
    {
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int OrdinalPosition { get; set; }
        public string? ColumnDefault { get; set; }
        public string IsNullable { get; set; }
        public string DataType { get; set; }
    }
}