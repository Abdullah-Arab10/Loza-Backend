using Loza.Data;
using Loza.Entities;
using Loza.Models;
using Loza.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Loza.Controllers
{
    [Authorize]
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
            var users = _context.Users.Select(u => new UserGetById
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth
            }).ToList();

            if (users == null || users.Count == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "No users to list " });
            }

            return Ok(users);
        }
        [HttpGet("Get_by_Id/{Id}")]
        public ActionResult<User> GetById(int Id)
        {
            //var user = _context.users.FirstOrDefault(u => u.Id == Id);
            var user = _context.users.Where(u => u.Id == Id).Select(u => new UserGetById 
            {
                Id= u.Id,
                FirstName= u.FirstName,
                LastName= u.LastName,
                PhoneNumber= u.PhoneNumber,
                DateOfBirth= u.DateOfBirth
            });

            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User does not exist" });
            }
            return Ok(user);
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
                    PhoneNumber = u.PhoneNumber,
                    DateOfBirth = u.DateOfBirth
                }).ToList();

            if (users == null || users.Count() == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "No users found with that name " });
            }

            return Ok(users);
        }
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var user = await _context.Users.FindAsync(Id);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User not found " });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully");
        }
    }
}
