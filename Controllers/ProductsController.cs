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



        [HttpPatch("{page}")]
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



        [HttpGet]
        public ActionResult<ProductWithPhotoD> GetProductById([FromQuery]int id)
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

                Dictionary<string, object> data = new Dictionary<string, object>
                {

                    { "Product",prod }
                };


                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = true,
                   Data = data
                });

            }
        }



        [Route("Category")]
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
        public ActionResult<List<Product>> AddProduct([FromForm] AddproductDTO request) {

          var product =  _productRepository.AddProduct(request);

            if (product != null)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false,
                    
                });
            }
   
            var errors = new List<string>();
            foreach(var error in product)
            {
                errors.Add(error.Description);    

            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = string.Join(",", errors) });
           
        }



        [HttpPut]
        public async Task<ActionResult<Product>> EditProduct(int id,[FromForm] AddproductDTO request)
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
        public async Task<IActionResult>GetNewest()
        {


           
            var re = await _productRepository.GetNewest();
            
            if (re.Count == 0 )
             {

                return Ok(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "No Data" }
                });
            }


            Dictionary<string, object> data = new Dictionary<string, object> 
            {
                {"Newest",re }
            };
             var response =  new OperationsResult
             {
                 statusCode = 200,
                 isError = false,
                Data = data
             };

            return Ok(response);

        }
    }
}
