using System;
namespace StockifyFunction.Stockify.Models
{
  public class ProductPage : Page
  {
    public string Name { get; }
    public string AddToCartButtonSelector { get; private set; }

    public ProductPage(string name, string url, string addToCartSelector) : base(url)
    {
      Name = name;
      AddToCartButtonSelector = addToCartSelector;
    }
  }
}
