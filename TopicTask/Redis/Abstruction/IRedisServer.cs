namespace TopicTask.Redis.Abstruction
{
    public interface IRedisServer
    {
        Task<bool> SendMessageTopic(string routingKey, string message);
    }
}
