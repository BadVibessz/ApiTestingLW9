using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Core;

public class ProductsService
{
    //"C:/Users/danil/source/repos/TestingLW9/Core/config.json" //TODO: make it dynamic
    private static readonly IConfigurationRoot Config = new ConfigurationBuilder()
        .AddJsonFile("D:/MINE/ПРОГРАММИРОВАНИЕ/C#/Projects/TestingLW9/Core/config.json").Build();

    private readonly string _uri = Config["BASE_URI"]!;
    private readonly string _api = Config["API_URI"]!;
    private readonly HttpClient _client;

    public ProductsService()
    {
        this._client = new(new HttpClientHandler()) { BaseAddress = new Uri(_uri) };
    }


    private bool GetResponseStatus(HttpResponseMessage? response)
    {
        var temp = (response?.Content.ReadAsStringAsync().Result.Split(':')[1])?[1];

        if (temp is not null)
        {
            if (temp == '1') return true;
            return false;
        }

        return false;
    }

    public List<Product>? GetAll()
    {
        List<Product>? list = new();
        var response = _client.Send(new HttpRequestMessage(HttpMethod.Get, $"{_api}/products"));


        var content = response.Content.ReadAsStringAsync().Result;
        var jsonOutput = JsonDocument.Parse(content).RootElement;

        try
        {
            list = JsonConvert.DeserializeObject<List<Product>>(jsonOutput.GetRawText());
        }
        catch
        {
            //TODO();
            return null;
        }

        return list;
    }

    public Product? Get(string id)
        => GetAll()?.Find(p => p.id == id);

    public bool Create(Product product)
    {
        bool flag = false;
        try
        {
            var jsonObject = JsonSerializer.Serialize(product);

            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var response = _client.PostAsync($"{_api}/addproduct", content).Result;

            flag = GetResponseStatus(response);
            if (response.StatusCode != HttpStatusCode.OK) return false;
        }
        catch
        {
            return false;
        }

        return flag;
    }

    public bool Delete(string productId)
    {
        bool flag = false;
        try
        {
            var response = _client
                .Send(new HttpRequestMessage(HttpMethod.Get, $"{_api}/deleteproduct?id={productId}"));

            flag = GetResponseStatus(response);
            if (response.StatusCode != HttpStatusCode.OK) return false;
        }
        catch
        {
            return false;
        }

        return flag;
    }

    public bool Update(Product newProduct)
    {
        bool flag = false;
        try
        {
            var jsonObject = JsonSerializer.Serialize(newProduct);

            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var response = _client.PostAsync($"{_api}/editproduct", content).Result;

            flag = GetResponseStatus(response);
            if (response.StatusCode != HttpStatusCode.OK) return false;
        }
        catch
        {
            return false;
        }

        return flag;
    }
}