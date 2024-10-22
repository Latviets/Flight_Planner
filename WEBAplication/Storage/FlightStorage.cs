using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Storage
{
    public static class FlightStorage
    {
        private static List<Flight> _flights = new List<Flight>();
        private static int _id = 0;
        private static readonly object _flightLock = new object();
        public static object FlightLock => _flightLock;

        public static Flight AddFlight(Flight flight)
        {
            lock (_flightLock)
            {
                flight.Id = ++_id;

                _flights.Add(flight);

                return flight;
            }
        }
        public static Flight Testing(Flight flight)
        {
            lock (_flightLock)
            {
                flight.Id = ++_id;

                _flights.Add(flight);

                return flight;
            }
        }

        public static Flight GetFlightById(int id)
        {
            return _flights.FirstOrDefault(fl => fl.Id == id);
        }

        public static void DeleteFlight(int id)
        {
            lock (_flightLock)
            {
                var item = _flights.SingleOrDefault(fl => fl.Id == id);

                _flights.Remove(item);
            }
        }

        public static List<Flight> GetFlights()
        {
            return _flights;
        }

        public static void ClearFlights()
        {
            _flights.Clear();
        }

        public static bool CheckExistingFlights(Flight flight)
        {
            lock (_flightLock)
            {
                return _flights.Any(fl =>
                    fl.From.Country == flight.From.Country &&
                    fl.From.City == flight.From.City &&
                    fl.From.AirportCode == flight.From.AirportCode &&
                    fl.To.Country == flight.To.Country &&
                    fl.To.City == flight.To.City &&
                    fl.To.AirportCode == flight.To.AirportCode &&
                    fl.DepartureTime == flight.DepartureTime &&
                    fl.ArrivalTime == flight.ArrivalTime);
            }
        }

        public static Flight ConvertToFlight(FlightRequest flightRequest)
        {
            return new Flight
            {
                Id = 0,
                From = flightRequest.From,
                To = flightRequest.To,
                Carrier = flightRequest.Carrier,
                DepartureTime = flightRequest.DepartureTime,
                ArrivalTime = flightRequest.ArrivalTime
            };
        }
    }
}
