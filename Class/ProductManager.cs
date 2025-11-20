using project2.Models;
using System.Collections.Generic;
using System.Linq;

namespace project2
{
    public class ProductManager
    {
        private User _currentUser;
        private List<ProductDisplay> allProducts;
        private bool sortAscending = true;

        public ProductManager(User user)
        {
            _currentUser = user;
            allProducts = new List<ProductDisplay>();
        }

    
        public List<ProductDisplay> LoadProducts(ImageHandler imageHandler)
        {
            using (var db = new Oshkinng207b2Context())
            {
                var productsFromDb = db.Products.ToList();

                
                var newProductList = new List<ProductDisplay>();

                foreach (var product in productsFromDb)
                {
                    var productDisplay = new ProductDisplay
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Category = product.Category,
                        Manufacturer = product.Manufacturer,
                        Price = product.Price,
                        Discount = product.Discount,
                        StockQuantity = product.StockQuantity,
                        ImagePath = product.ImagePath
                    };

                    productDisplay.Image = imageHandler.LoadProductImage(product.ImagePath);
                    newProductList.Add(productDisplay);
                }

                
                allProducts = newProductList;
                return allProducts;
            }
        }

        
        public List<string> GetManufacturers()
        {
            var manufacturers = allProducts
                .Where(p => p.Manufacturer != null)
                .Select(p => p.Manufacturer)
                .Distinct()
                .ToList();
            manufacturers.Insert(0, "Все производители");
            return manufacturers;
        }

        
        public List<ProductDisplay> ApplyFilters(string searchText, string selectedManufacturer)
        {
            
            IEnumerable<ProductDisplay> filtered = allProducts;

            if (!string.IsNullOrEmpty(searchText))
            {
                string search = searchText.ToLower();
                filtered = filtered.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(search)) ||
                    (p.Category != null && p.Category.ToLower().Contains(search)) ||
                    (p.Manufacturer != null && p.Manufacturer.ToLower().Contains(search))
                );
            }

            if (!string.IsNullOrEmpty(selectedManufacturer) && selectedManufacturer != "Все производители")
            {
                filtered = filtered.Where(p => p.Manufacturer == selectedManufacturer);
            }

            return filtered.ToList();
        }

       
        public List<ProductDisplay> SortProducts(List<ProductDisplay> products)
        {
            if (sortAscending)
                return products.OrderBy(p => p.StockQuantity ?? 0).ToList();
            else
                return products.OrderByDescending(p => p.StockQuantity ?? 0).ToList();
        }

        public void ToggleSortDirection() => sortAscending = !sortAscending;

        
        public void UpdateProductImage(int productId, string newImagePath)
        {
            using (var db = new Oshkinng207b2Context())
            {
                var productToUpdate = db.Products.FirstOrDefault(p => p.Id == productId);
                if (productToUpdate != null)
                {
                    productToUpdate.ImagePath = newImagePath;
                    db.SaveChanges();
                }
            }
        }

        
        public bool AddProduct(Product product)
        {
            try
            {
                using (var db = new Oshkinng207b2Context())
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        
        public bool UpdateProduct(Product product)
        {
            try
            {
                using (var db = new Oshkinng207b2Context())
                {
                    var existingProduct = db.Products.Find(product.Id);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = product.Name;
                        existingProduct.Category = product.Category;
                        existingProduct.Manufacturer = product.Manufacturer;
                        existingProduct.Price = product.Price;
                        existingProduct.Discount = product.Discount;
                        existingProduct.StockQuantity = product.StockQuantity;
                        existingProduct.ImagePath = product.ImagePath;

                        db.SaveChanges();
                        return true;
                    }
                }
            }
            catch
            {
                
            }
            return false;
        }

        
        public (bool success, string message) DeleteProduct(int productId)
        {
            try
            {
                using (var db = new Oshkinng207b2Context())
                {
                    var product = db.Products.Find(productId);
                    if (product != null)
                    {
                        

                        db.Products.Remove(product);
                        db.SaveChanges();
                        return (true, "Товар успешно удален");
                    }
                    return (false, "Товар не найден");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при удалении: {ex.Message}");
            }
        }

      
        public Product GetProductById(int id)
        {
            using (var db = new Oshkinng207b2Context())
            {
                return db.Products.Find(id);
            }
        }
    }
}