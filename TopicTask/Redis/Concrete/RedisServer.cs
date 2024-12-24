using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using TopicTask.Redis.Abstruction;

namespace TopicTask.Redis.Concrete
{
    public class RedisServer : IRedisServer
    {

        private readonly ConnectionFactory _connectionFactory;

        public RedisServer()
        {
            _connectionFactory = new ConnectionFactory
            {
                Uri = new Uri("amqps://usnletdq:FKYHW66StW-A21THXjGj_sIzy7Kn14ue@dog.lmq.cloudamqp.com/usnletdq")
            };
        }

        public async Task<bool> SendMessageTopic(string routingKey, string message)
        {

            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            string exchangeName = "main_folder";

            try
            {
                await channel.ExchangeDeclareAsync(
                    exchange: exchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    passive: true
                );
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
            {
                if (ex.ShutdownReason.ReplyCode == 404)
                {
                    await channel.ExchangeDeclareAsync(
                        exchange: exchangeName,
                        type: ExchangeType.Topic,
                        durable: true,
                        autoDelete: false
                    );
                }
                else
                {
                    throw;
                }
            }

            var body = System.Text.Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey + ".#",
                mandatory: false,
                body: body
            );

            Console.WriteLine($"Sent Message -> {message}");

            return true;
        }
    }
 }


