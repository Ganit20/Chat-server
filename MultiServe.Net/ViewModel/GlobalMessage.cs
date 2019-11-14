using MultiClientServer.Model;
using MultiServe.Net.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;

namespace MultiClientServer.ViewModel
{
    class GlobalMessage
    {

        public static int Control = 0;
        public static int ControlR = 0;
        static int roomid=0;

        public static void UserJoined(String Username, String IP)
        {
                Console.WriteLine(DateTime.Now + " New Connection " + Username + " IP: " + IP);
                ServerMessage(Username + " Joined");
            Task.Factory.StartNew(() =>
            {
                GlobalMessage.SendUserList();
            });
            Task.Factory.StartNew(() =>
            {
                GlobalMessage.sendRoomList();
            });
        }

        public static void UserDisconnected(User usr)
        {
            usr.Tcp.Close();
            Console.WriteLine("["+DateTime.UtcNow + "] " + usr.Name + " Disconnected");
            var ql = Listener.Rooms.Find(u => u.id==usr.RoomID);
            new Room_info().Check(usr.RoomID);
            ql.UserList.Remove(usr);
            Listener.usersList.Remove(usr);
            SendUserList();
            ServerMessage(usr.Name + " Disconnected");
            
        }
        public static void ServerMessage(String messages)
        {
            var msg = new Msg_Info()
            {
                From = "SERVER:",
                Message = messages,
                MsgTime = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()
            };
            var msgJson = JsonConvert.SerializeObject(msg);
            foreach (var item in Listener.Rooms) {
                item.SendRoom("?" + msgJson);
               }

        }
        public static void sendRoomList()
        {
            Dictionary<string, bool> roomlist = new Dictionary<string, bool>();
            string send = "RCC?";
            string b = send;
            foreach (var roomer in Listener.Rooms)
            {
                roomlist.Add(roomer.name, roomer.isPassword);
            }
            var json = JsonConvert.SerializeObject(roomlist, Formatting.Indented);
            b = b + json + "?END";
            var leng = b.Length.ToString();
            while(leng.Length<3)
            {
                leng = "0" + leng;
            }
            b = leng + "?" + b ;
            foreach (var item in Listener.Rooms)
            {
                item.SendRoomList(b);
            }
        }



        public static void SendUserList()
        {
            try
            {
                
                foreach (var a in Listener.Rooms)
                {
                    string list = String.Empty;
                    foreach (var item in a.UserList)
                    {
                        list += item.Name + "~";
                    }
                    var d = "USE?";
                    var msg = d + list + "?END";
                    var leng = msg.Length.ToString();
                    while (leng.Length < 3)
                    {
                        leng = "0" + leng;
                    }
                    msg = leng + "?" + msg ;
                    foreach (var item in a.UserList)
                    {
                        a.UserListSend(msg);
                    }
                }
                roomid++;

            }
            catch(InvalidOperationException) { }
        }
        }
    }


