namespace blazor_multi_tab_ui.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<OrderModel>? Orders { get; set; }
        public int? TotalOrders 
        {
            get { return Orders?.Count(); }
        }
    }
}
