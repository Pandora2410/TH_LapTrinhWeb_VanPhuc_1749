using Lap02.Models;

namespace Lap02.Repositories
{
    public class MockCategoryRepository: ICategoryRepository
    {
        private List<Category> _categoryList;
        public MockCategoryRepository()
        {
            _categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Sách lập trình" },
                new Category { Id = 2, Name = "Sách kinh tế" },
                new Category { Id = 3, Name = "Sách kỹ năng sống" },
                new Category { Id = 4, Name = "Tiểu thuyết" },
                new Category { Id = 5, Name = "Sách thiếu nhi" },
                new Category { Id = 6, Name = "Sách ngoại ngữ" },
                new Category { Id = 7, Name = "Sách lịch sử" },
                new Category { Id = 8, Name = "Sách khoa học" },
                new Category { Id = 9, Name = "Sách văn học" },
                new Category { Id = 10, Name = "Truyện tranh" }
            };
        }
        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryList;
        }
    }
}
