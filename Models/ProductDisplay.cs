using System.Windows.Media.Imaging;

public class ProductDisplay
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Manufacturer { get; set; }
    public decimal? Price { get; set; }
    public decimal? Discount { get; set; }
    public int? StockQuantity { get; set; }
    public string ImagePath { get; set; }
    public BitmapImage Image { get; set; }

   
    public decimal? DiscountedPrice
    {
        get
        {
            if (Price.HasValue && Discount.HasValue && Discount.Value > 0)
            {
                return Price.Value * (1 - Discount.Value / 100);
            }
            return Price;
        }
    }

    public bool HasHighDiscount
    {
        get
        {
            if (Discount.HasValue && Discount.Value > 15)
                return true;
            else
                return false;
        }
    }

    public bool HasAnyDiscount
    {
        get
        {
            if (Discount.HasValue && Discount.Value > 0)
                return true;
            else
                return false;
        }
    }

    public bool IsOutOfStock
    {
        get
        {
            if (!StockQuantity.HasValue || StockQuantity.Value <= 0)
                return true;
            else
                return false;
        }
    }
}