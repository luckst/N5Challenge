namespace N5.Challenge.Infrasctructure.KafkaConfig.Producers
{
    public interface IOperationProducer
    {
        Task SendOperationAsync(string nameOperation);
    }
}