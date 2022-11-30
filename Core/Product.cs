using System.Text.Json;
using Newtonsoft.Json;

namespace Core;



public class Product
{
    public string? id { get; set; }
    public string? category_id { get; set; }
    public string? title { get; set; }
    public string? alias { get; set; }
    public string? content { get; set; }
    public string? price { get; set; }
    public string? old_price { get; set; }
    public string? status { get; set; }
    public string? keywords { get; set; }
    public string? description { get; set; }
    public string? img { get; set; }
    public string? hit { get; set; }
    public string? cat { get; set; }

    public Product()
    {
    }

    public Product(string id, string categoryId, string title, string alias, string content,
        string price, string oldPrice, string status, string keywords,
        string description, string img, string hit, string cat)
    {
        this.id = id;
        category_id = categoryId;
        this.title = title;
        this.alias = alias;
        this.content = content;
        this.price = price;
        old_price = oldPrice;
        this.status = status;
        this.keywords = keywords;
        this.description = description;
        this.img = img;
        this.hit = hit;
        this.cat = cat;
    }

    public Product(Product other) : this(other.id, other.category_id, other.title, other.alias, other.content,
        other.price, other.old_price, other.status, other.keywords,
        other.description, other.img, other.hit, other.cat)
    {
    }


    public Product(string jsonData) : this(System.Text.Json.JsonSerializer.Deserialize<Product>(jsonData))
    {
    }
}