using Loza.Data;
using Loza.Entities;
using Loza.Models.DTO;
using Loza.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Loza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public AddressController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpPost]
        [Route("addAddress")]
        public async Task<IActionResult> addAddress(int userId ,[FromForm] AddressDTO address)
        {
            var userAddress = new Address 
            { 
                UserId = userId,
                Location = address.Location,
                AddressName = address.AddressName
            };
            _dataContext.Addresses.Add(userAddress);
            await _dataContext.SaveChangesAsync();
            return Ok(new OperationsResult
            {
                statusCode= 200,
                isError= false
            });
        }
        [HttpGet]
        [Route("getAddressById")]
        public async Task<IActionResult> getAddressById(int userId)
        {
            var address = await _dataContext.Addresses.Where(a=>a.UserId == userId).Select(a=>new 
            {
                Id= a.Id,
                Location = a.Location,
                AddressName = a.AddressName
            }).ToListAsync();
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "Addresses", address }
            };
            return Ok(new OperationsResult
            {
                statusCode= 200,
                isError= false,
                Data= data
            });
        }
    }
}
