using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Storage;

namespace WebApplication1.Controllers
{
    [Route("api")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        [Route("airports")]
        [HttpGet]

        public IActionResult SearchAirport(string search)
        {
            var trimedPhrase = search.Trim().ToLower();

            var fromAirportCodes = FlightStorage.GetFlights()
                .Where(a =>
                    a.From.Country.ToLower().Contains(trimedPhrase) ||
                    a.From.City.ToLower().Contains(trimedPhrase) ||
                    a.From.AirportCode.ToLower().Contains(trimedPhrase))
                .Select(a => a.From);

            var toAirportCodes = FlightStorage.GetFlights()
                .Where(a =>
                    a.To.Country.ToLower().Contains(trimedPhrase) ||
                    a.To.City.ToLower().Contains(trimedPhrase) ||
                    a.To.AirportCode.ToLower().Contains(trimedPhrase))
                .Select(a => a.To);

            var matchingAirports = fromAirportCodes
                .Union(toAirportCodes)
                .Distinct()
                .ToList();

            if (matchingAirports.Any())
            {
                return Ok(matchingAirports);
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

            var flightsResult = FlightStorage.GetFlights()
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
            var flight = FlightStorage.GetFlightById(id);

            if (flight is null)
            {
                return NotFound();
            }

            return Ok(flight);
        }
    }
}
