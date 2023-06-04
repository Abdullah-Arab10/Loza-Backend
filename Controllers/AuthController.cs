using Loza.Data;
using Loza.Entities;
using Loza.Models;
using Loza.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(UserManager<User> userManager, IConfiguration configuration, AppDbContext context)
        {
            _userManager = userManager;
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

                if (user_exist != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Email already exists" });
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
                    return Ok(new Response { Status = "Success", Message = "User created successfully" });
                }
                var errors = new List<string>();
                foreach (var error in is_created.Errors)
                {
                    errors.Add(error.Description);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = string.Join(",", errors) });

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
                var existing_user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (existing_user == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Invalid email" });

                }
                var isCorrect = await _userManager.CheckPasswordAsync(existing_user, loginRequest.Password);
                if (!isCorrect)
                {
                    return Unauthorized();
                }
                var userRoles = await _userManager.GetRolesAsync(existing_user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,existing_user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }


                var iwtToken = GenerateJwtToken(existing_user);

                return Ok(new { token = iwtToken });



            }
            return StatusCode(StatusCodes.Status500InternalServerError);


        }

        //[Authorize]
        [Route("Change_Password")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePassword model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User does not exists" });
            }
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = string.Join(",", errors) });

            }
            return Ok(new Response { Status = "Success", Message = "Password changed successfuly" });
        }

        [Route("Update/{Id}")]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UserRegistrationRequestDto model, int Id)
        {
            var user = _context.users.Find(Id);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User does not exists" });
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.email;
            user.PhoneNumber = model.phoneNumber;
            user.Address = model.Address;
            user.DateOfBirth = model.DateOfBirth;
            _context.SaveChanges();
            return Ok(new Response { Status = "Success", Message = "Edited successfuly" });


        }
        [HttpDelete("Delete/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var user = await _context.users.FindAsync(Id);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = "User does not exists" });
            }
            _context.Remove(user);
            _context.SaveChanges();

            return Ok(new Response { Status = "Success", Message = "Account deleted successfuly" });
        }
        //Generate JWT token
        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            //Token descriptor
            var tokendescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim("Id",user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString())
                }),
                Expires = DateTime.Now.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokendescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }

}
