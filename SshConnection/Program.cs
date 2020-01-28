using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SshConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.Connect("ip", 22, "user", "passw");
            string result = client.CreateCommand("ls -la");
            Console.WriteLine(result.ToString());

            Console.WriteLine("--------------------------");


            string stream = client.CreateCommandWithStream("ls -la");
            Console.WriteLine(stream.ToString());

            client.Disconnect();
            Console.ReadLine();
            
        }
    }
}
