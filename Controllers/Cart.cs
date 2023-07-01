using LozaApi.Data;
using LozaApi.Models;
using LozaApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LozaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cart : ControllerBase
    {
     
     private readonly DataContext _dataContext;
        public Cart(DataContext dataContext)
        {
            _dataContext = dataContext;

        }


        [HttpPost]
        public async Task<ActionResult> AddProductToCart(int userId , int prodctId, int quan ) {



            var find = await _dataContext.ShoppingCarts.FirstOrDefaultAsync(p=>p.ProductId==prodctId&&p.UserId == userId);
            
            if (find != null)
            {
            find.Quant=find.Quant + quan;
                await _dataContext.SaveChangesAsync();
                return Ok();
            }
            var pro =await _dataContext.Product.FirstOrDefaultAsync(P =>P.Id==prodctId);
            var cart = new ShoppingCart
            {
               ProductId = prodctId,
               ProductName = pro.Name,
               price = pro.Price,
               UserId = userId,
               Quant = quan

            };
            await _dataContext.ShoppingCarts.AddAsync(cart);
            await _dataContext.SaveChangesAsync();

            return Ok();

        }

        [HttpGet]
        public async Task<ActionResult> GetUserCart(int userId) 
        {
            var car = await _dataContext.ShoppingCarts.Where(p => p.UserId == userId)
            .Select(s =>new CartDTO
            {
                price = s.Total,
                Qunatity=s.Quant,
                ProductName=s.ProductName

            })
            .ToListAsync();
        
            return Ok(car);
        
        
        }

        [HttpDelete]
        public async Task<ActionResult> RmoveFromCart(int userid,int productud)
        {

            var pro = await _dataContext.ShoppingCarts.Where(o => o.UserId == userid&& o.ProductId == productud).ToListAsync();
            _dataContext.ShoppingCarts.RemoveRange(pro);
            _dataContext.SaveChanges();

            return Ok();
        }




    }
}
