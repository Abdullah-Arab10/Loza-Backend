using Loza.Data;
using Loza.Entities;
using Loza.Models.DTO;

using Loza.Models.ResponseModels;
using Loza.Repository.Abstract;
using Microsoft.AspNetCore.Mvc;



namespace Loza.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly DataContext _context;
        private readonly IProductRepository _productRepository;
        public ProductsController(IProductRepository productRepository, DataContext context)
        {
            this._context = context;
            this._productRepository = productRepository;
        }


        [Route("api/Product/GetProducts")]
        [HttpGet]
        public ActionResult<IEnumerable<ProductsDTO>> GetProducts(int page =1, [FromQuery]string sort="",[FromQuery]string search = null) 
        {
            var products = _productRepository.GetProducts(page,sort,search);

            Dictionary<string,  object> data = new Dictionary<string, object>

            {
                { "products" , products}
            };

            return Ok(new OperationsResult
            {
                Data = data,
                isError = false,
                statusCode = 200

            });

                 
        }


        [Route("api/Product/GetProductsById/{id}")]
        [HttpGet]
        public ActionResult<ProductWithPhotoD> GetProductById(int id)
        {

          
            bool pro = _context.Product.Any(x => x.Id == id);
            if (pro == false)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "No Product Existe" }
                });
            }
            else
            {

              var prod = _productRepository.GetProductById(id);

               /* Dictionary<string, object> data = new Dictionary<string, object>
                {

                    { prod }
                };*/


                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = true,
                   Data = prod
                });

            }
        }



        [Route("Category/{catygorey}")]
        [HttpGet]
        public ActionResult<IEnumerable<ProductsDTO>> GetProductByCat(int catygorey)
        {
            bool pro = _context.Product.Any(x => x.Category == catygorey);
            if (pro == false)
            {
                 return Ok(new OperationsResult
                    {
                        statusCode = 404,
                        isError = true,
                        Errors = new ErrorModel { Message = "No Category Existe" }
                    });
            }
            else
            {

                var prod = _productRepository.GetProductByCat(catygorey);
                Dictionary<string, object> data = new Dictionary<string, object> { { "ProductsByCategory",prod} };
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = true,
                    Data = data
                });
            }
        }



        [HttpPost]
        public ActionResult<Product> AddProduct([FromForm] AddproductDTO request) {
            //  var pro = _context.Product.Where(p=>p.Name== request.Name&&p.Color==request.Color&&p.ColorNo==request.ColorNo).First();
            var pro = _context.Product.Any(p => p.Name == request.Name && p.Color == request.Color && p.ColorNo == request.ColorNo);

            if (pro == true)
            {
               /* pro.Quantity += request.Quantity;
                _context.SaveChanges();*/
                return Ok(new OperationsResult
                {
                    statusCode = 400,
                    isError = false,
                    Errors  = new ErrorModel {Message = "There is product with same Name and Color" }
                });
            }
            else
            {
                var product = _productRepository.AddProduct(request);

                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false,

                });
            }   

              /*  var errors = new List<string>();
                foreach (var error in product)
                {
                    errors.Add(error.Description);

                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = string.Join(",", errors) });*/
            
        }


        [Route("EditeProduct/{id}")]
        [HttpPut]
        public async Task<ActionResult<Product>> EditProduct(int id,[FromBody] AddproductDTO request)
        {
           
            bool pro = _context.Product.Any(x => x.Id == id);
            if (pro == false)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "No Product Existe" }
                });
            }
            else
            { 
              _productRepository.EditProduct(id,request);

                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = true,
                   
                });
            }
        }


        [HttpDelete]
        public async Task<ActionResult<List<Product>>> DeleteProduct(List<int> ids) {
            foreach (int id in ids) {
               
                bool pro = _context.Product.Any(x => x.Id == id);
                if (pro == false)
                {
                    return Ok(new OperationsResult { 
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel{Message="No Product Found"}
                    });
                }
            }
            

                    _productRepository.DeleteProduct(ids);

            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = true,
              
            });


        }


        [Route("Newest")]
        [HttpGet]
        public async Task<IActionResult>GetNewest(int userId)
        {
            int itemCount = 5;
           var topSoldItemsId = await _context.OrderItems

                        .GroupBy(sale => sale.Product_Id)
                        .OrderByDescending(group => group.Sum(sale => sale.total_amount))
                        .Select(group => group.Key)
                        .Take(5)
                        .ToListAsync();
            var topSoldItems = await _context.Product .Where(item => topSoldItemsId.Contains(item.Id))
                 .Select(s => new ProductsDTO
                 {
                     Id = s.Id,
                     Name = s.Name,
                     Description = s.Description,
                     Price = s.Price,
                     Category = s.Category,
                     Color = s.Color,
                     ColorNo = s.ColorNo,
                     Quantity = s.Quantity,
                     ProductDimensions = s.ProductDimensions,
                     ProductImage = s.ProductImage,
                     IsFavorite = _context.favorites.Any(p => p.ProductId == s.Id && p.UserId == userId)
                 })
            .ToListAsync();

           // var random = new Random();
            var randomItems = await _context.Product
                .OrderBy(x => Guid.NewGuid())  // Randomize the order of items
                .Take(itemCount)
                .ToListAsync();

            var re = await _productRepository.GetNewest(userId);
            
            if (re.Count == 0 )
             {

                return Ok(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "No Data" }
                });
            }

            Dictionary<string, object> data1 = new Dictionary<string, object>
            {
                {"Newest",re },
                {"Shuffel",randomItems },
                { "top5sales",topSoldItems }
               
            };


             var response =  new OperationsResult
             {
                 statusCode = 200,
                 isError = false,
                Data = data1
             };

            return Ok(response);

        }
    }
}
