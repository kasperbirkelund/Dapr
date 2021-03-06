using Palprimes.Api.Hubs;
using Palprimes.Api.Services;
using Palprimes.Common.Logging;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers()
    .AddDapr();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpLogging(x => x.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestHeaders);
builder.Services.AddCors(options => options.AddPolicy("default", builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyMethod();
    builder.AllowAnyHeader();
}));

builder.Services.AddPalprimesServices();
builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetry("palprimesapi", builder.Logging, builder.Configuration);

var signalR = builder.Services.AddSignalR();

if (!builder.Environment.IsDevelopment())
{
    var redisSettings = builder.Configuration
        .GetSection("Redis");

    var redisHost = redisSettings.GetValue<string>("Host");
    var redisPassword = redisSettings.GetValue<string>("Password");

    //logger.LogInformation($"Redis host: {redisHost}");

    signalR.AddStackExchangeRedis(o =>
    {
        o.ConnectionFactory = async writer =>
        {
            var config = new ConfigurationOptions
            {
                AbortOnConnectFail = false
            };

            config.EndPoints.Add(redisHost, 6379);
            config.Password = redisPassword;
            config.SetDefaultPorts();

            var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
            connection.ConnectionFailed += (_, e) =>
            {
                //logger.LogError("Connection to Redis failed.");
            };

            connection.ConnectionRestored += (_, args) =>
            {
                //logger.LogInformation("Connection to Redis restored.");
            };

            //logger.LogInformation("Connected to Redis.");

            return connection;
        };
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("default");
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseWebSockets();
app.UseCloudEvents();
app.MapControllers();
app.MapHub<NotificationHub>("/notifications");
app.MapSubscribeHandler();
app.MapHealthChecks("/probe");

app.Run();
