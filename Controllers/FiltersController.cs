using Loza.Data;
using Loza.Models.DTO;
using Loza.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public FiltersController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpPost]
        [Route("Filters")]
        public async Task<IActionResult> Filters([FromForm] FiltersDTO filters)
        {
            var query = _dataContext.Product.AsQueryable();

            // Apply filtering based on the user's input
            if (filters.MinPrice > 0)
                query = query.Where(p => p.Price >= filters.MinPrice);

            if (filters.MaxPrice > 0)
                query = query.Where(p => p.Price <= filters.MaxPrice);

            if (filters.Categories.HasValue)
                query = query.Where(p => p.Category == filters.Categories.Value);

            if (!string.IsNullOrEmpty(filters.Color))
                query = query.Where(p => p.Color == filters.Color);

            if (filters.ColorNo.HasValue)
                query = query.Where(p => p.ColorNo == filters.ColorNo.Value);

            var products = await query.ToListAsync();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "Products", products }
            };

            if (products.Count == 0)
            {
                return NotFound(new OperationsResult
                {
                    statusCode = StatusCodes.Status404NotFound,
                    isError = true,
                    Errors = new ErrorModel { Message = "No products found within the specified price range." }
                });
            }
            return Ok(new OperationsResult
            {
                statusCode = StatusCodes.Status200OK,
                isError = false,
                Data = data
            });
        }
    }
}
