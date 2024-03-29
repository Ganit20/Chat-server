﻿using App3;
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
            byte[] info = new Byte[4];
            bool done = false;
            while (!done)
            {
                try {
                    stream.Read(info, 0, 4);
                    var UserInfo = Encoding.UTF8.GetString(info);
                    string d = UserInfo.Substring(0,3);
                    int bl = int.Parse(d);
                    byte[] bytes = new byte[bl];
                    Int32 leng = stream.Read(bytes, 0, bl);
                    string fumsg = new Encryption().Decrypt(bytes);
                    switch (fumsg.Substring(0,fumsg.IndexOf('?')))
                    {
                        case "REG":
                            done = new UserCommands().Register(stream, fumsg.Substring(4));
                            goto End;
                            break;
                        case "LOG":
                            done = new UserCommands().Login(stream, fumsg.Substring(4), oUser, user) ;
                            break;
                    }
                } catch (System.IO.IOException e) { goto End; }
                catch (System.ObjectDisposedException e) { goto End; }
                catch(ArgumentOutOfRangeException ) { goto End; }
            }
            byte[] ByteLength = new byte[8];
            byte[] message = new byte[120];
            while (true)
            {
                Array.Clear(message, 0, message.Length);

                try
                {
                    Int32 byt = stream.Read(ByteLength, 0, 8);
                    string c = System.Text.Encoding.UTF8.GetString(ByteLength, 0, 8);
                    switch (c.Substring(0, c.IndexOf("?", 0, 4)))
                    {
                        case "CRC":
                            new UserCommands().CreateRoom(stream, oUser,c);
                            break;
                        case "URC":
                            new UserCommands().ChangeRoom(stream, oUser,c);
                            break;
                        case "MSG":
                            new UserCommands().ReplyMSG(c, stream, oUser);
                            break;
                    }
                }
                catch (IOException e )
                {
                 
                    GlobalMessage.UserDisconnected(oUser);
                    break;
                }
                catch (ObjectDisposedException e)
                {
                    
                    GlobalMessage.UserDisconnected(oUser);
                    break;
                }
                catch (System.ArgumentOutOfRangeException e) { GlobalMessage.UserDisconnected(oUser);
                    break; }
            }
        End:;
        }
            }



        }
    
        
   

