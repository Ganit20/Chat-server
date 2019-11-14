using MultiClientServer.Model;
using MultiServe.Net.Model;
using MultiServe.Net.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MultiClientServer.ViewModel
{
    class HandleUser
    {
        public static void HandleUserStart(object obj )
        {
            User oUser ;
            TcpClient user = (TcpClient)obj;
            var stream = user.GetStream();
            byte[] info = new Byte[100];
            stream.Read(info, 0, info.Length);
            var UserInfo = System.Text.Encoding.ASCII.GetString(info);
            begin:
            if (UserInfo.Substring(0, 4).Equals("user")){
                var UserJson = JsonConvert.DeserializeObject<Msg_Info>(UserInfo.Substring(4));
                 oUser = new User(Listener.id, UserJson.From, stream, UserJson.IP, 0, user);
                Listener.usersList.Add(oUser);
                GlobalMessage.UserJoined( oUser.Name, oUser.IP);
                Listener.Rooms.Find(e => e.id == 0).UserList.Add(oUser);
            } else
                goto begin;
             
            byte[] ByteLength = new byte[4];
            byte[] message = new byte[120];
            while (true)
            {
                Array.Clear(message, 0, message.Length);

                try
                {
                    Int32 byt = stream.Read(ByteLength, 0, 4);
                    String c = System.Text.Encoding.ASCII.GetString(ByteLength, 0, 4);
                    switch (c.Substring(0, c.IndexOf("?", 0, 4)))
                    {
                        case "CRC":
                           new UserCommands().CreateRoom(c,stream,oUser);
                            break;
                        case "URC":
                            new UserCommands().ChangeRoom(c, stream, oUser);
                            break;
                        default:
                            new UserCommands().ReplyMSG(c, stream, oUser);
                            break;
                    }
                }
                catch (IOException e)
                {
                    GlobalMessage.UserDisconnected(oUser);
                    break;
                }
                catch (ObjectDisposedException)
                {
                    GlobalMessage.UserDisconnected(oUser);
                    break;
                }
                catch (System.ArgumentOutOfRangeException) { }
            }
        }
      
            
               
            }
        }
   

