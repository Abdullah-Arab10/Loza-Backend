using LozaApi.Data;
using LozaApi.Models;
using LozaApi.Models.DTO;
using LozaApi.Repository.Abstract;
using Microsoft.AspNetCore.Mvc;



namespace LozaApi.Controllers
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
             return products;
        }



        [HttpGet]
        public ActionResult<ProductWithPhotoD> GetProductById([FromQuery]int id)
        {

          
            bool pro = _context.Product.Any(x => x.Id == id);
            if (pro == false)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Product does not exist" });
            }
            else
            {
                return _productRepository.GetProductById(id);

            }
        }



        [Route("Category")]
        [HttpGet]
        public ActionResult<IEnumerable<ProductsDTO>> GetProductByCat(int catygorey)
        {

            bool pro = _context.Product.Any(x => x.Category == catygorey);
            if (pro == false)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Category does not exist" });
            }
            else
            {
                return _productRepository.GetProductByCat(catygorey);

            }

        }



        [HttpPost]
        public ActionResult<List<Product>> AddProduct([FromForm] AddproductDTO request) {

          var product =  _productRepository.AddProduct(request);

            if (product != null)
            {
                return Ok(new Response { Status = "success", Message = "Product added successfully" });
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
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "Product does not exist" });
            }
            else
            { 
              _productRepository.EditProduct(id,request);

                return Ok(new Response { Status = "Success", Message = "Product Edited" });
            }
        }


        [HttpDelete]
        public async Task<ActionResult<List<Product>>> DeleteProduct(List<int> ids) {
            foreach (int id in ids) {
               
                bool pro = _context.Product.Any(x => x.Id == id);
                if (pro == false)
                {
                    var resp = new MyResponse();
                    resp.Errors.Add(new ErrorModel { Message = "Not Found" });
                    return NotFound(resp);
                }
            }
            

                    _productRepository.DeleteProduct(ids);
         
               return Ok(new Response { Status = "Success", Message = "Product/Products Deleted" });


        }


        [Route("Newest")]
        [HttpGet]
        public async Task<IActionResult>GetNewest()
        {


           
            var re = await _productRepository.GetNewest();
            
            if (re.Count == 0 )
             {

                 var resp = new MyResponse();
                 resp.Errors.Add(new ErrorModel { Message = "No Data" });
                 return BadRequest();
             }

             var response = new MyResponse
             {
                 Data = new List<object> (re)

             };

                return Ok(re);

        }
    }
}
