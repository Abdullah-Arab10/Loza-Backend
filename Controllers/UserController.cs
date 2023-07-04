using Loza.Data;
using Loza.Entities;
using Loza.Migrations;
//using Loza.Migrations;
using Loza.Models;
using Loza.Models.ResponseModels;
using Loza.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loza.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("Get_all_Users")]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            var user = _context.Users.Select(u => new UserGetById
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Wallet= u.Wallet,
                Address = u.Address
            }).ToList();
            
            if (user == null || user.Count == 0)
            {
                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "No users to list" }
                });
            }
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "users", user }
            };

            return Ok(new OperationsResult
            {
                statusCode = StatusCodes.Status200OK,
                isError = false,
                Data = data
            });
        }
        [HttpGet("Get_by_Id/{Id}")]
        public ActionResult<User> GetById(int Id)
        {
            var user = _context.users.Where(u => u.Id == Id).Select(u => new UserGetById
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Wallet = u.Wallet,
                Address = u.Address
            }).ToList();

            if (user.Count == 0)
            {
                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "User not found" } 
                });
            }
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "user", user }
            };
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data = data
            });
            
        }
        [HttpGet("Search/{search}")]
        public ActionResult<IEnumerable<User>> Search(string search)
        {
            var users = _context.Users
                .Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search))
                .Select(u => new UserGetById
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    DateOfBirth = u.DateOfBirth,
                    Wallet= u.Wallet,
                    Address = u.Address
                }).ToList();

            var response = new OperationsResult();

            if (users.Count == 0)
            {
                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "User not found" }
                });
            }
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "users", users }
            };
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data =  data 
            });
        }
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "Not Found" }
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            //return Ok($"{user.FirstName}'s account has been deleted successfully");
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false
            });
        }
        [HttpGet("Add_Money_to_user/{Id}")]
        public async Task<IActionResult> AddMoneyToUser(int Id, decimal cash)
        {
            var user = _context.Users.Find(Id);
            if (user == null)
            {
                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors = new ErrorModel { Message = "User not found" }
                });
            }
            user.Wallet += cash;
            await _context.SaveChangesAsync();
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false
            });
        }
    }
}
