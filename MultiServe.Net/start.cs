using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MultiClientServer
{
    class start
    {
        static void Main(string[] args)
        {

            Thread t = new Thread(delegate ()
            {
                Server s = new Server();
            });
            t.Start();
            Console.WriteLine("STARTING");
        }
    }
}
