using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace project2.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public string? Manufacturer { get; set; }

    public decimal? Price { get; set; }

    public decimal? Discount { get; set; }

    public int? StockQuantity { get; set; }

    public string? ImagePath { get; set; }
}