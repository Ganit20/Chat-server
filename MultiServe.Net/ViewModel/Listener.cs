using MultiServe.Net.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiClientServer.ViewModel
{
    class Listener
    {

        static TcpListener listen;
        public static List<User> usersList = new List<User>();
        public static List<Room_info> Rooms = new List<Room_info>();
        public static int id = 0;
        public static int roomid = 1;
        static public void StartListener()
        {
            Room_info Main = new Room_info() { id = 0, name = "Main", RoomCreator = null };
            Rooms.Add(Main);
            IPHostEntry HostIp = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            listen = new TcpListener(ip, 8000);
            Console.WriteLine("Listener Started at " + ip.ToString());
            listen.Start();
            try
            {
                while (true)
                {
                    TcpClient client = listen.AcceptTcpClient();                    
                    Task.Factory.StartNew(() =>
                    {
                        HandleUser.HandleUserStart(client);
                    });
                    id++;
                   
                }
            }
            catch (SocketException)
            {
                listen.Stop();
    }
   }
    }}