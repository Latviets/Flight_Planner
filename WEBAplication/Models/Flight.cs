namespace WebApplication1.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public Airport From { get; set; }
        public Airport To { get; set; }
        public string Carrier { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Flight other = (Flight)obj;

            return From.Country == other.From.Country &&
                   From.City == other.From.City &&
                   From.AirportCode == other.From.AirportCode &&
                   To.Country == other.To.Country &&
                   To.City == other.To.City &&
                   To.AirportCode == other.To.AirportCode &&
                   Carrier == other.Carrier &&
                   DepartureTime == other.DepartureTime &&
                   ArrivalTime == other.ArrivalTime;
        }
    }
}
