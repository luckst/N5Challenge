using Confluent.Kafka;

namespace N5.Challenge.Infrasctructure.KafkaConfig
{
    public interface IKafkaProducer<TKey, TValue>
    {
        Task ProduceAsync(string topic, TKey key, TValue value);
    }
}
