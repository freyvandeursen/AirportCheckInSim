using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassengerLuggageAggregator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Aggregator running...");
            PassengerLuggageAggregator psl = new PassengerLuggageAggregator();
            Console.ReadLine();
        }
    }
}
