using Lap02.Models;

namespace Lap02.Repositories
{
    public class MockProductRepository: IProductRepository
    {
        private readonly List<Product> _products;
        public MockProductRepository()
        {
            // Tạo một số dữ liệu mẫu
            _products = new List<Product>
                {
                new Product
        {
            Id = 1,
            Name = "Lập Trình C# Cơ Bản",
            Price = 120000,
            Description = "Cuốn sách hướng dẫn kiến thức nền tảng về C#, phù hợp cho người mới bắt đầu.",
            CategoryId = 1,
            ImageUrl = "/images/book-csharp.jpg"
        },

        new Product
        {
            Id = 2,
            Name = "ASP.NET Core MVC Thực Chiến",
            Price = 180000,
            Description = "Sách hướng dẫn xây dựng website bằng ASP.NET Core MVC theo mô hình thực tế.",
            CategoryId = 1,
            ImageUrl = "/images/book-aspnet.jpg"
        },

        new Product
        {
            Id = 3,
            Name = "Tư Duy Kinh Doanh Hiện Đại",
            Price = 150000,
            Description = "Giúp người đọc hiểu cách xây dựng tư duy kinh doanh và quản lý tài chính cá nhân.",
            CategoryId = 2,
            ImageUrl = "/images/book-business.jpg"
        },

        new Product
        {
            Id = 4,
            Name = "Kỹ Năng Giao Tiếp Hiệu Quả",
            Price = 95000,
            Description = "Cuốn sách giúp cải thiện khả năng giao tiếp, thuyết trình và làm việc nhóm.",
            CategoryId = 3,
            ImageUrl = "/images/book-communication.jpg"
        },

        new Product
        {
            Id = 5,
            Name = "Hành Trình Tuổi Trẻ",
            Price = 110000,
            Description = "Một tiểu thuyết nhẹ nhàng về tuổi trẻ, ước mơ và những lựa chọn trong cuộc sống.",
            CategoryId = 4,
            ImageUrl = "/images/book-novel.jpg"
        }
                };
        }
        public IEnumerable<Product> GetAll()
        {
            return _products;
        }
        public Product GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }
        public void Add(Product product)
        {
            product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);
        }
        public void Update(Product product)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index != -1)
            {
                _products[index] = product;
            }
        }
        public void Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }
    }
}
