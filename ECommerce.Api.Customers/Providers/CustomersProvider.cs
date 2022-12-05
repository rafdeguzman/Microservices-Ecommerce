using AutoMapper;
using ECommerce.Api.Customers.Db;
using ECommerce.Api.Customers.Interfaces;
using ECommerce.Api.Customers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ECommerce.Api.Customers.Providers
{
    public class CustomersProvider : ICustomersProvider
    {
        private readonly CustomersDbContext context;
        private readonly ILogger<CustomersProvider> logger;
        private readonly IMapper mapper;

        public CustomersProvider(CustomersDbContext context, ILogger<CustomersProvider> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
            SeedData();
        }
        private void SeedData()
        {
            if (!context.Customers.Any())   
            {
                context.Customers.Add(new Db.Customer() { Id = 1, Name = "Bobby", Address = "123 lake st." });
                context.Customers.Add(new Db.Customer() { Id = 2, Name = "Johnny", Address = "124 lake st." });
                context.Customers.Add(new Db.Customer() { Id = 3, Name = "Manny", Address = "125 lake st." });
                context.SaveChanges();
            }
        }

        async Task<(bool IsSuccess, Models.Customer Customer, string ErrorMessage)> ICustomersProvider.GetCustomerAsync(int id)
        {
            try
            {
                var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == id);

                if (customer == null)
                    return (false, null, "Not found");

                var result = mapper.Map<Db.Customer, Models.Customer>(customer);
                return (true, result, null);
            }
            catch (System.Exception e)
            {
                logger?.LogError(e.ToString());
                return (false, null, e.Message);
            }
        }

        async Task<(bool IsSuccess, IEnumerable<Models.Customer> Customers, string ErrorMessage)> ICustomersProvider.GetCustomersAsync()
        {
            try
            {
                var customers = await context.Customers.ToListAsync();

                if (customers == null && !customers.Any())
                    return (false, null, "Not found");

                var result = mapper.Map<IEnumerable<Db.Customer>, IEnumerable<Models.Customer>>(customers);
                return (true, result, null);
            } 
            catch (System.Exception e)
            {
                logger?.LogError(e.ToString());
                return (false, null, e.Message);
            }
        }
    }
}
