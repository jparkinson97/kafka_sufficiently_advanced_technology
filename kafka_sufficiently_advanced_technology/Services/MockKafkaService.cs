namespace kafka_sufficiently_advanced_technology.Services;

public class MockKafkaService : IKafkaService
{
    private static readonly string[] MockTopics =
    [
        "orders",
        "payments",
        "user-events",
        "notifications",
        "inventory-updates",
        "audit-logs",
        "dead-letter-queue",
        "customer-created",
        "shipment-tracking",
    ];

    public async Task<IEnumerable<string>> GetTopicsAsync(string bootstrapServers)
    {
        await Task.Delay(600);
        return MockTopics;
    }

    public async Task<string> ConsumeRawMessageAsync(
        string bootstrapServers,
        string topic,
        bool takeFirst,
        int? partition = null,
        long? offset = null)
    {
        await Task.Delay(800);
        var locationNote = takeFirst
            ? "// first available message"
            : $"// partition={partition ?? 0}, offset={offset ?? 0}";

        return $$"""
            {
              "_meta": {
                "topic": "{{topic}}",
                "partition": {{partition ?? 0}},
                "offset": {{offset ?? 1042}},
                "timestamp": "2026-02-26T10:30:00Z"
              },
              {{locationNote}}
              "orderId": "ORD-12345",
              "customerId": "CUST-789",
              "items": [
                { "productId": "PROD-001", "quantity": 2, "price": 29.99 },
                { "productId": "PROD-042", "quantity": 1, "price": 49.99 }
              ],
              "totalAmount": 109.97,
              "currency": "USD",
              "status": "PENDING",
              "createdAt": "2026-02-26T10:30:00Z"
            }
            """;
    }

    public async Task<string> ConsumeAndDeserializeAsync(
        string bootstrapServers,
        string topic,
        string assemblyName,
        string className,
        bool takeFirst,
        int? partition = null,
        long? offset = null)
    {
        await Task.Delay(900);
        return $$"""
            // Proto-deserialized as {{className}} ({{assemblyName}})
            {
              "_meta": {
                "topic": "{{topic}}",
                "messageType": "{{className}}",
                "partition": {{partition ?? 0}},
                "offset": {{offset ?? 1042}},
                "timestamp": "2026-02-26T10:30:00Z"
              },
              "orderId": "ORD-12345",
              "customerId": "CUST-789",
              "timestamp": "2026-02-26T10:30:00Z",
              "payload": {
                "field1": "value1",
                "field2": 42,
                "nested": {
                  "active": true,
                  "tags": ["kafka", "proto", "maui"]
                }
              }
            }
            """;
    }

    public async Task<(int MinPartition, int MaxPartition, long MinOffset, long MaxOffset)> GetTopicRangesAsync(
        string bootstrapServers,
        string topic)
    {
        await Task.Delay(300);
        return (0, 3, 0, 10_420);
    }
}
