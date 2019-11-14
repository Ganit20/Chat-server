using MultiClientServer.Model;
using MultiClientServer.ViewModel;
using MultiServe.Net.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiServe.Net.ViewModel
{
    class UserCommands
    {
        private byte[] message = new byte[120];
        private string data;
        private byte[] bytes;

        public void CreateRoom(String c, NetworkStream Stream, User oUser)
        {
            try
            {
                Int32 p = Stream.Read(message, 0, message.Length);
                var b = System.Text.Encoding.ASCII.GetString(message, 0, message.Length);
                var d = JsonConvert.DeserializeObject<Room_info>(b);
                Room_info room = new Room_info();
                room.Create(d.name, oUser, d.isPassword, d.password);
            }
            catch (IOException e)
            {
                GlobalMessage.UserDisconnected(oUser);
            }
            catch (ObjectDisposedException)
            {
                GlobalMessage.UserDisconnected(oUser);
            }
            catch (System.ArgumentOutOfRangeException) { }

        }
        public void ChangeRoom(String c, NetworkStream Stream, User oUser)
        {
            try
            {
                Int32 p = Stream.Read(message, 0, message.Length);
                var b = System.Text.Encoding.ASCII.GetString(message, 0, message.Length);
                var o = JsonConvert.DeserializeObject<Room_info>(b);
                var changer = Listener.usersList.Find(e => e.Name.Equals(o.name));
                var OldRoom = Listener.Rooms.Find(e => e.id.Equals(changer.getRoomID()));
                var NewRoom = Listener.Rooms.Find(e => e.name.Equals(o.NewRoom));
                if (NewRoom.isPassword)
                {
                    if (NewRoom.password.Equals(o.password))
                    {
                        
                        OldRoom.UserList.Remove(changer);
                        NewRoom.UserList.Add(changer);
                        changer.setRoomID(NewRoom.id);
                        GlobalMessage.SendUserList();
                        new Room_info().Check(OldRoom.id);
                        changer.SendMessage("?SSG?You are chatting at: " + NewRoom.name + "?END");
                    }else
                    {
                        changer.SendMessage("?SSG?WRONG PASSWORD ?END");
                    }

                }
                else
                {
                    
                    OldRoom.UserList.Remove(changer);
                    NewRoom.UserList.Add(changer);
                    changer.setRoomID(NewRoom.id);
                    GlobalMessage.SendUserList();
                    new Room_info().Check(OldRoom.id);
                    changer.SendMessage("?SSG?You are chatting at: " + NewRoom.name + "?END");
                }
            }
            catch (System.NullReferenceException)
            {

            }
        }
        public void ReplyMSG(String c, NetworkStream Stream, User oUser)
        {
            try
            {
                if (c.IndexOf("?", 0, 4) > 0)
                {

                    String d = c.Substring(0, c.IndexOf('?', 0, 4));
                    int bl = int.Parse(d);
                    bytes = new byte[bl];
                    Int32 leng = Stream.Read(bytes, 0, bl);
                    message = new byte[leng];
                    message = bytes;

                    data = Encoding.ASCII.GetString(message, 0, message.Length);
                    String msgleng = message.Length + "?";
                    while (msgleng.Length < 4) msgleng = "0" + msgleng;

                    var msgdata = JsonConvert.DeserializeObject<Msg_Info>(data);
                    if (msgdata != null)
                    {
                        string str = msgdata.MsgTime + " " + msgdata.From + "(" + msgdata.IP + ") : " + msgdata.Message;
                        var send = JsonConvert.SerializeObject(str);
                        Console.WriteLine(str);
                        int g = Listener.usersList.Find(e => e.Name.Equals(msgdata.From)).getRoomID();
                        Task.Factory.StartNew(() =>
                        {
                            Listener.Rooms.Find(e => e.id == g).SendRoom("?MSG?" + data + "?END");
                        });

                    }


                }
            }
            catch (IOException e)
            {
                GlobalMessage.UserDisconnected(oUser);
            }
            catch (ObjectDisposedException)
            {
                GlobalMessage.UserDisconnected(oUser);
            }
            catch (System.ArgumentOutOfRangeException) { }


        }
        public void CheckPassword(String c, NetworkStream Stream, User oUser)
        {
            Int32 p = Stream.Read(message, 0, message.Length);
            var b = System.Text.Encoding.ASCII.GetString(message, 0, message.Length);
            var json = JsonConvert.DeserializeObject<Room_info>(b);
            var d = Listener.Rooms.Find(e => e.name.Equals(json.name));
            if(d.password.Equals(json.password)) {
                var g = new Room_info() { name = json.name, password = "True" };
                var SJson = JsonConvert.SerializeObject(g);
                var msg = "?PAS?" + SJson + "?END";
                msg = msg.Length + msg;
                byte[] bmsg = System.Text.Encoding.ASCII.GetBytes(msg);
                Stream.Write(bmsg,0,bmsg.Length);
            }else
            {
                var g = new Room_info() { name = json.name, password = "False" };
                var SJson = JsonConvert.SerializeObject(g);
                var msg = "?PAS?" + SJson + "?END";
                msg = msg.Length + msg;
                byte[] bmsg = System.Text.Encoding.ASCII.GetBytes(msg);
                Stream.Write(bmsg, 0, bmsg.Length);
            }
        }
    }
}
