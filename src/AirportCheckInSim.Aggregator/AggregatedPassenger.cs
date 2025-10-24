using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PassengerLuggageAggregator
{
    public class AggregatedPassenger
    {
       public Passenger passenger {  get; set; }
       public List<Luggage> luggage { get; set; }

    }
    public class Passenger
    {
        public string ReservationNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PiecesOfLuggage { get; set; }
    }

    public class Luggage
    {
        public string Id { get; set; }
        public string Identification { get; set; }
        public string TotalInSequence { get; set; }
        public string Category { get; set; }
        public string Weight { get; set; }
    }

    public class MessageEnvelope
    {
        public string MessageType { get; set; }
        public string CorrelationId { get; set; }
        public JsonElement Payload { get; set; }
    }
}
