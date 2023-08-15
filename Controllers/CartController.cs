using Loza.Data;
using Loza.Entities;
using Loza.Models.DTO;
using Loza.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders.Physical;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;

        }

        [Route("AddToCart")]
        [HttpPost]
        public async Task<ActionResult> AddProductToCart(AddToCart addcart) {

         

            var prodctId = await _dataContext.Product.Where(p => p.Name == addcart.name && p.Color == addcart.color && p.ColorNo == addcart.colorno).Select(p => new {id=p.Id,quant=p.Quantity }).FirstAsync();
            //var prodquant = await _dataContext.Product.Where(p => p.Id == prodctId.id).FirstAsync();
            if (prodctId.quant<addcart.quant)
            {


                return Ok(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "no enought products in stock" }
                });
            }
            else
            {
                var find = await _dataContext.ShoppingCarts.FirstOrDefaultAsync(p => p.ProductId == prodctId.id && p.UserId == addcart.userId);
                
                if (find != null)
                {
                    find.Quant = find.Quant + addcart.quant ;
                    if (prodctId.quant < find.Quant)
                    {

                        return Ok(new OperationsResult
                        {
                            statusCode = 400,
                            isError = true,
                            Errors = new ErrorModel { Message = "no enought products in stock" }
                        });
                    }
                    else
                    {
                        await _dataContext.SaveChangesAsync();

                        return Ok();
                    }
                }
                var pro = await _dataContext.Product.FirstOrDefaultAsync(P => P.Id == prodctId.id);
                var cart = new ShoppingCart
                {
                    ProductId = prodctId.id,
                    ProductName = pro.Name,
                    price = pro.Price,
                    UserId =addcart.userId,
                    Quant = addcart.quant

                };

                await _dataContext.ShoppingCarts.AddAsync(cart);
                await _dataContext.SaveChangesAsync();
                var response = new OperationsResult
                {
                    statusCode = 200,
                    isError = false,
                    
                };

                return Ok(response);

            }
        }
        [Route("GetUserCart")]
        [HttpGet]
        public async Task<ActionResult> GetUserCart(int userId)
        {
            //  var cartitems = from c in _dataContext.ShoppingCarts join p in _dataContext.Product on c.ProductId equals p.Id select p;
            var car = await _dataContext.ShoppingCarts.Where(p => p.UserId == userId)
            .Select(s => new CartDTO
            {
                price = s.Total,
                Qunatity = s.Quant,
                ProductName = s.ProductName,
                id = (int)s.ProductId,
                Photo =  _dataContext.Product.Where(p => p.Id == s.ProductId).Select( p => p.ProductImage).First(),
                color = _dataContext.Product.Where(p => p.Id == s.ProductId).Select(p => p.Color).First(),
                colorNo = _dataContext.Product.Where(p => p.Id == s.ProductId).Select(p => p.ColorNo).First()
            })
            .ToListAsync();


            Dictionary<string,object> data = new Dictionary<string, object>
            {
                { "UserCart",car}
            };

            var response = new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            };

            return Ok(response);

        }
        [Route("DeleteFromCart")]
        [HttpDelete]
        public async Task<ActionResult> RmoveFromCart(int userid, int productud)
        {

            var pro = await _dataContext.ShoppingCarts.Where(o => o.UserId == userid && o.ProductId == productud).ToListAsync();
            _dataContext.ShoppingCarts.RemoveRange(pro);
           await _dataContext.SaveChangesAsync();

            return Ok();
        }

    }
}
