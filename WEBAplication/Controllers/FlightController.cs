using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Storage;

namespace WebApplication1.Controllers
{
    [Route("api")]
    [ApiController]
    public class FlightController(FlightStorage storage) : ControllerBase
    {
        private readonly FlightStorage _storage = storage;

        [Route("airports")]
        [HttpGet]

        public IActionResult SearchAirport(string search)
        {
            var trimedPhrase = search.Trim().ToLower();

            var fromAirportCodes = _storage.GetAirports()
                .Where(port =>
                    port.Country.ToLower().Contains(trimedPhrase) ||
                    port.City.ToLower().Contains(trimedPhrase) ||
                    port.AirportCode.ToLower().Contains(trimedPhrase))
                .Select(a => a);

            if (fromAirportCodes.Any())
            {
                return Ok(fromAirportCodes);
            }

            return NotFound("No airports found");
        }

        [Route("flights/search")]
        [HttpPost]

        public IActionResult SearchFlights(SearchFlightsRequest req)
        {
            if (req.From is null || req.To is null || req.DepartureDate == null
                || req.From == req.To)
            {
                return BadRequest();
            }

            var trimmedFrom = req.From.Trim().ToLower();
            var trimmedTo = req.To.Trim().ToLower();
            var parsedDate = DateTime.Parse(req.DepartureDate).Date;

            var flightsResult = _storage.GetFlights()
                .Where(fl =>
                    fl.From.AirportCode.Trim().ToLower() == trimmedFrom &&
                    fl.To.AirportCode.Trim().ToLower() == trimmedTo &&
                    DateTime.Parse(fl.DepartureTime).Date == parsedDate)
                .ToList();


            if (!flightsResult.Any())
            {
                return Ok(new { Page = 0, TotalItems = 0, Items = new List<Flight>() });
            }

            return Ok(new { Page = 0, TotalItems = flightsResult.Count(), Items = flightsResult });
        }

        [Route("flights/{id}")]
        [HttpGet]

        public IActionResult GetFlight(int id)
        {
            var flight = _storage.GetFlightById(id);

            if (flight != null)
            {
                return Ok(flight);
            }

            return NotFound();
        }
    }
}
