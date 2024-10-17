using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Storage;

namespace WebApplication1.Controllers
{
    [Route("admin-api")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        [Route("flights/{id}")]
        [HttpGet]

        public IActionResult GetFlight(int id)
        {
            var flight = FlightStorage.GetFlightById(id);
            return NotFound(flight);
        }

        [Route("flights/{id}")]
        [HttpDelete]

        public IActionResult DeleteFlight(int id)
        {
            lock (FlightStorage.FlightLock)
            {
                FlightStorage.DeleteFlight(id);
                return Ok();
            }
        }

        [HttpPost]
        [Route("flights")]
        public IActionResult AddFlight(FlightRequest flightRequest)
        {
            var flight = FlightStorage.ConvertToFlight(flightRequest);

            lock (FlightStorage.FlightLock)
            {
                if (FlightStorage.CheckExistingFlights(flight))
                {
                    return Conflict();
                }

                if (!CheckForInvalidProperties(flight)
                    || flight.From.AirportCode.Trim().ToLower() == flight.To.AirportCode.Trim().ToLower()
                    || !CheckIfValidDates(flight))
                {
                    return BadRequest();
                }

                FlightStorage.AddFlight(flight);

                return Created("", flight);
            }
        }

        private static bool CheckIfValidDates(Flight flight)
        {
            if (DateTime.TryParse(flight.DepartureTime, out DateTime departure) &&
                DateTime.TryParse(flight.ArrivalTime, out DateTime arrival))
            {
                if (departure < arrival && departure > DateTime.MinValue && arrival > DateTime.MinValue)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckForInvalidProperties(Flight flight)
        {
            return flight != null &&
                   flight.From != null &&
                   !string.IsNullOrEmpty(flight.From.Country) &&
                   !string.IsNullOrEmpty(flight.From.City) &&
                   !string.IsNullOrEmpty(flight.From.AirportCode) &&
                   flight.To != null &&
                   !string.IsNullOrEmpty(flight.To.Country) &&
                   !string.IsNullOrEmpty(flight.To.City) &&
                   !string.IsNullOrEmpty(flight.To.AirportCode) &&
                   !string.IsNullOrEmpty(flight.Carrier) &&
                   !string.IsNullOrEmpty(flight.DepartureTime) &&
                   !string.IsNullOrEmpty(flight.ArrivalTime);
        }
    }
}
