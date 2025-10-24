using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PassengerLuggageAggregator
{
    internal class PassengerLuggageAggregator
    {
        protected MessageQueue inQueue = new MessageQueue(@".\Private$\PassengerInfo");
        //protected MessageQueue outQueuePassengerLuggage = new MessageQueue(@".\Private$\PassengerLuggage");
        private Dictionary<string, List<Luggage>> luggageBuffer = new Dictionary<string, List<Luggage>>();
        private Dictionary<string, Passenger> passengerBuffer = new Dictionary<string, Passenger>();

        public PassengerLuggageAggregator()
        {
            inQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            inQueue.BeginReceive();
        }

        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            mq.Formatter = new ActiveXMessageFormatter();
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            string json = (string)message.Body;

            var envelope = JsonSerializer.Deserialize<MessageEnvelope>(json);

            switch(envelope.MessageType)
            {
                case "Luggage":
                    var luggage = envelope.Payload.Deserialize<Luggage>();
                    HandleLuggage(luggage);
                    break;
                case "Passenger":
                    var passenger = envelope.Payload.Deserialize<Passenger>();
                    HandlePassenger(passenger);
                    break;
            }

            mq.BeginReceive();

        }

        private void HandlePassenger(Passenger passenger)
        {
            passengerBuffer[passenger.ReservationNumber] = passenger;

            if(luggageBuffer.ContainsKey(passenger.ReservationNumber))
            {
                if (luggageBuffer[passenger.ReservationNumber].Count() == int.Parse(passenger.PiecesOfLuggage))
                {
                    finalizePassenger(passenger.ReservationNumber);
                }
            }
            
        }


        private void HandleLuggage(Luggage luggage)
        {
            if (!luggageBuffer.ContainsKey(luggage.Id))
            {
                luggageBuffer[luggage.Id] = new List<Luggage>();
            }
            luggageBuffer[luggage.Id].Add(luggage);
            int totalInSequence = int.Parse(luggage.TotalInSequence);
            int currentCount = luggageBuffer[luggage.Id].Count;
            if (totalInSequence == currentCount && passengerBuffer.ContainsKey(luggage.Id))
            {
                finalizePassenger(luggage.Id);
            }
        }

        private void finalizePassenger(string reservationNumber)
        {
            var aggregatedPassenger = new AggregatedPassenger
            {
                passenger = passengerBuffer[reservationNumber],
                luggage = luggageBuffer[reservationNumber]
            };

            //display passenger
            var p = aggregatedPassenger.passenger;

            Console.WriteLine($"Finalized passenger: {p.FirstName} {p.LastName}");
            Console.WriteLine($"Reservation Number: {p.ReservationNumber}");
            Console.WriteLine($"Pieces of Luggage (expected): {p.PiecesOfLuggage}");
            Console.WriteLine($"Luggage received: {aggregatedPassenger.luggage.Count}");
            Console.WriteLine("--------------------------------------------------");

            foreach (var luggage in aggregatedPassenger.luggage)
            {
                Console.WriteLine($"     Luggage ID: {luggage.Id}");
                Console.WriteLine($"     Seq #: {luggage.Identification}/{luggage.TotalInSequence}");
                Console.WriteLine($"     Category: {luggage.Category}");
                Console.WriteLine($"     Weight: {luggage.Weight} kg");
                Console.WriteLine();
            }

            Console.WriteLine("--------------------------------------------------\n");

            //TO-DO: Migrate to SQL
            //Consume
        }
    }
    }
