using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Text.Json;

namespace Splitter
{
    internal class SplitterClass
    {
        protected MessageQueue inQueue = new MessageQueue(@".\Private$\AirportCheckInOutput");
        protected MessageQueue luggageQueue = new MessageQueue(@".\Private$\LuggageInfo");
        protected MessageQueue passengerQueue = new MessageQueue(@".\Private$\PassengerInfo");

        public SplitterClass()
        {
            inQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            inQueue.BeginReceive();
        }

        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            mq.Formatter = new ActiveXMessageFormatter();
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            string json = (string) message.Body;
            luggageQueue.Formatter = new ActiveXMessageFormatter();
            passengerQueue.Formatter = new ActiveXMessageFormatter();

            //Strongly typed object
            AirportInfo airportInfo = JsonSerializer.Deserialize<AirportInfo>(json);

            //PassengerSide
            foreach (var passenger in airportInfo.Passenger)
            {
                var envelope = new MessageEnvelope
                {
                    MessageType = "Passenger",
                    CorrelationId = passenger.ReservationNumber,
                    Payload = JsonSerializer.SerializeToElement(passenger)
                };

                string passengerJson = JsonSerializer.Serialize(envelope);
                Console.WriteLine("Sending passenger info to PassengerInfo queue...");
                passengerQueue.Send(passengerJson, "Passenger info");
            }

            //LuggageSide
            foreach (var luggage in airportInfo.Luggage)
            {
                var luggageInfo = new Luggage
                {
                    Id = luggage.Id,
                    Identification = luggage.Identification,
                    TotalInSequence = luggage.TotalInSequence,
                    Category = luggage.Category,
                    Weight = luggage.Weight
                };

                string luggageJson = JsonSerializer.Serialize(luggageInfo);
                Console.WriteLine("Sending luggage info to LuggageInfo queue...");
                luggageQueue.Send(luggageJson, "Luggage info");
            }



            mq.BeginReceive();

        }
    }
}
