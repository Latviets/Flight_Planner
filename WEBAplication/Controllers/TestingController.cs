using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Storage;

namespace WebApplication1.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        [HttpPost]
        [Route("clear")]

        public IActionResult Clear()
        {
            FlightStorage.ClearFlights();

            return Ok();
        }
    }
}
