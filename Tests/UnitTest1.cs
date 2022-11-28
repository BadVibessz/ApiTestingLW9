using Core;
using Microsoft.Extensions.Configuration;

namespace Tests;

public class Tests
{
    private readonly ProductsService _service = new();

    private readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .AddJsonFile("D:/MINE/ПРОГРАММИРОВАНИЕ/C#/Projects/TestingLW9/Tests/testobjects.json").Build();


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
        int createdId = _service.Create(productToCreate);
        var products = _service.GetAll();
        var createdProduct = _service.Get(createdId.ToString());

        // assert
        Assert.That(createdId != -1);
        Assert.That(products!.Find(p => p.id == createdId.ToString()) is not null);
        Assert.That(createdProduct.alias.ToUpperInvariant() == createdProduct.title.ToUpperInvariant());
        

        // delete
        _service.Delete(createdId.ToString());
    }

    [Test]
    public void Should_Successfully_Get_Product_By_Id() // получаем продукт по айди
    {
        // arrange
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        int createdId = _service.Create(productToCreate);
    
        // act
        var product = _service.Get(createdId.ToString());
    
        // assert
        Assert.That(product is not null);
        
        // delete
        _service.Delete(createdId.ToString());
    }

    [Test]
    public void Should_Successfully_Add_Few_Products() // создаем несколько валидных продуктов
    {
        // arrange
        var productToCreate1 = _config.GetSection("valid_product_2").Get<Product>()!;
        var productToCreate2 = _config.GetSection("valid_product_3").Get<Product>()!;

        // act
        int createdId1 = _service.Create(productToCreate1);
        int createdId2 = _service.Create(productToCreate2);
        var products = _service.GetAll();

        var createdProduct1 = _service.Get(createdId1.ToString());
        var createdProduct2 = _service.Get(createdId2.ToString());

        // assert
        Assert.That(createdId1 != -1);
        Assert.That(createdId2 != -1);
        Assert.That(products!.Find(p => p.id == createdId1.ToString()) is not null);
        Assert.That(products!.Find(p => p.id == createdId2.ToString()) is not null);

        // delete
        _service.Delete(createdId1.ToString());
        _service.Delete(createdId2.ToString());
    }

    [Test]
    public void Should_Successfully_Add_Products_With_Same_Title() // создаем продукт с уже существующим тайтлом
    {
        // arrange
        var productToCreate1 = _config.GetSection("valid_product_1").Get<Product>()!;
        var productToCreate2 = _config.GetSection("valid_product_1").Get<Product>()!;

        // act
        int createdId1 = _service.Create(productToCreate1);
        int createdId2 = _service.Create(productToCreate2);
        var products = _service.GetAll();
        var createdProducts = products!.FindAll(p => p.id == createdId1.ToString()
        || p.id == createdId2.ToString());

        // assert
        Assert.That(createdId1 != -1);
        Assert.That(createdId2 != -1);
        Assert.That(createdProducts[0] is not null);
        Assert.That(createdProducts[1] is not null);
        Assert.That(createdProducts[1]!.alias == createdProducts[0]!.alias + "-0");

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
        int createdId = _service.Create(productToCreate);
        var products = _service.GetAll();

        // assert
        Assert.That(!products!.Exists(p => p.id == createdId.ToString()));
    }

    [Test]
    public void Should_Not_Add_Null_Product() // пытаемся добавить пустой продукт
    {
        // arrange
        var productToCreate = _config.GetSection("null_product").Get<Product>();

        // act
        int createdId = _service.Create(productToCreate);
        var products = _service.GetAll();

        // assert
        Assert.That(createdId == -1);
    }

    [Test]
    public void Should_Successfully_Update_Product_And_Alias_Has_No_Id() // апдейтим продукт и алиас не должен содержать айди
    {
        // arrange
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("valid_update_product_1").Get<Product>()!;

        int createdId = _service.Create(productToUpdate);

        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == createdId.ToString())!;
        updater.id = temp.id;
        updater.alias = temp.alias;

        // act
        bool isSuccess = _service.Update(updater);
        products = _service.GetAll();

        var updatedProduct = products!.Find(p => p.id == updater.id);

        // assert
        Assert.That(isSuccess);

        Assert.That(updatedProduct?.title == updater.title);
        Assert.That(updatedProduct?.content == updater.content);
        Assert.That(updatedProduct?.price == updater.price);
        Assert.That(updatedProduct?.status == updater.status);
        Assert.That(updatedProduct?.keywords == updater.keywords);
        Assert.That(updatedProduct?.description == updater.description);
        Assert.That(updatedProduct?.hit == updater.hit);
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

        int createdId = _service.Create(productToUpdate);
        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == createdId.ToString());

        updater.id = temp.id;
        updater.alias = temp.alias;
        updater.title = temp.title;

        // act
        bool isSuccess = _service.Update(updater);
        products = _service.GetAll();

        var updatedProduct = products!.Find(p => p.id == updater.id);

        // assert
        Assert.That(isSuccess);

        Assert.That(updatedProduct?.title == productToUpdate.title);
        Assert.That(updatedProduct?.content == updater.content);
        Assert.That(updatedProduct?.price == updater.price);
        Assert.That(updatedProduct?.status == updater.status);
        Assert.That(updatedProduct?.keywords == updater.keywords);
        Assert.That(updatedProduct?.description == updater.description);
        Assert.That(updatedProduct?.hit == updater.hit);
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

        // act

        updater.id = notExistingProduct.id;

        bool isSuccess = _service.Update(updater);
        var products = _service.GetAll();

        var updatedProduct = products!.Find(p => p.id == updater.id);
        var createdProduct = products.Find(p => p.title == updater.title);

        // delete
        _service.Delete(createdProduct.id);

        // assert
        Assert.That(updatedProduct is null);
        Assert.That(createdProduct is null, "Был создан новый продукт");
        Assert.That(!isSuccess);
    }

    [Test]
    public void Should_Do_Nothing_When_Updating_With_Invalid_Product() // пытаемся апдейтить товар невалидными данными
    {
        // arrange
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("invalid_update_product").Get<Product>()!;

        int createdId = _service.Create(productToUpdate);

        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == createdId.ToString());
        updater.id = temp.id;

        // act
        bool isSuccess = _service.Update(updater);

        products = _service.GetAll();

        var updatedProduct = products!.Find(p => p.id == temp.id);

        // assert
        Assert.That(updatedProduct is not null, "продукт был удален, во время изменения");
        Assert.That(!isSuccess);
        Assert.That(updatedProduct?.title == productToUpdate.title);
        Assert.That(updatedProduct?.content == productToUpdate.content);
        Assert.That(updatedProduct?.price == productToUpdate.price);
        Assert.That(updatedProduct?.status == productToUpdate.status);
        Assert.That(updatedProduct?.keywords == productToUpdate.keywords);
        Assert.That(updatedProduct?.description == productToUpdate.description);
        Assert.That(updatedProduct?.hit == productToUpdate.hit);
        Assert.That(updatedProduct?.alias == productToUpdate.alias);

        // delete
        _service.Delete(temp.id);
    }


    [Test]
    public void Should_Successfully_Delete_Existing_Product() // удаялем существующий товар
    {
        // arrange
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        int createdId = _service.Create(productToCreate);

        // act
        var isSuccess = _service.Delete(createdId.ToString());
        var products = _service.GetAll();

        // assert
        Assert.That(isSuccess);
        Assert.That(!products!.Exists(p => p.id == createdId.ToString()));
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