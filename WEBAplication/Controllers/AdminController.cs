using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Database;
using WebApplication1.Models;
using WebApplication1.Storage;

namespace WebApplication1.Controllers
{
    [Route("admin-api")]
    [ApiController]
    [Authorize]
    public class AdminController(FlightStorage storage) : ControllerBase
    {
        private readonly FlightStorage _storage = storage;

        [Route("flights/{id}")]
        [HttpGet]

        public IActionResult GetFlight(int id)
        {
            var flight = _storage.GetFlightById(id);
            return NotFound(flight);
        }

        [Route("flights/{id}")]
        [HttpDelete]

        public IActionResult DeleteFlight(int id)
        {
            //lock (FlightStorage.FlightLock)
            //{
                _storage.DeleteFlight(id);
                return Ok();
            //}
        }

        [HttpPost]
        [Route("flights")]
        public IActionResult AddFlight(FlightRequest flightRequest)
        {
            var flight = _storage.ConvertToFlight(flightRequest);

            //lock (FlightStorage.FlightLock)
            //{
                if (_storage.CheckExistingFlights(flight))
                {
                    return Conflict();
                }

                if (!CheckForInvalidProperties(flight)
                    || flight.From.AirportCode.Trim().ToLower() == flight.To.AirportCode.Trim().ToLower()
                    || !CheckIfValidDates(flight))
                {
                    return BadRequest();
                }

                _storage.AddFlight(flight);

                return Created("", flight);
            //}
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
