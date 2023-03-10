using System.Threading.Channels;
using StackExchange.Redis;

var options = new ConfigurationOptions
{
    // add and update parameters as needed
    EndPoints = {"localhost:6379"}
};

// initalize a multiplexer with ConnectionMultiplexer.Connect()
var muxer = await ConnectionMultiplexer.ConnectAsync(options);

// get an IDatabase here with GetDatabase
var db = muxer.GetDatabase();

// check connection
Console.WriteLine($"ping: {db.Ping().TotalMilliseconds} ms");


// Subscribing to a redis channel using a dotnet channel to read incoming messages
var chanOpts = new BoundedChannelOptions(200)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleWriter = true
};
var chann = Channel.CreateBounded<string>(chanOpts);

// subscribe to a redis channel
var sub = muxer.GetSubscriber();

// async void as stated by Marc Gravell https://github.com/StackExchange/StackExchange.Redis/issues/639
await sub.SubscribeAsync("notifications", async (_,v) =>
{
    await chann.Writer.WriteAsync(v);
});

// await foreach (var msg in chann.Reader.ReadAllAsync())
// {
//     Console.WriteLine(msg);
// }

// transaction
var transaction = db.CreateTransaction();
var a = transaction.KeyExistsAsync("test-key");
var b = transaction.StringSetAsync("test-key", "value", null, When.Always);
var c = transaction.KeyExistsAsync("test-key");
await transaction.ExecuteAsync();
// tasks from transaction can only be awaited after transaction execution
Console.WriteLine(await a);
Console.WriteLine(await b);
Console.WriteLine(await c);