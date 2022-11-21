using Core;

var service = new ProductsService();

//var products = service.GetAll();

service.Create(new Product("9999", "1", "title-denchik", "title-denchik", "content",
    "10", "100", "0", "keywords",
    "desc", "img.jpg", "0", "men"));



//service.Delete("9837");

// service.Update(new Product("9836", "2", "title-denchik-335", "title-denchik-335", "content",
//      "101", "100", "0", "keywords",
//      "desc", "img.jpg", "0", "men"));
//
// products = service.GetAll();
// var z =3 ;


// while (products!.Find(p => p.title == "zykovProduct1") is not null)
// {
//     var z = products.Find(p => p.title == "zykovProduct1");
//     service.Delete(z!.id);
//     products = service.GetAll();
// }


// var temp = service.GetAll().Find(p => p.title == "new_title_34");
// var id = temp.id;
// var upd = new Product()
// {
//     title = temp.title, cat = temp.cat,
//     content = temp.content, hit = temp.hit, description = temp.description, img = temp.img,
//     keywords = temp.keywords, price = temp.price, old_price = temp.old_price, status = temp.status
//
// };

//
// temp.description = "new desc";
// temp.alias = null;
// service.Update(temp);

//service.Delete("10035");



// TODO: ПРИ ИЗМЕНЕНИЯ ТАЙТЛА АЛИАС ПРЕОБРЕТАЕТ ЗНАЧЕНИЕ ТАЙТЛА, НО ПРИ ИЗМЕНЕНИИ ДРУГИХ ПОЛЕЙ, КРОМЕ ТАЙТЛА В АЛИАС ДОБАВЛЯЕТСЯ АЙДИШНИК