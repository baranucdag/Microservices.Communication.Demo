using System;
namespace SharedLibrary.Messages
{
	public class OrderItemMessage
	{
		public Guid ProductId { get; set; }
		public int Count { get; set; }
	}
}

