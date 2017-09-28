using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

using Messages;
using Common;

namespace RabbitSubscribe
{
    class Program
    {
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine("Started RabbitMQ Subscriber");

            var factory = new ConnectionFactory()
            {
                HostName = Env.Var("RABBITMQ_HOST"),
                Port = Env.VarInt("RABBITMQ_PORT"),
                UserName = Env.Var("RABBITMQ_USERNAME"),
                Password = Env.Var("RABBITMQ_PASSWORD")
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "first-queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var typedMessage = JsonConvert.DeserializeObject<Message>(message);
                    await Console.Out.WriteLineAsync($"Recieved Message with Id = {typedMessage.Id} and Body {typedMessage.Body}");
                };


                channel.BasicConsume(queue: "first-queue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
