using Loza.Data;
using Loza.Entities;
//using Loza.Migrations;
using Loza.Models;
using Loza.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loza.Controllers
{
    [Authorize(Roles = "Admin")]
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
                Address = u.Address
            }).ToList();
            var response = new OperationsResult();

            if (user == null || user.Count == 0)
            {
                response.Errors.Add(new ErrorModel { Message = "No users to list" });
                return NotFound(response);
            }

            response.Data.AddRange(user);
            return Ok(response);
        }
        [HttpGet("Get_by_Id/{Id}")]
        public ActionResult<User> GetById(int Id)
        {
            //var user = _context.users.FirstOrDefault(u => u.Id == Id);
            var user = _context.users.Where(u => u.Id == Id).Select(u => new UserGetById
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth
            }).ToList();

            var response = new OperationsResult();

            if (user.Count == 0)
            {
                response.Errors.Add(new ErrorModel { Message = "User not found" });
                return NotFound(response);
            }

            response.Data.AddRange(user);
            return Ok(response);
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
                    Address = u.Address
                }).ToList();

            var response = new OperationsResult();

            if (users.Count == 0)
            {
                response.Errors.Add(new ErrorModel { Message = "User not found" });
                return NotFound(response);
            }

            response.Data.AddRange(users);
            return Ok(response);
        }
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var user = await _context.Users.FindAsync(Id);
            var response = new OperationsResult();
            if (user == null)
            {
                response.Errors.Add(new ErrorModel { Message = "Not Found" });
                return NotFound(response);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"{user.FirstName}'s account has been deleted successfully");
        }
        [HttpGet("Add_Money_to_user/{Id}")]
        public async Task<IActionResult> AddMoneyToUser(int Id, int cash)
        {
            var user = _context.Users.Find(Id);
            var response = new OperationsResult();
            if (user == null)
            {
                response.Errors.Add(new ErrorModel { Message = "User not found" });
                return NotFound(response);
            }
            user.Wallet += cash;
            await _context.SaveChangesAsync();
            return Ok($"Cash has been sent to {user.Email} wallet ");
        }
    }
}
