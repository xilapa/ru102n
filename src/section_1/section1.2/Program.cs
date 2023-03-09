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