using Microsoft.AspNetCore.Mvc;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        [HttpGet(Name = "GetAllRifas")]
        public IEnumerable<int> Get()
        {
            List<int> numeros = new List<int> { 1, 2, 3, 4, 5 };
            IEnumerable<int> enumerableNumeros = numeros;

            return enumerableNumeros;
        }
    }
}
