using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri(System.Environment.GetEnvironmentVariable("RABBITMQ_URI") ?? "amqp://guest:guest@localhost:5672");


using var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

//durable:true fiziksel olarak kaydedilmesini sağlar
channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    string message = $"log {x}";
    var messageBody = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish("logs-fanout", "", null, messageBody);
    Console.WriteLine($"Mesaj gönderilmiştir:  {message}");

});