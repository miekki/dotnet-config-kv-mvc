using Azure;
using Azure.Data.Tables;

public class OfficeSuplyEntity : ITableEntity
{
    public required string Product { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public required string PartitionKey { get; set; }
    public required string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}