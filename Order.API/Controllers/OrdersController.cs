using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entites;
using Order.API.ViewModels;
using SharedLibrary.Events;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        readonly OrderAPIDbContext _context;
        readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(OrderAPIDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = _context.Set<Models.Entites.Order>().ToList();
            return Ok(result);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost()]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderVM createOrderVM)
        {
            Models.Entites.Order order = new()
            {
                OrderId = Guid.NewGuid(),
                BuyerId = createOrderVM.BuyerId,
                CreatedDate = DateTime.UtcNow,
                OrderStatus = Models.Enums.OrderStatus.Suspend
            };

            order.OrderItems = createOrderVM.OrderItems.Select(oi => new OrderItem
            {
                Id = Guid.NewGuid(),
                Count = oi.Count,
                Price = oi.Price,
                ProductId = oi.ProductId,
                Order = order
            }).ToList();

            order.TotalPrice = createOrderVM.OrderItems.Sum(oi => (oi.Price * oi.Count));

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            OrderCreatedEvent orderCreatedEvent = new()
            {
                BuyerId = order.BuyerId,
                OrderId = order.OrderId,
                OrderItems = order.OrderItems.Select(oi => new SharedLibrary.Messages.OrderItemMessage
                {
                    Count = oi.Count,
                    ProductId = oi.ProductId
                }).ToList()
            };

            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok();
        }

    }
}

