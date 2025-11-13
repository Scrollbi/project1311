using Microsoft.Win32;
using project2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace project2
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private List<ProductDisplay> allProducts;
        private bool sortAscending = true;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            txtUser.Text = "Пользователь: " + user.FullName;
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (var db = new Oshkinng207b2Context())
            {
                var productsFromDb = db.Products.ToList();

                allProducts = new List<ProductDisplay>();

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

                    
                    productDisplay.Image = LoadProductImage(product.ImagePath);

                    allProducts.Add(productDisplay);
                }

                productsGrid.ItemsSource = allProducts;

                
                var manufacturers = allProducts
                    .Where(p => p.Manufacturer != null)
                    .Select(p => p.Manufacturer)
                    .Distinct()
                    .ToList();

                manufacturers.Insert(0, "Все производители");
                cmbManufacturer.ItemsSource = manufacturers;
                cmbManufacturer.SelectedIndex = 0;
            }
        }

        private BitmapImage LoadProductImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return CreateSimplePlaceholder();
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return CreateSimplePlaceholder();
            }
        }

        private BitmapImage CreateSimplePlaceholder()
        {
            try
            {
                
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/Resources/no-image.png", UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        private void ApplyFilters()
        {
            var filtered = allProducts;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                filtered = filtered.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(search)) ||
                    (p.Category != null && p.Category.ToLower().Contains(search)) ||
                    (p.Manufacturer != null && p.Manufacturer.ToLower().Contains(search))
                ).ToList();
            }

            if (cmbManufacturer.SelectedItem != null && cmbManufacturer.SelectedItem.ToString() != "Все производители")
            {
                filtered = filtered.Where(p => p.Manufacturer == cmbManufacturer.SelectedItem.ToString()).ToList();
            }

            productsGrid.ItemsSource = filtered;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cmbManufacturer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            var products = productsGrid.ItemsSource as List<ProductDisplay>;
            if (products == null) return;

            if (sortAscending)
                productsGrid.ItemsSource = products.OrderBy(p => p.StockQuantity ?? 0).ToList();
            else
                productsGrid.ItemsSource = products.OrderByDescending(p => p.StockQuantity ?? 0).ToList();

            sortAscending = !sortAscending;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void BtnChangeImage_Click(object sender, RoutedEventArgs e)
        {
            if (productsGrid.SelectedItem is ProductDisplay selectedProduct)
            {
                ChangeProductImage(selectedProduct);
            }
            else
            {
                MessageBox.Show("Выберите товар для изменения фото");
            }
        }

        private void productsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (productsGrid.SelectedItem is ProductDisplay selectedProduct)
            {
                ChangeProductImage(selectedProduct);
            }
        }

        private void ChangeProductImage(ProductDisplay product)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Картинки|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                using (var db = new Oshkinng207b2Context())
                {
                    var productToUpdate = db.Products.FirstOrDefault(p => p.Id == product.Id);
                    if (productToUpdate != null)
                    {
                        productToUpdate.ImagePath = dialog.FileName;
                        db.SaveChanges();
                        LoadProducts();
                        MessageBox.Show("Фото обновлено");
                    }
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}