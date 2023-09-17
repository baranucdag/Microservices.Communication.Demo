using System;
using MassTransit;
using MongoDB.Driver;
using SharedLibrary.Events;
using Stock.API.Services;

namespace Stock.API.Consumer
{
    public class OrderCeratedEventConsumer : IConsumer<OrderCreatedEvent>
    {

        IMongoCollection<Models.Stock> _stockCollection;

        public OrderCeratedEventConsumer(MongoDBService mongoDbService)
        {
            _stockCollection = mongoDbService.GetCollection<Models.Stock>();
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            foreach (var orderItem in context.Message.OrderItems)
            {
                stockResult.Add((await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());
            }

            if (stockResult.TrueForAll(sr => sr.Equals(true)))
            {
                foreach (var orderItem in context.Message.OrderItems)
                {
                    Models.Stock stock = await (await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstAsync();

                    stock.Count -= orderItem.Count;
                    await _stockCollection.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);
                }
            }
            else
            {

            }

        }
    }
}

