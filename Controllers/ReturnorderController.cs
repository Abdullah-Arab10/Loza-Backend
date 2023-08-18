using Loza.Data;
using Loza.Entities;
using Loza.Models.DTO;
using Loza.Models.ResponseModels;
using Loza.Repository.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnorderController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly AppDbContext _appDbContext;
        private readonly IClearCart _clearCart;
        public ReturnorderController(DataContext dataContext, AppDbContext appDbContext, IClearCart clearCart)
        {
            this._dataContext = dataContext;
            this._appDbContext = appDbContext;
            this._clearCart = clearCart;
        }
        [Route("ReturnRequest")]
        [HttpPost]
        public async Task<IActionResult> ReturnRequest([FromBody]ReturnOrderRequest ROQ)
        {
            var find = await _dataContext.Orders.Where(p=>p.Order_Id==ROQ.Order_Id).FirstOrDefaultAsync();
            if (find == null)
            {

                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "No Order with this number" }
                });
            }
            if (find.Deleverd is false) 
            {
                return BadRequest(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "You Can't Return Not devliverd Order" }
                });
            }
            var checkif = await _dataContext.ReturnOrders.AnyAsync(p =>p.Order_Id==ROQ.Order_Id);
            if (checkif == true) 
            {

                return BadRequest(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "You Sent Request Already" }
                });
            }


            var RO = new ReturnOrder{Order_Id=ROQ.Order_Id,Reason = ROQ.Reason};
            await _dataContext.AddAsync(RO);
            await _dataContext.SaveChangesAsync();
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false
                
            });
        }
        [Route("GetReturnRequestes")]
        [HttpGet]
        public async Task<IActionResult> GetRetunrRequests()
        {
            var GRR = await _dataContext.ReturnOrders.Select(p=>new {Order_Id =p.Order_Id, Request_Id = p.Request_Id, Reason=p.Reason,isConfirmed=p.Confirmed,isRejected = p.isRejected }).OrderBy(p=>p.isConfirmed).ThenBy(p=>p.isRejected).ToListAsync();
            Dictionary<string,object> data = new Dictionary<string, object>
            {
                { "ReturnRequests",GRR}
            };
            return NotFound(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            });
        }

        [Route("ReturnOrderDetailsForAdmin")]
        [HttpGet]
        public async Task<ActionResult> RODFA(int requestnumber)
        {
            var ordernumber = await _dataContext.ReturnOrders.Where(p => p.Request_Id == requestnumber).FirstOrDefaultAsync();

            var getpro = await _dataContext.Orders.FirstOrDefaultAsync(p => p.Order_Id == ordernumber.Order_Id);
            if (getpro == null)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = false,
                    Errors = new ErrorModel { Message = "order not found" }
                });
            }
            var orderitmes = await _dataContext.OrderItems.Where(p => p.Order_Id == ordernumber.Order_Id).Select(p => p.Product_Id).ToListAsync();
            List<OrderItems> o = new List<OrderItems>();
            foreach (var item in orderitmes)
            {
                var quant = await _dataContext.OrderItems.Where(p => p.Product_Id == item&&p.Order_Id==ordernumber.Order_Id).Select(p => p.total_amount).FirstAsync();
                var pr = await _dataContext.Product.Where(p => p.Id == item).Select(p => p.Price).FirstAsync();
                var s = new OrderItems
                {
                    proname = await _dataContext.Product.Where(p => p.Id == item).Select(p => p.Name).FirstOrDefaultAsync(),
                    color = await _dataContext.Product.Where(p => p.Id == item).Select(p => p.Color).FirstOrDefaultAsync(),
                    quantinty = quant,
                    price = pr * quant

                };
                o.Add(s);

            }
            var pro = new OrderForAdmin
            {
                RequestNumber = requestnumber,
                OrderNumber = ordernumber.Order_Id,
                shippingadress = getpro.User_Adress,
                paymentmethod = getpro.paymethod,
                orderdate = getpro.Created_at,
                isDelivered = getpro.Deleverd,
                TotalCheck = getpro.TotalCheck,
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

        [Route("ConfirmingRetrunRequest")]
        [HttpPut]
        public async Task<IActionResult> ConfirmingRetrunRequest(ConfirmROR X)
        {

            var ror = await _dataContext.ReturnOrders.Where(r => r.Order_Id == X.roId).FirstOrDefaultAsync();
            if (X.isConfirmed is true)
            {
                if (ror.isRejected is true)
                {
                    return BadRequest(new OperationsResult
                    {
                        statusCode = 400,
                        isError = true,
                        Errors = new ErrorModel { Message = "This Return Order Request Already Rejected" }
                    });
                }
                ror.Confirmed = true;
                await _dataContext.SaveChangesAsync();
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false

                });
            }
            else
            {
                if (ror.Confirmed is true)
                {
                    return BadRequest(new OperationsResult
                    {
                        statusCode = 400,
                        isError = true,
                        Errors = new ErrorModel { Message = "This Return Order Request Already Confirmed" }
                    });
                }
                ror.isRejected = true;
                await _dataContext.SaveChangesAsync();
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false

                });
            }
        }
        }
 }

