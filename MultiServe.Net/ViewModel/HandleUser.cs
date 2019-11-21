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
        public static User oUser = null;
        public static void HandleUserStart(object obj)
        {
            TcpClient user = (TcpClient)obj;
            var stream = user.GetStream();
            byte[] info = new Byte[100];
            bool done = false;
            while (!done)
            {
                try {
                    stream.Read(info, 0, info.Length);
                    var UserInfo = System.Text.Encoding.ASCII.GetString(info);


                    switch (UserInfo.Substring(0, UserInfo.IndexOf("?", 0, 4)))
                    {
                        case "REG":
                            done = new UserCommands().Register(stream, UserInfo);
                            goto End;
                            break;
                        case "LOG":
                            done = new UserCommands().Login(stream, UserInfo, oUser, user);
                            break;
                    }
                } catch (System.IO.IOException e) { }
                catch (System.ObjectDisposedException e) { goto End; Console.WriteLine(e); }
            }
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
                            new UserCommands().CreateRoom(stream, oUser);
                            break;
                        case "URC":
                            new UserCommands().ChangeRoom(stream, oUser);
                            break;
                        default:
                            new UserCommands().ReplyMSG(c, stream, oUser);
                            break;
                    }
                }
                catch (IOException e )
                {
                    Console.WriteLine(e);
                    GlobalMessage.UserDisconnected(oUser);
                    break;
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine(e);
                    GlobalMessage.UserDisconnected(oUser);
                    break;
                }
                catch (System.ArgumentOutOfRangeException e) { Console.WriteLine(e); }
            }
        End:;
        }
            }



        }
    
        
   

