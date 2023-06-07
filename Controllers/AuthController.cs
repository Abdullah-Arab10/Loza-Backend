using Loza.Data;
using Loza.Entities;
//using Loza.Migrations;
using Loza.Models;
using Loza.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromForm] UserRegistrationRequestDto requestDto)
        {
            //validate the incoming request 
            if (ModelState.IsValid)
            {
                //we need to check if the email already exist
                var user_exist = await _userManager.FindByEmailAsync(requestDto.email);
                var response = new OperationsResult();
                if (user_exist != null)
                {
                    response.Errors.Add(new ErrorModel
                    {
                        Message = "Email already exist"
                    });
                    return BadRequest(response);
                }
                //create a user 
                var new_user = new User()
                {
                    Email = requestDto.email,
                    UserName = requestDto.email,
                    FirstName = requestDto.FirstName,
                    LastName = requestDto.LastName,
                    PhoneNumber = requestDto.phoneNumber,
                    Address = requestDto.Address,
                    DateOfBirth = requestDto.DateOfBirth
                };
                var is_created = await _userManager.CreateAsync(new_user, requestDto.password);


                if (is_created.Succeeded)
                {
                    var result = await _userManager.AddToRoleAsync(new_user, "User");
                    if (result.Succeeded)
                    {
                        return Ok("User registered successfully");
                    }
                    return BadRequest();
                }

                var errors = new List<string>();
                foreach (var error in is_created.Errors)
                {
                    errors.Add(error.Description);
                }
                response.Errors.Add(new ErrorModel { Message = string.Join(",", errors) });
                return BadRequest(response);
            }

            return BadRequest();
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserLoginRequestDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                //check if the user exist
                var user = await _userManager.FindByEmailAsync(loginRequest.Email);
                var response = new OperationsResult();
                if (user == null)
                {
                    response.Errors.Add(new ErrorModel
                    {
                        Message = "Wrong Email"
                    });
                    return BadRequest(response);
                }
                var isCorrect = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
                if (!isCorrect)
                {
                    response.Errors.Add(new ErrorModel { Message = "Wrong Password" });
                    return Unauthorized(response);
                }
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim("Id",user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub,user.FirstName),
                    new Claim(JwtRegisteredClaimNames.Sub,user.LastName),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value));

                var token = new JwtSecurityToken(
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                //var iwtToken = GenerateJwtToken(existing_user);
                response.Data.Add(jwtToken);
                return Ok(response);

            }
            return StatusCode(StatusCodes.Status500InternalServerError);


        }

        //[Authorize]
        [Route("Change_Password")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var response = new OperationsResult();
            if (user == null)
            {
                response.Errors.Add(new ErrorModel { Message = "Email does not exist" });
                return NotFound(response);
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                response.Errors.Add(new ErrorModel { Message = string.Join(",", errors) });
                return BadRequest(response);
            }
            return Ok("Password changed successfuly");
        }

        [Route("Update/{Id}")]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UserUpdateRequestDto model, int Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            var response = new OperationsResult();
            if (user == null)
            {
                response.Errors.Add(new ErrorModel { Message = "User not Found " });
                return BadRequest(response);
            }
            user.SecurityStamp = Guid.NewGuid().ToString();
            bool hasChanges = false;

            if (model.FirstName != null && model.FirstName != user.FirstName)
            {
                user.FirstName = model.FirstName;
                hasChanges = true;
            }

            if (model.LastName != null && model.LastName != user.LastName)
            {
                user.LastName = model.LastName;
                hasChanges = true;
            }

            if (model.Email != null && model.Email != user.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                hasChanges = true;
            }

            if (model.PhoneNumber != null && model.PhoneNumber != user.PhoneNumber)
            {
                user.PhoneNumber = model.PhoneNumber;
                hasChanges = true;
            }

            if (model.Address != null && model.Address != user.Address)
            {
                user.Address = model.Address;
                hasChanges = true;
            }

            if (model.DateOfBirth.HasValue && model.DateOfBirth != user.DateOfBirth)
            {
                user.DateOfBirth = model.DateOfBirth.Value;
                hasChanges = true;
            }

            if (hasChanges)
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _context.SaveChanges();
                    return Ok("Edited successfully");
                }

                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                response.Errors.Add(new ErrorModel { Message = string.Join(",", errors) });
                return BadRequest(response);
            }
            else
            {
                return Ok("No changes were made to your profile.");
            }

        }
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var user = await _context.users.FindAsync(Id);
            var response = new OperationsResult();
            if (user == null)
            {
                response.Errors.Add(new ErrorModel { Message = "User does not exists" });
                return NotFound(response);
            }
            _context.Remove(user);
            _context.SaveChanges();

            return Ok("Account deleted successfuly");
        }

    }

}
/*private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            //Token descriptor
            var tokendescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id",user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub,user.FirstName),
                    new Claim(JwtRegisteredClaimNames.Sub,user.LastName),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString())
                }),
                Expires = DateTime.Now.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = jwtTokenHandler.CreateToken(tokendescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
 */
/* private async Task<List<Claim>> GetAllValidClaims(User user)
        {
            var options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim("Id",user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.FirstName),
                new Claim(JwtRegisteredClaimNames.Sub,user.LastName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString())
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach(var userRole in userRoles)
            { 
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach(var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }*/