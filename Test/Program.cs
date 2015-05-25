
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Models;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SignalRClient _client = new SignalRClient();
            _client.On<Message>("DisplayMessage", new Action<Message>(x =>
                {
                    displayMessage(x);
                }
                ));
            _client.Open();
            Console.ReadKey();
        }

        private static void displayMessage(Message x)
        {
            Console.WriteLine(x.Username + ": " + x.Content);
            Console.ReadKey();
        }


    }
}
