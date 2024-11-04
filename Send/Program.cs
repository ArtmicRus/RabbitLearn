using System.Text;
using RabbitMQ.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        // Создание подключения к серверу
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Создаёт очередь если она не создана
        channel.QueueDeclare(queue: "hello", // Имя очереди
                     durable: false, // Определяет, должна ли очередь сохраняться после перезапуска брокера RabbitMQ
                     exclusive: false, // Указывает, может ли эта очередь использоваться другими подключениями к серверу (false = может)
                     autoDelete: false, // Если этот параметр установлен в true, очередь автоматически удалится, когда все потребители отключатся от нее. В нашем случае false, значит, очередь останется до тех пор, пока её вручную не удалят.
                     arguments: null); // Дополнительные аргументы для настройки очереди. Например, можно указать политику истечения времени жизни сообщений (TTL), максимальное количество сообщений в очереди и другие параметры. В приведенном коде аргумент не указан (null), следовательно, используются значения по умолчанию

        const string message = "Hello World! 3672";
        var body = Encoding.UTF8.GetBytes(message);

        // Публикация сообщения
        channel.BasicPublish(exchange: string.Empty, // Название обмена (Exchange). Обмены распределяют сообщения по очередям согласно определенным правилам маршрутизации. В данном случае указано пустое значение string.Empty, что соответствует обмену типа default. Этот обмен отправляет сообщение прямо в очередь с таким же именем, как у ключа маршрутизации (routing key).
                             routingKey: "hello", // Ключ маршрутизации. Он определяет, куда именно должно попасть сообщение.
                             basicProperties: null, // Свойства сообщения. Это могут быть такие параметры, как заголовки, идентификатор сообщения, приоритет и т.п. В примере эти свойства не указаны (null), так что будут использованы стандартные значения.
                             body: body); // Тело сообщения. Здесь передается непосредственно само сообщение, которое нужно отправить. В переменной body содержится информация, которую вы хотите передать через RabbitMQ

        Console.WriteLine($" [x] Sent {message}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}