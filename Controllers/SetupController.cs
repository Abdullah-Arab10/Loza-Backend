using Loza.Data;
using Loza.Entities;
using Loza.Migrations;
using Loza.Models;
using Loza.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace Loza.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly AppDbContext _context;

        public SetupController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "roles", roles }
            };
            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError = false,
                Data= data
            });
        }
        [HttpPost("Create_Role")]
        public async Task<IActionResult> CreateRole(string role)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            var response = new OperationsResult();
            if (!roleExist)
            {
                var Role = new IdentityRole<int>(role);
                var roleResult = await _roleManager.CreateAsync(Role);
                if (roleResult.Succeeded)
                {
                    return Ok(new OperationsResult
                    {
                        statusCode = 200,
                        isError = false
                    });
                }
                return BadRequest(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "Role has not been added" }
                });

            }
            return BadRequest(new OperationsResult
            {
                statusCode = 400,
                isError = true,
                Errors = new ErrorModel { Message = "Role already exist" }
            });
        }
        [HttpPost("Add_User_to_Role")]
        public async Task<IActionResult> AddUserToRole(string Email, string RoleName)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            user.SecurityStamp = Guid.NewGuid().ToString();
            if (user == null)
            {
                return BadRequest(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "User not found" }
                });
            }
            var role = await _roleManager.RoleExistsAsync(RoleName);
            if (!role)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false
                });
            }
            var result = await _userManager.AddToRoleAsync(user, RoleName);
            if (result.Succeeded)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false
                });
            }
            return BadRequest(new OperationsResult
            {
                statusCode = 400,
                isError = true,
                Errors = new ErrorModel { Message = $"{user} han not been added to the Role{RoleName}" }
            });
        }
        [HttpPost("Remove_User_From_Role")]
        public async Task<IActionResult> RemoveUserFromRole(string Email, string RoleName)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            user.SecurityStamp = Guid.NewGuid().ToString();
            if (user == null)
            {
                return BadRequest(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "User not found" }
                });
            }
            var role = await _roleManager.RoleExistsAsync(RoleName);
            if (!role)
            {
                return BadRequest(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = $"{RoleName} does not exist" }
                });
            }
            var result = await _userManager.RemoveFromRoleAsync(user, RoleName);
            if (result.Succeeded)
            {
                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false
                });
            }
            return BadRequest(new OperationsResult
            {
                statusCode = 400,
                isError = true,
                Errors = new ErrorModel { Message = $"{user} han not been removed from the Role{RoleName}" }
            });
        }
    }
}
