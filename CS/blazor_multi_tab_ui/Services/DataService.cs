using blazor_multi_tab_ui.Models;

namespace blazor_multi_tab_ui.Services
{
    public class DataService
    {
        private List<CustomerModel> customers;
        public DataService() { customers = GenerateTestData(); }

        public List<CustomerModel> GetCustomers() { return customers; }
        public List<OrderModel>? GetOrdersByCustomerId(int customerId) 
        {
            var customer = customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null) return null;
            return customer.Orders;
        }
        public List<CustomersChartPoint> GetCustomersChartData()
        {
            return customers.Select(customer => new CustomersChartPoint 
            { 
                Name = customer.Name,
                Orders = customer?.Orders?.Count ?? 0,
                Sales = customer?.Orders?.Sum(order => order.Cost) ?? 0
            }).ToList();
        }


        private List<CustomerModel> GenerateTestData()
        {
            var now = DateTime.Now;
            return new List<CustomerModel>{ 
                new CustomerModel()
                {
                    Id = 1,
                    Name = "John Doe",
                    Orders = new List<OrderModel>
                    {
                        new OrderModel{ Id = 1, Description = $"Apples", CreatedDate = new DateTime(now.Year, now.Month, now.Day ), Cost = 100 },
                        new OrderModel{ Id = 2, Description = $"Bananas", CreatedDate = new DateTime(now.Year, now.Month, now.Day - 1 ), Cost = 200 },
                    }
                },
                new CustomerModel()
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Orders = new List<OrderModel>
                    {
                        new OrderModel{ Id = 3, Description = $"Kiwi", CreatedDate = new DateTime(now.Year, now.Month, now.Day ), Cost = 150 },
                        new OrderModel{ Id = 4, Description = $"Durian", CreatedDate = new DateTime(now.Year, now.Month, now.Day - 1 ), Cost = 300 },
                        new OrderModel{ Id = 5, Description = $"Potatoes", CreatedDate = new DateTime(now.Year, now.Month, now.Day - 2 ), Cost = 42 },
                    }
                },
            };
        }
    }
}
