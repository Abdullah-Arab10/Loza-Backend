using Loza.Data;
using Loza.Data;
using Loza.Entities;
using Loza.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Org.BouncyCastle.Bcpg;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingReview : ControllerBase
    {

        private readonly DataContext _dbContext;
        private readonly AppDbContext _appDbContext;

        public RatingReview(DataContext dbContext,AppDbContext appDbContext)
        {
            this._dbContext = dbContext;
            this._appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<ActionResult> AddRate(int userId , int productId,decimal rate,[FromBody]string reviews)
        {
            var check= await _dbContext.Ratings.AnyAsync(p=>p.UserId== userId&&p.ProductId==productId);
            if (check == true) 
            {
               
               var che =  await _dbContext.Ratings.FirstAsync(p => p.UserId == userId && p.ProductId == productId);
                che.Rate = rate;
                che.Rreviews = reviews;
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            var ra = new Rating{
            UserId = userId ,
            ProductId = productId ,
            Rate = rate,
            Rreviews = reviews
            };

            
            await _dbContext.Ratings.AddAsync(ra);
            _dbContext.SaveChanges();

            return Ok();
        }


        [HttpGet]
        public async Task<ActionResult> GetReview(int userId, int productId)
        {

            var username = await _appDbContext.Users.FirstOrDefaultAsync(p => p.Id == userId);
            var review = await _dbContext.Ratings.FirstAsync(p => p.UserId == userId && p.ProductId == productId);

            var rateing = new RatingReviewDTO
            {
                Rate = review.Rate,
                Rreviews = review.Rreviews,
                UserName = username.FirstName

            };
            return Ok(rateing);

        }

        [Route("/AllReviews")]
        [HttpGet]
        public async Task<ActionResult> GetAllReviews( int productId) 
        {

          
            var ratings = _dbContext.Ratings.ToList();
            var userIds = ratings.Select(r => r.UserId).ToList();
            var users = _appDbContext.Users.Where(u => userIds.Contains(u.Id)).ToList();

            var userRatings = ratings.Select(r => new RatingReviewDTO
            {
                UserName = users.First(u => u.Id == r.UserId).FirstName,
                Rate = r.Rate,
                Rreviews = r.Rreviews
            });


            return Ok(userRatings);
        }

       /* [Route("/total")]
        [HttpGet]
        public decimal TotalRate(int productId) 
        { 
            var total = _dbContext.Ratings.Where(p=>p.ProductId==productId).Select(p=>p.Rate).ToList();

            decimal totalRate = (decimal)0;
            foreach (var user in total)
            {
                totalRate = totalRate + user;
            }
            totalRate = totalRate / total.Count;
            totalRate = Math.Round(totalRate, 2);
            return totalRate;
        
        }*/
    }
}
