using Loza.Data;
using Loza.Data;
using Loza.Entities;
using Loza.Migrations;
using Loza.Models.DTO;
using Loza.Models.ResponseModels;
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
        [Route("AddRate")]
        [HttpPost]
        public async Task<ActionResult> AddRate([FromBody]AddRatingDTO request)
        {
              var userId = request.userId;
              var productId=request.productId;
              decimal rate = request.rating;
              string reviews = request.reviews;
              var error = new List<string>();
            var checkuserid = await _appDbContext.users.AnyAsync(p=>p.Id==userId);
            var checkproductid = await _dbContext.Product.AnyAsync(p=>p.Id==productId);
            if (checkuserid is false || checkproductid is false) 
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "No Product or User Existe" }
                });
            }
         

            var check = await _dbContext.Ratings.AnyAsync(p=>p.UserId== userId&&p.ProductId==productId);
              if (check == true) 
              {

                  var che =  await _dbContext.Ratings.FirstAsync(p => p.UserId == userId && p.ProductId == productId);
                  che.Rate = rate;
                  che.Rreviews = reviews;
                  await _dbContext.SaveChangesAsync();
                  return Ok(new OperationsResult {isError=false,statusCode=200 } );
              }
              var ra = new Rating{
              UserId = userId ,
              ProductId = productId ,
              Rate = rate,
              Rreviews = reviews
              };


              await _dbContext.Ratings.AddAsync(ra);
              _dbContext.SaveChanges();

              return Ok(new OperationsResult { isError = false, statusCode = 200 });
           
        }


        [HttpGet]
        public async Task<ActionResult> GetReview(int userId, int productId)
        {
            var checkuserid = await _appDbContext.users.AnyAsync(p => p.Id == userId);
            var checkproductid = await _dbContext.Product.AnyAsync(p => p.Id == productId);

            if (checkuserid is false || checkproductid is false)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "No Product or User Existe" }
                });
            }

            var username = await _appDbContext.Users.FirstOrDefaultAsync(p => p.Id == userId);
            var review = await _dbContext.Ratings.FirstAsync(p => p.UserId == userId && p.ProductId == productId);

            var rateing = new RatingReviewDTO
            {
                Rate = review.Rate,
                Rreviews = review.Rreviews,
                UserName = username.FirstName

            };

            Dictionary<string, object> data = new Dictionary<string, object> { {"Review",rateing } };
            return Ok(new OperationsResult { isError = false, statusCode = 200 , Data=data});

        }

        [Route("/AllReviews/{productId}")]
        [HttpGet]
        public async Task<ActionResult> GetAllReviews(int productId) 
        {
            
            var checkproductid = await _dbContext.Product.AnyAsync(p => p.Id == productId);

            if ( checkproductid is false)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "No Product Existe" }
                });
            }


            var ratings = _dbContext.Ratings.Where(p=>p.ProductId==productId).ToList();
            var userIds = ratings.Select(r => r.UserId).ToList();
            var users = _appDbContext.Users.Where(u => userIds.Contains(u.Id)).ToList();

            var userRatings = ratings.Select(r => new RatingReviewDTO
            {
                UserName = users.First(u => u.Id == r.UserId).FirstName,
                Rate = r.Rate,
                Rreviews = r.Rreviews
            });
            Dictionary<string,object> data = new Dictionary<string, object>{ { "allReveiws",userRatings}};

            return Ok(new OperationsResult { isError = false, statusCode = 200 ,Data= data});
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
