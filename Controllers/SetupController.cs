using Loza.Data;
using Loza.Entities;
using Loza.Models;
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
            return Ok(roles);
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
                    return Ok("Role has been added successfully");
                }
                response.Errors.Add(new ErrorModel { Message = "Role has not been added" });
                return BadRequest(response);

            }
            response.Errors.Add(new ErrorModel { Message = "Role already exist" });
            return BadRequest(response);
        }
        [HttpPost("Add_User_to_Role")]
        public async Task<IActionResult> AddUserToRole(string Email, string RoleName)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            user.SecurityStamp = Guid.NewGuid().ToString();
            var response = new OperationsResult();
            if (user == null)
            {
                response.Errors.Add(new ErrorModel { Message = "User not found" });
                return BadRequest(response);
            }
            var role = await _roleManager.RoleExistsAsync(RoleName);
            if (!role)
            {
                return Ok("Role has been added successfully");
            }
            var result = await _userManager.AddToRoleAsync(user, RoleName);
            if (result.Succeeded)
            {
                return Ok($"{user} has been added to the Role {RoleName}");
            }
            response.Errors.Add(new ErrorModel { Message = $"{user} han not been added to the Role{RoleName}" });
            return BadRequest(response);
        }
        [HttpPost("Remove_User_From_Role")]
        public async Task<IActionResult> RemoveUserFromRole(string Email, string RoleName)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            user.SecurityStamp = Guid.NewGuid().ToString();
            var response = new OperationsResult();
            if (user == null)
            {
                response.Errors.Add(new ErrorModel { Message = "User not found" });
                return BadRequest(response);
            }
            var role = await _roleManager.RoleExistsAsync(RoleName);
            if (!role)
            {
                return BadRequest($"{RoleName} does not exist");
            }
            var result = await _userManager.RemoveFromRoleAsync(user, RoleName);
            if (result.Succeeded)
            {
                return Ok($"{user} has been removed from the Role{RoleName}");
            }
            response.Errors.Add(new ErrorModel { Message = $"{user} han not been removed from the Role{RoleName}" });
            return BadRequest(response);
        }
    }
}
