using Microsoft.EntityFrameworkCore;

namespace lab06.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Precision(10,2)]
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
