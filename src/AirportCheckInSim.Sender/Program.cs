using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Messaging;
using AirportCheckInSim.Sender;

namespace AirportCheckInSim.Sender
{
    // Root object representing the JSON structure
    class Program
    {
        static void Main(string[] args)
        {
            MessageQueue messageQueue = null;
            messageQueue = new MessageQueue(@".\Private$\AirportCheckInOutput");
            messageQueue.Label = "CheckIn Queue";
            messageQueue.Formatter = new ActiveXMessageFormatter();

            var airportInfo = new AirportInfo

            {
                Flight = new Flight
                {
                    Attributes = new FlightAttributes
                    {
                        number = "SK937",
                        Flightdate = "20220225"
                    },
                    Origin = "ARLANDA",
                    Destination = "LONDON"
                },
                Passenger =
                [
                    new() {
                        ReservationNumber = "CA937200305252",
                        FirstName = "Anders",
                        LastName = "And",
                        PiecesOfLuggage = "2"
                    },
                    new() {
                        ReservationNumber = "CA937200305253",
                        FirstName = "Andersine",
                        LastName = "And",
                        PiecesOfLuggage = "2"
                    }
                ],
                Luggage =
                [
                    new() {
                        Id = "CA937200305252",
                        Identification = "1",
                        TotalInSequence = "2",
                        Category = "Normal",
                        Weight = "17.3"
                    },
                    new() {
                        Id = "CA937200305252",
                        Identification = "2",
                        TotalInSequence = "2",
                        Category = "Large",
                        Weight = "22.7"
                    },
                    new() {
                        Id = "CA937200305253",
                        Identification = "1",
                        TotalInSequence = "2",
                        Category = "Large",
                        Weight = "24.2"
                    },
                    new() {
                        Id = "CA937200305253",
                        Identification = "2",
                        TotalInSequence = "2",
                        Category = "Large",
                        Weight = "21.4"
                    }
                ]
            };

            string json = JsonSerializer.Serialize(airportInfo);
            Console.WriteLine(json);

            string AirlineCompany = "SAS";

            var msg = new Message
            {
                Body = json,
                Label = AirlineCompany,
                Formatter = new ActiveXMessageFormatter()
            };

            messageQueue.Send(msg);
        }
    }
}
