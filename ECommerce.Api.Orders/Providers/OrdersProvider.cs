using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using ECommerce.Api.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Order = ECommerce.Api.Orders.Db.Order;
using OrderItem = ECommerce.Api.Orders.Db.OrderItem;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrdersDbContext context;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext context, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
            SeedData();
        }
        
        private void SeedData()
        {
            {
                if (!context.Orders.Any())
                {
                    context.Orders.Add(new Order()
                    {
                        Id = 1,
                        CustomerId = 1,
                        OrderDate = DateTime.UtcNow,
                        Total = 100,
                        Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Id = 1,
                            OrderId = 1,
                            ProductId = 1,
                            Quantity = 1,
                            UnitPrice = 100
                        }
                    }
                    });
                    context.Orders.Add(new Order()
                    {
                        Id = 2,
                        CustomerId = 1,
                        OrderDate = DateTime.UtcNow,
                        Total = 200,
                        Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Id = 2,
                            OrderId = 2,
                            ProductId = 2,
                            Quantity = 1,
                            UnitPrice = 200
                        }
                    }
                    });
                    context.Orders.Add(new Order()
                    {
                        Id = 3,
                        CustomerId = 2,
                        OrderDate = DateTime.UtcNow,
                        Total = 300,
                        Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Id = 3,
                            OrderId = 3,
                            ProductId = 3,
                            Quantity = 1,
                            UnitPrice = 300
                        }
                    }
                    });
                    context.SaveChanges();
                }
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await context.Orders
                    .Where(o => o.CustomerId == customerId)
                    .Include(o => o.Items)
                    .ToListAsync();

                if (orders == null || orders.Count == 0)
                    return (false, null, "Not found");

                var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                return (true, result, null);
            }
            catch (System.Exception e)
            {
                logger.LogError(e.ToString());
                return (false, null, e.Message);
            }
        }
    }
}
