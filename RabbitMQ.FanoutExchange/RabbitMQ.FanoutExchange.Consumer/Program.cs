using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://wylxhthv:9gEUm_Hi-PGavdDsheGhvL106sliPAi5@shark.rmq.cloudamqp.com/wylxhthv ");


using var connection = factory.CreateConnection();
var channel = connection.CreateModel();



//durable:true fiziksel olarak kaydedilmesini sağlar
//channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);




var randomQueueName = channel.QueueDeclare().QueueName; //random olarak queue adı tanımlar

//eğer bu kuyruğun kalıcı olmasını istiyorsak alttaki gibi bir kuyruk oluşturmamız gerek.
//var queueName = "log-database-save-queue";
//channel.QueueDeclare(queueName, true, false, false);

channel.QueueBind(randomQueueName, "logs-fanout", "", null); //bir kuyruk oluşturmak yerine onu bind ettik.


channel.BasicQos(0, 1, false);
var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume(randomQueueName, false, consumer);

Console.WriteLine("Logları dinleniyor...");

consumer.Received += (object sender, BasicDeliverEventArgs e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine("Gelen Mesaj: " + message);

    channel.BasicAck(e.DeliveryTag, false);

};

Console.ReadLine();