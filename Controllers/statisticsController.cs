using Loza.Data;
using Loza.Entities;
using Loza.Models.DTO;
using Loza.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class statisticsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly DataContext _dataContext;

        public statisticsController(AppDbContext appDbContext, DataContext dataContext)
        {
            _appDbContext = appDbContext;
            _dataContext = dataContext;
        }
        [HttpGet]
        [Route("Statistics")]
        public async Task<ActionResult<OperationsResult>> GetStatistics()
        {
            var userCount = await _appDbContext.Users.CountAsync();
            var productCount = await _dataContext.Product.CountAsync();
            var orderCount = await _dataContext.Orders.CountAsync();
            var orderReturnedCount = await _dataContext.ReturnOrders.Where(r => r.Confirmed == true).CountAsync();
            var totalChecks = await _dataContext.Orders.SumAsync(b => b.TotalCheck);

            var topSoldItems = await _dataContext.OrderItems
                .GroupBy(sale => sale.Product_Id)
                .OrderByDescending(group => group.Sum(sale => sale.total_amount))
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalSales = group.Sum(sale => sale.total_amount)
                })
                .Take(5)
                .ToListAsync();

            var productIds = topSoldItems.Select(item => item.ProductId).ToList();

            var topSoldProducts = await _dataContext.Product
                .Where(product => productIds.Contains(product.Id))
                .Select(product => new Most5ProductsSold
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Category = product.Category,
                    Color = product.Color,
                    ColorNo = product.ColorNo,
                    Quantity = product.Quantity,
                    ProductDimensions = product.ProductDimensions,
                    ProductImage = product.ProductImage
                })
                .ToListAsync();

            foreach (var product in topSoldProducts)
            {
                var matchingItem = topSoldItems.FirstOrDefault(item => item.ProductId == product.Id);
                if (matchingItem != null)
                {
                    product.TotalSales = matchingItem.TotalSales;
                }
            }

            Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { "userCount", userCount },
                    { "productCount", productCount },
                    { "orderCount", orderCount },
                    { "orderReturnedCount", orderReturnedCount },
                    { "TotalCheck", totalChecks },
                    { "topSold", topSoldProducts }
                };

            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            });
        }

    }
}
