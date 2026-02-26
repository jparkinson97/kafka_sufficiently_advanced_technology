namespace kafka_sufficiently_advanced_technology.Services;

public interface IKafkaService
{
    Task<IEnumerable<string>> GetTopicsAsync(string bootstrapServers);

    Task<string> ConsumeRawMessageAsync(
        string bootstrapServers,
        string topic,
        bool takeFirst,
        int? partition = null,
        long? offset = null);

    Task<string> ConsumeAndDeserializeAsync(
        string bootstrapServers,
        string topic,
        string assemblyName,
        string className,
        bool takeFirst,
        int? partition = null,
        long? offset = null);

    Task<(int MinPartition, int MaxPartition, long MinOffset, long MaxOffset)> GetTopicRangesAsync(
        string bootstrapServers,
        string topic);
}
