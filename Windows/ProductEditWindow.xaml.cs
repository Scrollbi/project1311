using Microsoft.Win32;
using project2.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace project2.Windows
{
    public partial class ProductEditWindow : Window
    {
        private Product _product;
        private bool _isEditMode;
        private string _selectedImagePath;

        public ProductEditWindow(Product product = null)
        {
            InitializeComponent();

            if (product != null)
            {
                _product = product;
                _isEditMode = true;
                Title = "Редактирование товара";
                LoadProductData();
            }
            else
            {
                _product = new Product();
                _isEditMode = false;
                Title = "Добавление товара";
            }
        }

        private void LoadProductData()
        {
            txtName.Text = _product.Name;
            txtCategory.Text = _product.Category;
            txtManufacturer.Text = _product.Manufacturer;
            txtPrice.Text = _product.Price?.ToString();
            txtDiscount.Text = _product.Discount?.ToString();
            txtStockQuantity.Text = _product.StockQuantity?.ToString();

            if (!string.IsNullOrEmpty(_product.ImagePath))
            {
                _selectedImagePath = _product.ImagePath;
                txtImagePath.Text = Path.GetFileName(_product.ImagePath);
                LoadPreviewImage(_product.ImagePath);
            }
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Картинки|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {
               
                try
                {
                    var imageInfo = new BitmapImage(new Uri(dialog.FileName));
                    if (imageInfo.PixelWidth > 300 || imageInfo.PixelHeight > 200)
                    {
                        MessageBox.Show("Размер изображения не должен превышать 300×200 пикселей.", "Предупреждение",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось загрузить изображение.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _selectedImagePath = dialog.FileName;
                txtImagePath.Text = Path.GetFileName(_selectedImagePath);
                LoadPreviewImage(_selectedImagePath);
            }
        }

        private void LoadPreviewImage(string imagePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imgPreview.Source = bitmap;
            }
            catch
            {
                imgPreview.Source = null;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (var db = new Oshkinng207b2Context())
                {
                    if (_isEditMode)
                    {
                        
                        var productToUpdate = db.Products.Find(_product.Id);
                        if (productToUpdate != null)
                        {
                            UpdateProductData(productToUpdate);
                        }
                    }
                    else
                    {
                        
                        var newProduct = new Product();
                        UpdateProductData(newProduct);
                        db.Products.Add(newProduct);
                    }

                    db.SaveChanges();
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateProductData(Product product)
        {
            product.Name = txtName.Text.Trim();
            product.Category = string.IsNullOrEmpty(txtCategory.Text) ? null : txtCategory.Text.Trim();
            product.Manufacturer = string.IsNullOrEmpty(txtManufacturer.Text) ? null : txtManufacturer.Text.Trim();
            product.Price = decimal.TryParse(txtPrice.Text, out decimal price) ? price : (decimal?)null;
            product.Discount = decimal.TryParse(txtDiscount.Text, out decimal discount) ? discount : (decimal?)null;
            product.StockQuantity = int.TryParse(txtStockQuantity.Text, out int stock) ? stock : (int?)null;

            if (!string.IsNullOrEmpty(_selectedImagePath))
                product.ImagePath = _selectedImagePath;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название товара", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtName.Focus();
                return false;
            }

            if (decimal.TryParse(txtPrice.Text, out decimal price) && price < 0)
            {
                MessageBox.Show("Цена не может быть отрицательной", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtPrice.Focus();
                return false;
            }

            if (decimal.TryParse(txtDiscount.Text, out decimal discount) && (discount < 0 || discount > 100))
            {
                MessageBox.Show("Скидка должна быть в диапазоне от 0 до 100%", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtDiscount.Focus();
                return false;
            }

            if (int.TryParse(txtStockQuantity.Text, out int stock) && stock < 0)
            {
                MessageBox.Show("Количество на складе не может быть отрицательным", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStockQuantity.Focus();
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}