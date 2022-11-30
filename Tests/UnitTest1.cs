using Core;
using Microsoft.Extensions.Configuration;
using NUnit.Framework.Interfaces;

namespace Tests;

public class Tests
{
    private readonly ProductsService _service = new();

    private readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .AddJsonFile("D:/MINE/ПРОГРАММИРОВАНИЕ/C#/Projects/TestingLW9/Tests/testobjects.json").Build();

    private readonly List<string> _productIds = new();


    private bool AreEqual(Product p1, Product p2)
    {
        Assert.That(p1.title == p2.title);
        Assert.That(p1.content == p2.content);
        Assert.That(p1.price == p2.price);
        Assert.That(p1.status == p2.status);
        Assert.That(p1.keywords == p2.keywords);
        Assert.That(p1.description == p2.description);
        Assert.That(p1.hit == p2.hit);

        return true;
    }

    [TearDown]
    public void Delete()
    {
        foreach (var id in _productIds)
            _service.Delete(id);
    }

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Should_Successfully_Get_All_Products() // получаем продукты
    {
        // arrange

        // act
        var products = _service.GetAll();

        // assert
        Assert.That(products is not null);
    }

    [Test]
    public void Should_Successfully_Add_Product() // создаем валидный продукт
    {
        // arrange
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;

        // act
        var creationResponse = _service.Create(productToCreate);

        // assert
        _productIds.Add(creationResponse.Id);

        var products = _service.GetAll();
        var createdProduct = _service.Get(creationResponse.Id);

        Assert.That(creationResponse.Id != "-1");
        Assert.That(products!.Find(p => p.id == creationResponse.Id) is not null);
        Assert.That(createdProduct.alias.ToUpperInvariant() == createdProduct.title.ToUpperInvariant());
        Assert.That(AreEqual(productToCreate, createdProduct));

        // // delete
        // _service.Delete(creationResponse.ToString());
    }

    [Test]
    public void Should_Successfully_Get_Product_By_Id() // получаем продукт по айди
    {
        // arrange
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        var creationResponse = _service.Create(productToCreate);

        // act
        var product = _service.Get(creationResponse.Id);

        // assert
        _productIds.Add(creationResponse.Id);

        Assert.That(product is not null);
        Assert.That(AreEqual(productToCreate, product));

        // delete
        _service.Delete(creationResponse.Id);
    }

    [Test]
    public void Should_Successfully_Add_Few_Products() // создаем несколько валидных продуктов
    {
        // arrange
        var productToCreate1 = _config.GetSection("valid_product_2").Get<Product>()!;
        var productToCreate2 = _config.GetSection("valid_product_3").Get<Product>()!;

        // act
        var creationResponse1 = _service.Create(productToCreate1);
        var creationResponse2 = _service.Create(productToCreate2);


        // assert
        _productIds.Add(creationResponse1.Id);
        _productIds.Add(creationResponse2.Id);

        var products = _service.GetAll();
        var createdProduct1 = _service.Get(creationResponse1.Id);
        var createdProduct2 = _service.Get(creationResponse2.Id);

        Assert.That(creationResponse1.Id != "-1");
        Assert.That(creationResponse2.Id != "-1");
        Assert.That(products!.Find(p => p.id == creationResponse1.Id) is not null);
        Assert.That(products.Find(p => p.id == creationResponse2.Id) is not null);
        Assert.That(AreEqual(productToCreate1, createdProduct1));
        Assert.That(AreEqual(productToCreate2, createdProduct2));

        // delete
        _service.Delete(creationResponse1.Id);
        _service.Delete(creationResponse2.Id);
    }

    [Test]
    public void Should_Successfully_Add_Products_With_Same_Title() // создаем продукт с уже существующим тайтлом
    {
        // arrange
        var productToCreate1 = _config.GetSection("valid_product_1").Get<Product>()!;
        var productToCreate2 = _config.GetSection("valid_product_1").Get<Product>()!;

        // act
        var creationResponse1 = _service.Create(productToCreate1);
        var creationResponse2 = _service.Create(productToCreate2);


        // assert
        _productIds.Add(creationResponse1.Id);
        _productIds.Add(creationResponse2.Id);


        var products = _service.GetAll();
        var createdProducts = products!.FindAll(p => p.id == creationResponse1.Id
                                                     || p.id == creationResponse2.Id);

        Assert.That(creationResponse1.Id != "-1");
        Assert.That(creationResponse2.Id != "-1");
        Assert.That(createdProducts[0] is not null);
        Assert.That(createdProducts[1] is not null);
        Assert.That(createdProducts[1]!.alias == createdProducts[0]!.alias + "-0");
        Assert.That(AreEqual(productToCreate1, createdProducts[0]));
        Assert.That(AreEqual(productToCreate2, createdProducts[1]));

        // delete
        _service.Delete(createdProducts[0]!.id);
        _service.Delete(createdProducts[1]!.id);
    }

    [Test]
    public void Should_Not_Add_Invalid_Product() // пытаемся добавить невалидный продукт
    {
        // arrange
        var productToCreate = _config.GetSection("invalid_product").Get<Product>()!;

        // act
        var creationResponse = _service.Create(productToCreate);

        // assert
        var products = _service.GetAll();

        Assert.That(!products!.Exists(p => p.id == creationResponse.Id));
        Assert.That(!creationResponse.Status, "Товар не был создан, но api говорит об обратном");
        Assert.That(creationResponse.Id == "-1");
    }

    [Test]
    public void Should_Not_Add_Null_Product() // пытаемся добавить пустой продукт
    {
        // arrange
        var productToCreate = _config.GetSection("null_product").Get<Product>();

        // act
        var creationResponse = _service.Create(productToCreate);

        // assert
        var products = _service.GetAll();

        Assert.That(products!.Find(p => p.id == creationResponse.Id) is null);
        Assert.That(!creationResponse.Status);
        Assert.That(creationResponse.Id == "-1");
    }

    [Test]
    public void Should_Successfully_Update_Product_And_Alias_Has_No_Id() // апдейтим продукт и алиас не должен содержать айди
    {
        // arrange
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("valid_update_product_1").Get<Product>()!;

        var creationResponse = _service.Create(productToUpdate);

        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == creationResponse.Id)!;
        updater.id = temp.id;
        updater.alias = temp.alias;

        // act
        bool isSuccess = _service.Update(updater);


        // assert
        _productIds.Add(creationResponse.Id);

        products = _service.GetAll();
        var updatedProduct = products!.Find(p => p.id == updater.id);

        Assert.That(isSuccess);
        Assert.That(AreEqual(updatedProduct, updater));
        Assert.That(!updatedProduct.alias.Contains(temp.id));

        // delete
        _service.Delete(temp.id);
    }


    [Test]
    public void Should_Successfully_Update_Product_And_Alias_Has_Id() // апдейтим продукт и алиса должен содержать айди
    {
        // arrange
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("valid_update_product_2").Get<Product>()!;

        var creationResponse = _service.Create(productToUpdate);
        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == creationResponse.Id);

        updater.id = temp.id;
        updater.alias = temp.alias;
        updater.title = temp.title;

        // act
        bool isSuccess = _service.Update(updater);


        // assert
        _productIds.Add(creationResponse.Id);

        products = _service.GetAll();
        var updatedProduct = products!.Find(p => p.id == updater.id);

        Assert.That(isSuccess);
        Assert.That(AreEqual(updatedProduct, updater));
        Assert.That(updatedProduct?.alias != updater.alias);
        Assert.That(updatedProduct.alias.Contains(temp.id));

        // delete
        _service.Delete(temp.id);
    }

    [Test]
    public void Should_Do_Nothing_When_Updating_Not_Existing_Product() // изменяем несуществующий продукт
    {
        // arrange
        var notExistingProduct = _config.GetSection("not_existing_product").Get<Product>()!;
        var updater = _config.GetSection("valid_update_product_1").Get<Product>()!;
        updater.id = notExistingProduct.id;

        // act
        bool isSuccess = _service.Update(updater);


        // assert
        var products = _service.GetAll();

        var updatedProduct = products!.Find(p => p.id == updater.id);

        // так как апи на editproduct возвращает только status, то я не знаю как отследить айди товара,
        // который был создан (баг апи?), поэтому, так как мое название товара уникальное, 
        // я веду поиск по тайтлу.
        _productIds.Add(products.Find(p => p.title == updater.title).id);


        //_service.Delete(createdProduct.id);

        Assert.That(!isSuccess, "Был создан новый продукт");
        Assert.That(updatedProduct is null);
    }

    [Test]
    public void Should_Do_Nothing_When_Updating_With_Invalid_Product() // пытаемся апдейтить товар невалидными данными
    {
        // arrange
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("invalid_update_product").Get<Product>()!;

        var creationResponse = _service.Create(productToUpdate);

        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == creationResponse.Id);
        updater.id = temp.id;

        // act
        bool isSuccess = _service.Update(updater);

        // assert
        _productIds.Add(creationResponse.Id);

        products = _service.GetAll();
        var updatedProduct = products!.Find(p => p.id == temp.id);

        Assert.That(updatedProduct is not null, "Продукт был удален, во время изменения");
        Assert.That(!isSuccess);
        Assert.AreEqual(updatedProduct, productToUpdate);
        Assert.That(updatedProduct?.alias == productToUpdate.alias);

        // delete
        _service.Delete(temp.id);
    }


    [Test]
    public void Should_Successfully_Delete_Existing_Product() // удаялем существующий товар
    {
        // arrange
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        var creationResponse = _service.Create(productToCreate);

        // act
        var isSuccess = _service.Delete(creationResponse.Id);
        

        // assert
        _productIds.Add(creationResponse.Id);
        
        var products = _service.GetAll();
        Assert.That(isSuccess);
        Assert.That(!products!.Exists(p => p.id == creationResponse.Id));
    }

    [Test]
    public void Should_Do_Nothing_When_Deleting_Not_Existing_Product() // удаляем несуществующий товар
    {
        // arrange
        var notExistingProduct = _config.GetSection("not_existing_product").Get<Product>()!;

        // act
        var isSuccess = _service.Delete(notExistingProduct.id);

        // assert
        Assert.That(!isSuccess);
    }
}