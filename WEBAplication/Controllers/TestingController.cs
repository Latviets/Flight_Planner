using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Storage;

namespace WebApplication1.Controllers
{
    [Route("testing-api")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly FlightStorage _storage;

        public TestingController(FlightStorage storage)
        {
            _storage = storage;
        }

        [HttpPost]
        [Route("clear")]

        public IActionResult Clear()
        {
            _storage.ClearFlights();

            return Ok();
        }
    }
}
