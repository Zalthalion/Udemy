using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var productFromDb = _db.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productFromDb is not null)
            {
                productFromDb.Title = product.Title;
                productFromDb.ISBN = product.ISBN;
                productFromDb.Price = product.Price;
                productFromDb.Price50 = product.Price50;
                productFromDb.ListPrice = product.ListPrice;
                productFromDb.Price100 = product.Price100;
                productFromDb.Description = product.Description;
                productFromDb.Category = product.Category;
                productFromDb.Author = product.Author;
                productFromDb.Cover = product.Cover;
                // Only updates the image URL when creating a new product
                productFromDb.ImageUrl = product.ImageUrl is not null ? product.ImageUrl : productFromDb.ImageUrl;
            }
        }
    }
}
