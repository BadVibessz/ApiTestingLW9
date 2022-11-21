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
    public void Should_Successfully_Get_All_Products()
    {
        // arrange

        // act
        var products = _service.GetAll();

        // assert
        Assert.That(products is not null);
    }

    [Test]
    public void Should_Successfully_Add_Product()
    {
        // arrange
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;

        // act
        bool isSuccess = _service.Create(productToCreate);
        var products = _service.GetAll();


        // assert
        Assert.That(isSuccess);
        Assert.That(products!.Find(p => p.title == productToCreate.title) is not null);

        // delete
        var createdProductId = products.Find(p => p.title == productToCreate.title)!.id;
        _service.Delete(createdProductId);
    }

    [Test]
    public void Should_Successfully_Add_Few_Products()
    {
        // arrange
        var productToCreate1 = _config.GetSection("valid_product_2").Get<Product>()!;
        var productToCreate2 = _config.GetSection("valid_product_3").Get<Product>()!;

        // act
        bool isSuccess1 = _service.Create(productToCreate1);
        bool isSuccess2 = _service.Create(productToCreate2);
        var products = _service.GetAll();


        // assert
        Assert.That(isSuccess1);
        Assert.That(isSuccess2);
        Assert.That(products!.Find(p => p.title == productToCreate1.title) is not null);
        Assert.That(products!.Find(p => p.title == productToCreate2.title) is not null);

        // delete
        var createdProductId1 = products.Find(p => p.title == productToCreate1.title)!.id;
        var createdProductId2 = products.Find(p => p.title == productToCreate2.title)!.id;
        _service.Delete(createdProductId1);
        _service.Delete(createdProductId2);
    }

    [Test]
    public void Should_Successfully_Add_Products_With_Same_Title()
    {
        // arrange
        var productToCreate1 = _config.GetSection("valid_product_1").Get<Product>()!;
        var productToCreate2 = _config.GetSection("valid_product_1").Get<Product>()!;

        // act
        bool isSuccess1 = _service.Create(productToCreate1);
        bool isSuccess2 = _service.Create(productToCreate2);
        var products = _service.GetAll();
        var createdProducts = products!.FindAll(p => p.title == productToCreate1.title);

        // assert
        Assert.That(isSuccess1);
        Assert.That(isSuccess2);
        Assert.That(createdProducts[0] is not null);
        Assert.That(createdProducts[1] is not null);
        Assert.That(createdProducts[1]!.alias == createdProducts[0]!.alias + "-0");

        // delete
        _service.Delete(createdProducts[0]!.id);
        _service.Delete(createdProducts[1]!.id);
    }

    [Test]
    public void Should_Not_Add_Invalid_Product()
    {
        // arrange
        var productToCreate = _config.GetSection("invalid_product").Get<Product>()!;

        // act
        bool isSuccess = _service.Create(productToCreate);
        var products = _service.GetAll();

        // assert
        Assert.That(isSuccess);
        Assert.That(!products!.Exists(p => p.title == productToCreate.title));
    }

    [Test]
    public void Should_Not_Add_Null_Product()
    {
        // arrange
        var productToCreate = _config.GetSection("null_product").Get<Product>()!;

        // act
        bool isSuccess = _service.Create(productToCreate);
        var products = _service.GetAll();

        // assert
        Assert.That(!isSuccess);
    }

    // [Test]
    // public void Should_Successfully_Update_Product_And_Not_Change_Alias()
    // {
    //     // arrange
    //     var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
    //     var updater = _config.GetSection("valid_update_product_1").Get<Product>()!;
    //
    //     // act
    //     _service.Create(productToUpdate);
    //
    //     var products = _service.GetAll();
    //     var temp = products!.Find(p => p.title == productToUpdate.title)!;
    //     updater.id = temp.id;
    //     updater.alias = temp.alias;
    //
    //     bool isSuccess = _service.Update(updater);
    //     products = _service.GetAll();
    //
    //     var updatedProduct = products!.Find(p => p.id == updater.id);
    //
    //     // assert
    //     Assert.That(isSuccess);
    //
    //     Assert.That(updatedProduct?.title == updater.title);
    //     Assert.That(updatedProduct?.content == updater.content);
    //     Assert.That(updatedProduct?.price == updater.price);
    //     Assert.That(updatedProduct?.status == updater.status);
    //     Assert.That(updatedProduct?.keywords == updater.keywords);
    //     Assert.That(updatedProduct?.description == updater.description);
    //     Assert.That(updatedProduct?.hit == updater.hit);
    //     Assert.That(!updatedProduct.alias.Contains(temp.id));
    //
    //     // delete
    //     _service.Delete(temp.id);
    // }


    [Test]
    public void Should_Successfully_Update_Product_And_Change_Alias()
    {
        // arrange
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("valid_update_product_2").Get<Product>()!;

        // act
        _service.Create(productToUpdate);

        var products = _service.GetAll();
        var temp = products!.Find(p => p.title == productToUpdate.title);
        updater.id = temp.id;
        updater.alias = temp.alias;
        updater.title = temp.title;

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
    public void Should_Do_Nothing_When_Updating_Not_Existing_Product()
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
        Assert.That(isSuccess);
        Assert.That(updatedProduct is null);
        Assert.That(createdProduct is null, "Был создан новый продукт");
        
        

    }

    [Test]
    public void Should_Do_Nothing_When_Updating_With_Invalid_Product() // TODO:
    {
        // arrange
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("invalid_update_product").Get<Product>()!;

        // act
        _service.Create(productToUpdate);

        var products = _service.GetAll();
        var temp = products!.Find(p => p.title == productToUpdate.title);
        updater.id = temp.id;

        bool isSuccess1 = _service.Update(updater); // todo: удаляет

        products = _service.GetAll();

        var updatedProduct = products!.Find(p => p.id == temp.id);

        // assert
        Assert.That(isSuccess1);

        Assert.That(updatedProduct is not null, "продукт был удален, во время изменения");
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
    public void Should_Successfully_Delete_Existing_Product()
    {
        // arrange
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        _service.Create(productToCreate);
        string id = _service.GetAll().Find(p => p.title == productToCreate.title).id;


        // act
        var isSuccess = _service.Delete(id);
        var products = _service.GetAll();

        // assert
        Assert.That(isSuccess);
        Assert.That(!products!.Exists(p => p.id == id));
    }

    [Test]
    public void Should_Do_Nothing_When_Deleting_Not_Existing_Product()
    {
        // arrange
        var notExistingProduct = _config.GetSection("not_existing_product").Get<Product>()!;

        // act
        var isSuccess = _service.Delete(notExistingProduct.id);

        // assert
        Assert.That(!isSuccess);
    }
}