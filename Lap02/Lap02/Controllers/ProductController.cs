using Lap02.Models;
using Lap02.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lap02.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string? searchString, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                products = products
                    .Where(p =>
                        p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        (!string.IsNullOrEmpty(p.Description) &&
                         p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products
                    .Where(p => p.CategoryId == categoryId.Value)
                    .ToList();
            }

            if (minPrice.HasValue)
            {
                products = products
                    .Where(p => p.Price >= minPrice.Value)
                    .ToList();
            }

            if (maxPrice.HasValue)
            {
                products = products
                    .Where(p => p.Price <= maxPrice.Value)
                    .ToList();
            }

            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            ViewBag.Categories = await _categoryRepository.GetAllCategories();

            return View(products);
        }

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Add(Product product, IFormFile? imageUrl, List<IFormFile>? imageUrls)
        {
            if (ModelState.IsValid)
            {
                product.Id = 0;

                if (imageUrl != null && imageUrl.Length > 0)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                if (imageUrls != null && imageUrls.Count > 0)
                {
                    product.Images = new List<ProductImage>();

                    foreach (var file in imageUrls)
                    {
                        if (file != null && file.Length > 0)
                        {
                            var imagePath = await SaveImage(file);

                            product.Images.Add(new ProductImage
                            {
                                Url = imagePath
                            });
                        }
                    }
                }

                await _productRepository.Add(product);

                return RedirectToAction("Index");
            }

            var categories = await _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Update(Product product, IFormFile? imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                await _productRepository.Update(product);

                return RedirectToAction("Index");
            }

            var categories = await _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetById(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.Delete(id);

            return RedirectToAction("Index");
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var savePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + fileName;
        }
    }
}