using Loza.Data;
using Loza.Models.DTO;
using Loza.Models.ResponseModels;
using Loza.Repository.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryManController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly AppDbContext _appDbContext;
        private readonly IClearCart _clearCart;
        public DeliveryManController(DataContext dataContext, AppDbContext appDbContext)
        {
            this._dataContext = dataContext;
            this._appDbContext = appDbContext;
        }
        [Route("NotDelIveredOrders")]
        [HttpGet]
        public async Task<ActionResult> NotDeliveredOrders(int page = 1)
        {
            var pageResult = 10f;
            var notdelivered = await _dataContext.Orders.Where(s => s.Deleverd == false)
                .OrderByDescending(p => p.Created_at)
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult).Select(s => new GetAllOrders
                {
                    orderNumber = s.Order_Id,
                    useraddress = s.User_Adress,
                    isDelivered = s.Deleverd,
                    Orderdate = s.Created_at,
                }).ToListAsync();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"NotDeliveredOrderes",notdelivered }
            };

            var response = new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            };

            return Ok(response);

        }


        [Route("GetOrderDetailsForDeliveryMan")]
        [HttpGet]
        public async Task<ActionResult> ODFD(int ordernumber)
        {

            var getpro = await _dataContext.Orders.FirstOrDefaultAsync(p => p.Order_Id == ordernumber);
            if (getpro == null)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = false,
                    Errors = new ErrorModel { Message = "order not found" }
                });
            }
            var orderitmes = await _dataContext.OrderItems.Where(p => p.Order_Id == ordernumber).Select(p => p.Product_Id).ToListAsync();
            List<OrderItems> o = new List<OrderItems>();
            foreach (var item in orderitmes)
            {
                var s = new OrderItems
                {
                    proname = await _dataContext.Product.Where(p => p.Id == item).Select(p => p.Name).FirstOrDefaultAsync(),
                    color = await _dataContext.Product.Where(p => p.Id == item).Select(p => p.Color).FirstOrDefaultAsync(),
                    quantinty = await _dataContext.OrderItems.Where(p => p.Id == item).Select(p => p.total_amount).FirstOrDefaultAsync(),
                    price = await _dataContext.Product.Where(p => p.Id == item).Select(p => p.Price).FirstOrDefaultAsync(),

                };
                o.Add(s);

            }
            var pro = new OrderForDeliveryMan
            {
                number = ordernumber,
                shippingadress = getpro.User_Adress,
                paymentmethod = getpro.paymethod,
                orderdate = getpro.Created_at,
                isDelivered = getpro.Deleverd,
                phonenumber = await _appDbContext.Users.Where(u => u.Id == getpro.User_Id).Select(p => p.PhoneNumber).FirstAsync(),
                username = await _appDbContext.Users.Where(u => u.Id == getpro.User_Id).Select(p => p.FirstName + " " + p.LastName).FirstOrDefaultAsync(),
                products = o
            };


            Dictionary<string, object> data = new Dictionary<string, object> { { "OrderDetails", pro } };

            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            });
        }

        [Route("ConfirmOrder")]
        [HttpPut]
        public async Task<ActionResult> ConfirmOrder(int ordernumber)
        {
            var order = await _dataContext.Orders.Where(p => p.Order_Id == ordernumber).FirstOrDefaultAsync();
            if (order == null)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = false,
                    Errors = new ErrorModel { Message = "order not found" }
                });
            }

            order.Deleverd = true;
            await _dataContext.SaveChangesAsync();

            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false
                
            });

        }
    }
}
