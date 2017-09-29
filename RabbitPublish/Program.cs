using System;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Diagnostics;

using Common;
using Messages;

/// <summary>
/// Publishing messages with raw RabbitMQ.Client
/// </summary>
namespace RabbitPublish
{
    class Program
    {
        static Random rnd = new Random();
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = Env.Var("RABBITMQ_HOST"),
                Port = Env.VarInt("RABBITMQ_PORT"),
                UserName = Env.Var("RABBITMQ_USERNAME"),
                Password = Env.Var("RABBITMQ_PASSWORD")
            };

            
            using (var connection = factory.CreateConnection())
            {
                Console.WriteLine("RabbitMQ Publisher started");

                using (var channel = connection.CreateModel())
                {
                    var result = channel.QueueDeclare(queue: "first-queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                    Console.WriteLine($"Connected to queue: {result.QueueName}");

                    var counter = 1;
                    while (true)
                    {
                        var input = Console.ReadLine();

                        if (input == "quit") break;

                        Console.Write("How many times do you want to send the message? ");

                        var messageCountInput = Console.ReadLine();
                        var msgCount = int.Parse(messageCountInput);

                        var messageProps = channel.CreateBasicProperties();
                        messageProps.Persistent = true;

                        var sw = Stopwatch.StartNew();
                        sw.Start();

                        for (var i = 0; i < msgCount; i++)
                        {

                            var message = new Message
                            {
                                WorkDelay = rnd.Next(1, 1000),
                                Id = counter,
                                Body = input
                            };

                            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                            channel.BasicPublish(exchange: "",
                                                 routingKey: "first-queue",
                                                 basicProperties: messageProps,
                                                 body: body);

                            counter = counter + 1;
                        }

                        sw.Stop();



                        Console.WriteLine($"Sent {msgCount} messages in {sw.ElapsedMilliseconds} ms");
                    }
                }
            }

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
