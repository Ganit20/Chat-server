
using App3;
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

        public void CreateRoom(NetworkStream Stream, User oUser, string info)
        {

            try
            {

                var d = new TextOperations().ReadRoom_info(Stream, info);
                Room_info room = new Room_info();
                room.Create(d.name, oUser, d.isPassword, d.password);
            }
            catch (IOException)
            {
                GlobalMessage.UserDisconnected(oUser);
            }
            catch (ObjectDisposedException)
            {
                GlobalMessage.UserDisconnected(oUser);
            }
            catch (System.ArgumentOutOfRangeException) { }
            catch (System.NullReferenceException) { }


        }
        public void ChangeRoom(NetworkStream Stream, User oUser, string message)
        {
            try
            {
                var o = new TextOperations().ReadRoom_info(Stream, message);
                var changer = Listener.usersList.Find(e => e.Name.Equals(o.name));
                var OldRoom = Listener.Rooms.Find(e => e.id.Equals(changer.GetRoomID()));
                var NewRoom = Listener.Rooms.Find(e => e.name.Equals(o.NewRoom));
                if (NewRoom.isPassword)
                {
                    if (NewRoom.password.Equals(o.password))
                    {
                        new TextOperations().RoomChanger(changer, OldRoom, NewRoom);
                    }
                    else
                    {
                        changer.SendMessage("SSG?","WRONG PASSWORD");
                    }
                }
                else
                {

                    new TextOperations().RoomChanger(changer, OldRoom, NewRoom);
                }
            }
            catch (System.NullReferenceException) { }
        }
        public void ReplyMSG(string c, NetworkStream Stream, User oUser)
        {
            try
            {
                var pg = new TextOperations().LengthStream(c);
                byte[] bytes = new byte[pg];
                Int32 leng = Stream.Read(bytes, 0, bytes.Length);

                var i = System.Text.Encoding.UTF8.GetString(bytes);
                var messag = new Encryption().Decrypt(bytes);
                var msgdata = JsonConvert.DeserializeObject<Msg_Info>(messag);
                data = Encoding.ASCII.GetString(message, 0, message.Length);
               


                if (msgdata != null)
                {
                    string str = msgdata.MsgTime + " " + msgdata.From + "(" + msgdata.IP + ") : " + msgdata.Message;
                    var g = Listener.usersList.Find(e => e.Name.Equals(msgdata.From));
                    if (msgdata.Message[0].Equals('/') && g.Rank.Equals("admin"))
                    {
                        Task.Factory.StartNew(() =>
                        {
                            new Commands().SetCommandAsync(msgdata.Message.Substring(1, msgdata.Message.Length - 1), msgdata.From);
                        });
                    }
                    else if (!msgdata.Message[0].Equals('/'))
                    {
                        var send = JsonConvert.SerializeObject(str);
                        new Logs().saveLogs(str);
                        Console.WriteLine(str);
                        int m = Listener.usersList.Find(e => e.Name.Equals(msgdata.From)).GetRoomID();
                        
                        Task.Factory.StartNew(() =>
                        {
                            Listener.Rooms.Find(e => e.id == m).SendRoom("MSG?", messag);
                        });
                    } else if (!g.Rank.Equals("admin"))
                    {
                        var send = " You Don't Have Permission for that";
                        var json = JsonConvert.SerializeObject(new Msg_Info() { From = "info", Message = send });
                        g.SendMessage("MSG?" , json);
                    }
                }


            }

            catch (IOException)
            {
                GlobalMessage.UserDisconnected(oUser);
            }
            catch (ObjectDisposedException)
            {
                GlobalMessage.UserDisconnected(oUser);
            }
            catch (ArgumentOutOfRangeException) { }
            catch (IndexOutOfRangeException) { }


        } 
        public void CheckPassword( NetworkStream Stream)
        {
            string message = "l";
            var json = new TextOperations().ReadRoom_info(Stream, message);
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
        public bool Register(NetworkStream stream,string UserInfo)
        {
            var UserJson = JsonConvert.DeserializeObject<User>(UserInfo.Substring(4, UserInfo.LastIndexOf('?') - 4));
            if (new DBConnect(Listener.config).UserRegister(UserJson.Name, UserJson.password, UserJson.email))
            {
                var msg = "RDC?" + "Confirmed" + "?END";
                msg = new TextOperations().MessageLength(msg);
                stream.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
                return true;
            }
            else
            {
                var msg = "RDC?" + "Nope" + "?END";
                msg = new TextOperations().MessageLength(msg);
                stream.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
                return false;
            }
        }
        public bool Login(NetworkStream stream, string UserInfo,User oUser,TcpClient user)
        {
            string msg;
            byte[] ByteLength = new byte[4];
            string d = UserInfo.Substring(UserInfo.IndexOf('?') + 1, UserInfo.LastIndexOf('?') - UserInfo.IndexOf('?') - 1);
            int bl = int.Parse(d);
                byte[] bytes = new byte[bl];
                Int32 leng = stream.Read(bytes, 0, bl);
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
            var messag = new Encryption().Decrypt(bytes);
            var UserJson = JsonConvert.DeserializeObject<User>(messag);
            if (new DBConnect(Listener.config).UserLogin(UserJson.Name, UserJson.password))
            {
                
                oUser = new DBConnect(Listener.config).DownloadUserInfo(UserJson.Name, stream, user);
                Console.WriteLine("Ban checking...");
                if(oUser.banned ==1)
                {
                    Console.WriteLine(oUser.Name + " is banned disconnecting");
                    Msg_Info m = new Msg_Info() { From = "Server", MsgTime = DateTime.UtcNow.ToString(), Message = "You Are banned for " + oUser.bannedFor };
                     msg = "BAN?"+JsonConvert.SerializeObject(m) +"?END";
                    msg =new TextOperations().MessageLength(msg);
                    stream.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
                    GlobalMessage.UserDisconnected(oUser);
                    return false;
                }else
                {
                    Console.WriteLine(oUser.Name + " Logged");
                    HandleUser.oUser = oUser;
                    msg = "LOG?TRUE?END";
                    msg = new TextOperations().MessageLength(msg);
                    stream.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
                    return true;
                }
                
                
            }
            else
            {
                 msg = "LOG?WRONG?END";
                msg = new TextOperations().MessageLength(msg);
                stream.Write(Encoding.ASCII.GetBytes(msg), 0, msg.Length);
                return false;
            }
        }
    }
}
