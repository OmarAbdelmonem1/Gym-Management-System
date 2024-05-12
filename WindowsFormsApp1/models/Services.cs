namespace WindowsFormsApp1.models
{
    public class Services
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Services(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }
}
