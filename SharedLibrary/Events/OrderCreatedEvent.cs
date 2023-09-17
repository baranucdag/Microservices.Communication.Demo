using System;
using SharedLibrary.Events.Common;
using SharedLibrary.Messages;

namespace SharedLibrary.Events
{
	public class OrderCreatedEvent : IEvent
	{
		public Guid OrderId { get; set; }
		public Guid BuyerId{ get; set; }
		public List<OrderItemMessage> OrderItems { get; set; }
	}
}

