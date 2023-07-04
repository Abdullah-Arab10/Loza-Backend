using Loza.Data;
using Loza.Entities;
using Loza.Repository.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly DataContext _dataContext;
        private readonly AppDbContext _appDbContext;
        private readonly IClearCart _clearCart;
        public OrderController(DataContext dataContext, AppDbContext appDbContext, IClearCart clearCart)
        {

            this._dataContext = dataContext;
            this._appDbContext = appDbContext;
            this._clearCart = clearCart;
        }

        [HttpPost]
        public async Task<ActionResult> Createorder(int userid, int paymentmethod, int total = 80)
        {

            if (paymentmethod == 1)
            {
              var check = await _appDbContext.Users.Where(p => p.Id == userid).FirstAsync();
                if (check.Wallet < total)
                {
                    return Ok("there is not enogh money in wallet");
                }
                else
                {
                   check.Wallet -= total;
                   await _appDbContext.SaveChangesAsync();

                    var createorder = new Order
                    {
                        User_Id = userid,
                        User_Adress = await _appDbContext.Users.Where(p => p.Id == userid).Select(p => p.Address).FirstAsync(),
                        paymethod = paymentmethod
                    };
                    await _dataContext.Orders.AddAsync(createorder);
                    await _dataContext.SaveChangesAsync();
                    int orderid = createorder.Order_Id;



                    var items = await _dataContext.ShoppingCarts.Where(p => p.UserId == userid).Select(p => new { p.price, p.ProductId }).ToListAsync();

                    foreach (var item in items)
                    {
                        var orderitem = new OrderItem
                        {
                            Order_Id = orderid,
                            Product_Id = (int)item.ProductId,
                            total_check = item.price
 
                        };
                        await _dataContext.OrderItems.AddAsync(orderitem);
                        await _dataContext.SaveChangesAsync();
                    }
                    await _clearCart.clearCart(userid);
                    return Ok();

                }

            }

            else
            {
                var createorder1 = new Order
                {
                    User_Id = userid,
                    User_Adress = await _appDbContext.Users.Where(p => p.Id == userid).Select(p => p.Address).FirstAsync(),
                    paymethod=paymentmethod
                };
                await _dataContext.Orders.AddAsync(createorder1);
                await _dataContext.SaveChangesAsync();
                int orderid1 = createorder1.Order_Id;



                var items1 = await _dataContext.ShoppingCarts.Where(p => p.UserId == userid).Select(p => new { p.price, p.ProductId }).ToListAsync();

                foreach (var item in items1)
                {
                    var orderitem1 = new OrderItem
                    {
                        Order_Id = orderid1,
                        Product_Id = (int)item.ProductId,
                        total_check = item.price
                        

                    };
                    await _dataContext.OrderItems.AddAsync(orderitem1);
                    await _dataContext.SaveChangesAsync();

                }


                await _clearCart.clearCart(userid);



                return Ok();
            }
        }
    
    }
    
}
        
       
 
