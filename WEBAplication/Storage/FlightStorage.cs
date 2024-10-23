using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Database;
using WebApplication1.Models;

namespace WebApplication1.Storage
{
    public class FlightStorage(FlightPlannerDBContext context)
    {
        private static readonly object _flightLock = new object();
        public static object FlightLock => _flightLock;

        private readonly FlightPlannerDBContext _context = context;

        public Flight AddFlight(Flight flight)
        {
            lock (_flightLock)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();

                return flight;
            }
        }

        public Flight GetFlightById(int id)
        {
            return _context.Flights
                .Include(fl => fl.From)
                .Include(fl => fl.To)
                .FirstOrDefault(fl => fl.Id == id);
        }

        public void DeleteFlight(int id)
        {
            lock (_flightLock)
            {
                var item = _context.Flights.FirstOrDefault(fl => fl.Id == id);
                if (item != null)
                {
                    _context.Flights.Remove(item);
                    _context.SaveChanges();
                }
            }
        }

        public List<Flight> GetFlights()
        {
            var list = _context.Flights
                .Include(fl => fl.From)
                .Include(fl => fl.To)
                .ToList();

            return list;
        }

        public List<Airport> GetAirports()
        {
            var list = _context.Airports.ToList();
            return list;
        }

        public void ClearFlights()
        {
            lock (_flightLock)
            {
                _context.Flights.RemoveRange(_context.Flights);
                _context.Airports.RemoveRange(_context.Airports);
                _context.SaveChanges();
            }
        }

        public bool CheckExistingFlights(Flight flight)
        {
            if (flight == null || flight.From == null || flight.To == null)
            {
                return false;
            }

            return _context.Flights.Any(fl =>
                fl.From.Country == flight.From.Country &&
                fl.From.City == flight.From.City &&
                fl.From.AirportCode == flight.From.AirportCode &&
                fl.To.Country == flight.To.Country &&
                fl.To.City == flight.To.City &&
                fl.To.AirportCode == flight.To.AirportCode &&
                fl.DepartureTime == flight.DepartureTime &&
                fl.ArrivalTime == flight.ArrivalTime);
        }

        public Flight ConvertToFlight(FlightRequest flightRequest)
        {
            return new Flight
            {
                From = flightRequest.From,
                To = flightRequest.To,
                Carrier = flightRequest.Carrier,
                DepartureTime = flightRequest.DepartureTime,
                ArrivalTime = flightRequest.ArrivalTime
            };
        }
    }
}
