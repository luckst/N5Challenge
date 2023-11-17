using Confluent.Kafka;

namespace N5.Challenge.Infrasctructure.KafkaConfig
{
    public interface IKafkaProducer<K, V>
    {
        Task ProduceAsync(string topic, Message<K, V> message);
        void Flush(TimeSpan timeout);
    }
}
