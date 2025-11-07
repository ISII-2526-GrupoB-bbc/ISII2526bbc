using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Alias para desambiguar IModel (RabbitMQ) vs IModel (EF Core)
using RmqModel = RabbitMQ.Client.IModel;
using RmqConnection = RabbitMQ.Client.IConnection;

namespace AppForSEII2526.API; // <-- usa tu namespace

public sealed class Subscriber : IDisposable
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _user;
    private readonly string _pass;
    private readonly string _exchangeName;

    private RmqConnection _conn = default!;
    private RmqModel _ch = default!;   // ¡no nullable!

    public Subscriber(
        string host = "localhost", int port = 5672,
        string user = "guest", string pass = "guest",
        string exchangeName = "logs")
    {
        _host = host; _port = port; _user = user; _pass = pass; _exchangeName = exchangeName;
    }

    public void Start()
    {
        // (1) Conexión
        var factory = new ConnectionFactory { HostName = _host, Port = _port, UserName = _user, Password = _pass };
        _conn = factory.CreateConnection();

        // (2) Canal
        _ch = _conn.CreateModel();

        // (3) Exchange fanout (durable opcional)
        _ch.ExchangeDeclare(_exchangeName, ExchangeType.Fanout, durable: true);

        // (4) Cola efímera/exclusiva/autodelete
        var q = _ch.QueueDeclare(queue: "", durable: false, exclusive: true, autoDelete: true, arguments: null);
        var queueName = q.QueueName;

        // (5) Binding cola<->exchange (sin routingKey en fanout)
        _ch.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: "");

        // (6) Consumidor y (7) callback
        var consumer = new EventingBasicConsumer(_ch);
        consumer.Received += (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var ts = root.GetProperty("timestamp").GetDateTimeOffset();
                var lvl = root.GetProperty("level").GetString();
                var cat = root.TryGetProperty("category", out var c) ? c.GetString() : "?";
                var msg = root.GetProperty("message").GetString();

                Console.WriteLine($"[{ts:u}] {lvl,-10} {cat}: {msg}");
            }
            catch { Console.WriteLine($"[RAW] {json}"); }
        };

        // (8) Iniciar consumo
        _ch.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        Console.WriteLine($"[LogViewer] Subscrito a '{_exchangeName}'. Esperando logs…");
    }

    public void Dispose()
    {
        try { _ch?.Close(); } catch { }
        try { _conn?.Close(); } catch { }
        _ch?.Dispose();
        _conn?.Dispose();
    }
}
