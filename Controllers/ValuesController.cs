using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LozaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly DbContext _dbContext;
        public ValuesController(DbContext dbContext)
        {

            _dbContext = dbContext;

        }
    }
}
