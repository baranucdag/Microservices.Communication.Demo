using MassTransit;
using MongoDB.Driver;
using SharedLibrary;
using Stock.API.Consumer;
using Stock.API.Models;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit((config) =>
{
    config.AddConsumer<OrderCeratedEventConsumer>();
    config.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
        _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCeratedEventConsumer>(context));
    });
});

builder.Services.AddSingleton<MongoDBService>();


using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();
MongoDBService mongoDBService = scope.ServiceProvider.GetService<MongoDBService>();
var collection = mongoDBService.GetCollection<Stock.API.Models.Stock>();
if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid(), Count = 2000 });
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid(), Count = 1000 });
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid(), Count = 300 });
    await collection.InsertOneAsync(new Stock.API.Models.Stock() { ProductId = Guid.NewGuid(), Count = 31 });
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

