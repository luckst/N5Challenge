using Confluent.Kafka;

namespace N5.Challenge.Infrasctructure.KafkaConfig
{
    public class KafkaProducer<K, V> : IKafkaProducer<K, V>
    {
        IProducer<K, V> kafkaHandle;

        public KafkaProducer(KafkaClientHandle handle)
        {
            kafkaHandle = new DependentProducerBuilder<K, V>(handle.Handle).Build();
        }

        public Task ProduceAsync(string topic, Message<K, V> message)
            => this.kafkaHandle.ProduceAsync(topic, message);

        public void Flush(TimeSpan timeout)
            => this.kafkaHandle.Flush(timeout);
    }
}
