using System;
namespace StockifyFunction.Stockify.Models
{
  public class Page
  {
    public Page(string url)
    {
      Url = url;
    }
    public string Url { get; private set; }
  }
}
