using MultiServe.Net.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            Dictionary<string, string> config = new Dictionary<string, string>();
           
            if (File.Exists("Server.config"))
            {
                var a = File.ReadAllText("server.config");
                config = JsonConvert.DeserializeObject<Dictionary<string,string>>(a);
                Console.WriteLine("Server.config loaded...");

            }
            else
            {
                Console.WriteLine("Creating server.config");
                config.Add("IP", "127.0.0.1");
                config.Add("Port", "8000");
                config.Add("Allow-room-Creation", "true");
                File.WriteAllText("server.config",JsonConvert.SerializeObject(config));
                Console.WriteLine("server.config created...");

            }
            Room_info Main = new Room_info() { id = 0, name = "Main", RoomCreator = null };
            Rooms.Add(Main);
            IPHostEntry HostIp = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ip = IPAddress.Parse(config["IP"]);
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