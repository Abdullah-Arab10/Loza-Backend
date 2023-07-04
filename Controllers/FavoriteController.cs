using Loza.Data;
using Loza.Entities;
using Loza.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Loza.Models.ResponseModels;
using Loza.Migrations;
using Newtonsoft.Json.Linq;
using Loza.Migrations.Data;


namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly AppDbContext _context;

        public FavoriteController(DataContext dataContext, AppDbContext context)
        {
            _dataContext = dataContext;
            _context = context;
        }
        [HttpPost]
        [Route("MakeFavorite/{userId}/{productId}")]
        public async Task<ActionResult> makeFavorite(int userId, int productId)
        {
            var favorite = new favorite { UserId = userId, ProductId = productId };
            var exist = await _dataContext.favorites.AnyAsync(p => p.ProductId == productId && p.UserId == userId);
            if (exist == true)
            {
                var existingfavorite = await _dataContext.favorites.Where(f => f.UserId == userId && f.ProductId == productId).FirstOrDefaultAsync();
                _dataContext.favorites.Remove(existingfavorite);
                await _dataContext.SaveChangesAsync();
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false
                });
            }

            await _dataContext.favorites.AddAsync(favorite);
            await _dataContext.SaveChangesAsync();

            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false
            });
        }
        [HttpGet]
        [Route("GetAllFavorites/{UserId}")]
        public async Task<ActionResult> GetAllFavorites(int UserId)
        {
            var favoriteProducts = await _dataContext.favorites
                .Where(f => f.UserId == UserId)
                .Select(f => f.ProductId)
                .ToListAsync();

            var products = await _dataContext.Product.ToListAsync();
            var pro = await _dataContext.Product.Select(s => new ProductsDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                Category = s.Category,
                Color = s.Color,
                Quantity = s.Quantity,
                ProductImage = s.ProductImage,
                IsFavorite = _dataContext.favorites.Any(p => p.ProductId == s.Id && p.UserId == UserId)
            }).Where(p => p.IsFavorite).ToListAsync();
           
            Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "favoriteList", pro }
                    };
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            });
        }
        /*[HttpDelete]
        [Route("RemoveFromFavorites/{UserId}/{ProductId}")]
        public async Task<ActionResult> deleteFavorite(int UserId, int ProductId)
        {
            var favorite = await _dataContext.favorites.Where(f => f.UserId == UserId && f.ProductId == ProductId).FirstOrDefaultAsync();
            if (favorite == null)
            {
                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError = true
                });
            }
            _dataContext.favorites.Remove(favorite);
            await _dataContext.SaveChangesAsync();
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false
            });
        }*/
    }
}
