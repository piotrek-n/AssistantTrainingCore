using AssistantTrainingCore.Repositories;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;

namespace AssistantTrainingCore.Controllers
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    public class Product
    {
        public Category Category { get; set; }
        public int CategoryID { get; set; }
        public bool Discontinued { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string QuantityPerUnit { get; set; }
        public int ReorderLevel { get; set; }
        public int SupplierID { get; set; }
        public double UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int UnitsOnOrder { get; set; }
    }

    [Produces("application/json")]
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private readonly IReportsRepository reportsRepository;

        public ProductsController(IReportsRepository reportsRepository)
        {
            this.reportsRepository = reportsRepository;
        }
        // GET: api/Products
        // Model kazdego raportu w tsx
        [HttpGet]
        public JsonResult GetProducts([DataSourceRequest] DataSourceRequest request, [FromQuery] int value)
        {

            var json = string.Empty;
            switch (value)
            {
                case 1:
                    json = "{}"; ReportsRepository.EmptyReport();
                    break;

                case 2:
                    var r2 = reportsRepository.IncompleteTrainingResult();
                    return Json(r2?.ToDataSourceResult(request));
                    break;

                case 3:
                    var r3 = reportsRepository.InstructionsWithoutTrainingResult();
                    return Json(r3?.ToDataSourceResult(request));
                    break;

                case 4:
                    json = reportsRepository.WorkersWithoutTrainingJSON();
                    var r4 = reportsRepository.WorkersWithoutTrainingResult();
                    return Json(r4?.ToDataSourceResult(request));

                default:
                    break;
            }

            var products = new List<Product>() {
              new Product{
                ProductID= 1,
                ProductName= "Chai" + value,
                SupplierID= 1,
                CategoryID= 1,
                QuantityPerUnit= "10 boxes x 20 bags",
                UnitPrice= 18.0000,
                UnitsInStock= 39,
                UnitsOnOrder= 0,
                ReorderLevel= 10,
                Discontinued= false,
                Category= new Category {
                            CategoryID= 1,
                  CategoryName= "Beverages",
                  Description= "Soft drinks, coffees, teas, beers, and ales"
                }
                    },
                new Product {
                ProductID=  2,
                ProductName=  "Chang" + value,
                SupplierID=  1,
                CategoryID=  1,
                QuantityPerUnit=  "24 - 12 oz bottles",
                UnitPrice=  19.0000,
                UnitsInStock=  17,
                UnitsOnOrder=  40,
                ReorderLevel=  25,
                Discontinued=  false,
                Category = new Category {
                  CategoryID=  1,
                  CategoryName=  "Beverages",
                  Description=  "Soft drinks, coffees, teas, beers, and ales"
                }
              },
                new Product {
                ProductID=  3,
                ProductName=  "Aniseed Syrup" + value,
                SupplierID=  1,
                CategoryID=  2,
                QuantityPerUnit=  "12 - 550 ml bottles",
                UnitPrice=  10.0000,
                UnitsInStock=  13,
                UnitsOnOrder=  70,
                ReorderLevel=  25,
                Discontinued=  false,
                Category= new Category {
                  CategoryID=  2,
                  CategoryName=  "Condiments",
                  Description=  "Sweet and savory sauces, relishes, spreads, and seasonings"
                }
  }, };
            // ToDataSourceResult works with IQueryable and thus Entity Framework could be used as a source
            // instead of an in-memory list.
            return Json(products?.ToDataSourceResult(request));
        }
        
    }
}