using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace AirportLuggageSort
{
    internal class LuggageResequencer
    {
        protected MessageQueue inQueue = new MessageQueue(@".\Private$\LuggageInfo");
        protected MessageQueue outQueue = new MessageQueue(@".\Private$\PassengerInfo");
        private Dictionary<string, List<Luggage>> buffer = new Dictionary<string, List<Luggage>>();

        public LuggageResequencer()
        {
            inQueue.Formatter = new ActiveXMessageFormatter();
            outQueue.Formatter = new ActiveXMessageFormatter();
            inQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnMessage);
            inQueue.BeginReceive();
        }

        private void OnMessage(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            MessageQueue mq = (MessageQueue)source;
            mq.Formatter = new ActiveXMessageFormatter();
            Message message = mq.EndReceive(asyncResult.AsyncResult);
            string json = (string)message.Body;

            Console.WriteLine("Received message");
            //Strongly typed object
            Luggage luggage = JsonSerializer.Deserialize<Luggage>(json);

            //Park luggage
            if (!buffer.ContainsKey(luggage.Id))
            {
                buffer[luggage.Id] = new List<Luggage>();
            }
            buffer[luggage.Id].Add(luggage);
            int totalInSequence = int.Parse(luggage.TotalInSequence);
            int currentCount = buffer[luggage.Id].Count;

            Console.WriteLine($"Received luggage: Id={luggage.Id}, Seq={luggage.Identification}/{luggage.TotalInSequence}");

            //publish luggage
            if (currentCount == totalInSequence)
            {
                var ordered = buffer[luggage.Id]
                        .OrderBy(l => int.Parse(l.Identification))
                        .ToList();
                publishMessages(ordered);
                buffer.Remove(luggage.Id);
            }

            mq.BeginReceive();

        }

        private void publishMessages(List<Luggage> ordered)
        {
            foreach (var luggage in ordered)
            {
              
                var envelope = new MessageEnvelope
                {
                    MessageType = "Luggage",
                    CorrelationId = luggage.Id,
                    Payload = JsonSerializer.SerializeToElement(luggage)
                };

             
                string json = JsonSerializer.Serialize(envelope);

                
                var message = new Message
                {
                    Body = json,
                    Label = "Luggage Resequenced",
                    Formatter = new ActiveXMessageFormatter(),
                    Recoverable = true 
                };

                outQueue.Send(message);

                Console.WriteLine($"Publishing luggage {luggage.Identification} for sequence {luggage.Id}");
            }

        }


    }
}
