using Loza.Data;
using Loza.Entities;
using Loza.Migrations;

using Loza.Models.DTO;
using Loza.Models.ResponseModels;
using Loza.Repository.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MimeKit.Cryptography;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [Route("CreateOrder")]
        [HttpPost]
        public async Task<ActionResult> Createorder([FromBody] OrderQuery request/*int userid, int paymentmethod, int addressid,decimal total = 80*/)
        {
            int userid = request.userid;
            int paymentmethod = request.paymentmethod;
            int addressid = request.addressid;
            decimal total = request.total;
            if (paymentmethod == 1)
            {
                var check = await _appDbContext.Users.Where(p => p.Id == userid).FirstAsync();
                if (check.Wallet < total)
                {


                    return Ok(new OperationsResult
                    {
                        statusCode = 400,
                        isError = true,
                        Errors = new ErrorModel { Message = "No Enough Money" }
                    });

                }
                else
                {
                    check.Wallet -= total;
                    await _appDbContext.SaveChangesAsync();

                    var createorder = new Order
                    {
                        User_Id = userid,
                        TotalCheck = total,
                        User_Adress = await _dataContext.Addresses.Where(p => p.Id == addressid).Select(p => p.Location).FirstAsync(),
                        paymethod = paymentmethod,
                        AdressId = addressid,
                       
                    };
                    await _dataContext.Orders.AddAsync(createorder);
                    await _dataContext.SaveChangesAsync();
                    int orderid = createorder.Order_Id;



                    var items = await _dataContext.ShoppingCarts.Where(p => p.UserId == userid).Select(p => new { p.price, p.ProductId, p.Quant }).ToListAsync();
                    foreach (var item in items)
                    {
                        var prodquant = await _dataContext.Product.Where(p => p.Id == item.ProductId).FirstAsync();
                        prodquant.Quantity -= item.Quant;
                        var orderitem = new OrderItem
                        {
                            Order_Id = orderid,
                            Product_Id = (int)item.ProductId,
                            total_amount = item.Quant

                        };
                        await _dataContext.OrderItems.AddAsync(orderitem);
                        await _dataContext.SaveChangesAsync();
                    }
                    await _clearCart.clearCart(userid);
                    return Ok(new OperationsResult
                    {
                        statusCode = 200,
                        isError = false,
                        Errors = new ErrorModel { Message = "Order Confirmed" }
                    });
                }

            }

            else
            {
                var createorder1 = new Order
                {
                    User_Id = userid,
                    User_Adress = await _dataContext.Addresses.Where(p => p.Id == addressid).Select(p => p.Location).FirstAsync(),
                    paymethod = paymentmethod,
                    AdressId = addressid,
                    TotalCheck = total
                };
                await _dataContext.Orders.AddAsync(createorder1);
                await _dataContext.SaveChangesAsync();
                int orderid1 = createorder1.Order_Id;



                var items1 = await _dataContext.ShoppingCarts.Where(p => p.UserId == userid).Select(p => new { p.price, p.ProductId, p.Quant }).ToListAsync();
                foreach (var item1 in items1)
                {
                    var prodquant = await _dataContext.Product.Where(p => p.Id == item1.ProductId).FirstAsync();
                    prodquant.Quantity -= item1.Quant;
                    var orderitem1 = new OrderItem
                    {
                        Order_Id = orderid1,
                        Product_Id = (int)item1.ProductId,
                        total_amount = item1.Quant
                    };
                    await _dataContext.OrderItems.AddAsync(orderitem1);
                    await _dataContext.SaveChangesAsync();
                }
                await _clearCart.clearCart(userid);
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false,
                    Errors = new ErrorModel { Message = "Order Confirmed" }
                });
            }
        }

        [Route("GetOrderByNumber")]
        [HttpGet]
        public async Task<ActionResult> GetOrderByNumber( int orderid) 
        {

            var getpro = await _dataContext.Orders.FirstOrDefaultAsync(p => p.Order_Id == orderid);
            if (getpro == null)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = false,
                    Errors =  new ErrorModel { Message ="order not found"} 
                });
            }
            var orderitmes = await _dataContext.OrderItems.Where(p=>p.Order_Id==orderid).Select(p=>p.Product_Id).ToListAsync();
            List<OrderItems> o = new List<OrderItems>();
          foreach(var item in orderitmes)
            {
                var qu = await _dataContext.OrderItems.Where(p => p.Product_Id == item&&p.Order_Id==orderid).Select(p => p.total_amount).FirstAsync();
                var pr = await _dataContext.Product.Where(p => p.Id == item).Select(p => p.Price).FirstAsync();
                var s = new OrderItems
                { proname =await _dataContext.Product.Where(p=>p.Id==item).Select(p=>p.Name).FirstOrDefaultAsync(),
                  color= await _dataContext.Product.Where(p => p.Id == item).Select(p => p.Color).FirstOrDefaultAsync(),
                  quantinty= qu,
                  price= pr * qu,
                  
                };
                o.Add(s);
                
            }
            var pro = new GetOrder
            {
                number = orderid,
                shippingadress = getpro.User_Adress,
                paymentmethod = getpro.paymethod,
                orderdate = getpro.Created_at,
                isDelivered = getpro.Deleverd,
                TotalCheck = getpro.TotalCheck,
                products = o
            };
            Dictionary<string, object> data = new Dictionary<string, object> { { "OrderByOrderNumber", pro } };

            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            });
        }

        [Route("GetUserOrders/{userid}")]
        [HttpGet]
        public async Task<ActionResult> GetUserOrders(int userid) {
            var founduser = await _appDbContext.users.AnyAsync(p=>p.Id==userid);
            if (founduser is false)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message ="No UserFound" }
                    
                });
            }
            
            var getorders = await _dataContext.Orders.Where(p => p.User_Id == userid).OrderByDescending(s => s.Created_at)
           
                           .ToListAsync();
            if (getorders.Count == 0)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel{ Message =  "no orders yet"}
                });
            }
            var getAllOrders = new List<GetAllOrders>();
            foreach (var order in getorders) 
            {
                var c = new GetAllOrders { orderNumber = order.Order_Id, useraddress = order.User_Adress,isDelivered=order.Deleverd,Orderdate=order.Created_at };
                getAllOrders.Add(c);
            }
            Dictionary<string, object> data = new Dictionary<string, object> { { "AllOrders", getAllOrders} };
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            });

        }

        [Route("api/Order/GetAllOrders")]
        [HttpGet]
        public async Task<ActionResult> GetAllOrders()
        {
            
            var orders = await _dataContext.Orders
                         .Select( s => new GetAllOrders
                         {
                            orderNumber = s.Order_Id,
                            useraddress=s.User_Adress,
                            isDelivered =s.Deleverd,
                            Orderdate=s.Created_at,
                         })
                         .ToListAsync();
            Dictionary<string, object> data1 = new Dictionary<string, object>
            {
                {"AllOrders",orders}

            };
            var response = new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data1
            };

            return Ok(response);
        }
    }
  }
    






        
       
 
