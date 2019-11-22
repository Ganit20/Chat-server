using System;
using System.Threading;
using System.Threading.Tasks;

using MultiClientServer.ViewModel;

namespace MultiClientServer
{
    class Server
    {
        public Server()
        {
            Console.WriteLine("starting server");
           
           Task.Factory.StartNew(() => {
               Listener.StartListener();
               });

            while (true)
            {

                String Command = Console.ReadLine();
                Task.Factory.StartNew(() =>
                {
                    new Commands().SetCommandAsync(Command,"server");
                });
            }
        }
        }
    }

