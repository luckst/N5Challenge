using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using N5.Challenge.Entities.Dtos;
using N5.Challenge.Entities.Settings;

namespace N5.Challenge.Infrasctructure.KafkaConfig.Producers
{
    public class OperationProducer : IOperationProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topicName;

    public OperationProducer(ProducerConfig config, IOptions<KafkaSettings> options)
    {
        _producer = new ProducerBuilder<string, string>(config).Build();

        _topicName = options.Value.PermissionsTopic;
    }

    public async Task SendOperationAsync(string nameOperation)
    {
        var dto = new OperationDto
        {
            Id = Guid.NewGuid(),
            NameOperation = nameOperation
        };

        var jsonValue = JsonSerializer.Serialize(dto);
        
        var deliveryResult = await _producer.ProduceAsync(
            _topicName,
            new Message<string, string>
            {
                Key = dto.Id.ToString(),
                Value = jsonValue
            });

        Console.WriteLine($"Produced message to {deliveryResult.TopicPartitionOffset}");
    }
}
}