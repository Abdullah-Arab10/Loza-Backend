using Loza.Data;
using Loza.Entities;
//using Loza.Migrations;
using Loza.Models.ResponseModels;
using Loza.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Loza.Models.DTO;

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
        private readonly DataContext _dataContext;

        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration, AppDbContext context,DataContext dataContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _dataContext = dataContext;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
        {
            //validate the incoming request 
            if (ModelState.IsValid)
            {
                //we need to check if the email already exist
                var user_exist = await _userManager.FindByEmailAsync(requestDto.email);
                var isBlocked = await _context.blockedAccounts.AnyAsync(b => b.Email == requestDto.email);
                if (isBlocked)
                {
                    return StatusCode(403, new OperationsResult
                    {
                        statusCode = 403,
                        isError = true,
                        Errors = new ErrorModel { Message = "Email Blocked" }
                    });
                }

                if (user_exist != null)
                {
                    return BadRequest(new OperationsResult
                    {
                        statusCode = 400,
                        isError = true,
                        Errors = new ErrorModel { Message = "Email already exist" }
                    });
                }
                //create a user 
                var new_user = new User()
                {
                    Email = requestDto.email,
                    FirstName = requestDto.FirstName,
                    LastName = requestDto.LastName,
                    PhoneNumber = requestDto.phoneNumber,
                    UserName = requestDto.email,
                    Address = requestDto.Address,
                    DateOfBirth = DateOnly.ParseExact(requestDto.DateOfBirth, "dd/MM/yyyy", null)
                };

                var is_created = await _userManager.CreateAsync(new_user, requestDto.password);

                if (is_created.Succeeded)
                {
                    var result = await _userManager.AddToRoleAsync(new_user, "User");
                    int adddressUserId = new_user.Id;
                    var address = new Address
                    {
                        UserId = adddressUserId,
                        Location = requestDto.Address,
                        AddressName = "Default"
                    };
                    await _dataContext.Addresses.AddAsync(address);
                    await _dataContext.SaveChangesAsync();
                    if (result.Succeeded)
                    {
                        return Ok(new OperationsResult
                        {
                            statusCode = 200,
                            isError = false
                        });
                    }
                    
                    return BadRequest();
                }
               
                var errors = new List<string>();
                foreach (var error in is_created.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(new OperationsResult
                {
                    statusCode= 400,
                    isError = true,
                    Errors = new ErrorModel { Message= string.Join(",", errors) }
                });
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
                    return BadRequest(new OperationsResult
                    {
                        statusCode = 400,
                        isError= true,
                        Errors = new ErrorModel { Message = "Wrong Email" }
                    });
                }
                var isCorrect = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
                if (!isCorrect)
                {
                    return Unauthorized(new OperationsResult
                    {
                        statusCode = 401,
                        isError= true,
                        Errors = new ErrorModel { Message = "Wrong Password" }
                    });
                }
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.FirstName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.LastName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim("role", userRole));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value));

                var new_token = new JwtSecurityToken(
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
                var jwtToken = new JwtSecurityTokenHandler().WriteToken(new_token);

                Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "token", jwtToken }
                    };

                return Ok(new OperationsResult
                {
                    statusCode = 200,
                    isError = false,
                    Data = data
                });

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
                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError = true,
                    Errors =  new ErrorModel { Message = "Email does not exist" } 
                });
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(new OperationsResult
                {
                    statusCode = 401,
                    isError = true,
                    Errors =  new ErrorModel { Message = string.Join(",", errors) } 
                });
            }
            return Ok(new OperationsResult 
            { 
                statusCode = 200,
                isError = false
            });
        }

        [Route("Update/{Id}")]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UserUpdateRequestDto model, int Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            var response = new OperationsResult();
            if (user == null)
            {
                return BadRequest(new OperationsResult
                {
                    statusCode = 400,
                    isError = true,
                    Errors = new ErrorModel { Message = "User not Found " } 
                });
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
                    return Ok(new OperationsResult
                    {
                        statusCode = 200,
                        isError= false
                    });
                }

                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                return BadRequest(new OperationsResult
                {
                    statusCode = 400,
                    isError= true,
                    Errors = new ErrorModel { Message = string.Join(",", errors) }
                });
            }
            else
            {
                return Ok(new OperationsResult
                {
                    statusCode= 200,
                    isError= false
                });
            }

        }
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var user = await _context.users.FindAsync(Id);
            var response = new OperationsResult();
            if (user == null)
            {
                return NotFound(new OperationsResult
                {
                    statusCode = 404,
                    isError= true,
                    Errors = new ErrorModel { Message = "User does not exists" }
                });
            }
            var address = await _dataContext.Addresses.Where(a => a.UserId == user.Id).ToListAsync();
            _dataContext.Addresses.RemoveRange(address);
            await _dataContext.SaveChangesAsync();
            _context.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new OperationsResult
            {
                statusCode = 200,
                isError= false
            });
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