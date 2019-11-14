using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiClientServer.ViewModel
{
    class Commands
    {
        public void SetCommand(String Command)
        {
            Command += " ";
            switch (Command.Substring(0, Command.IndexOf(' ', 0, Command.Length)) )
            {
                case "say":
                    var d5 = Command.IndexOf(' ', 0, Command.Length);
                    String message = Command.Substring(d5);
                    GlobalMessage.ServerMessage(message);
                    break;

                case "kick":
                    if (Command.Length > 5)
                    {
                        try
                        {
                            var d = Command.IndexOf(' ', 0, Command.Length);
                            var d2 = Command.IndexOf(' ', d, Command.Length - d);
                            String id = Command.Substring(d, d2 - 1);
                            String reason = Command.Substring(d2);
                            KickUser(id, reason);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("kick ID REASON");
                        }
                    }
                    else
                    {
                        Console.WriteLine("kick ID REASON");
                    }
                    break;
                case "list":
                    foreach (var user in Listener.usersList)
                    {
                        Console.WriteLine($"\r\n ID: {user.Id} NAME: {user.Name} IP: {user.IP} Room ID: {user.RoomID}");
                    }
                    break;
                case "help":
                    Console.WriteLine("list - show all users online and their id");
                    Console.WriteLine("kick ID REASON - kicks user and tells why");
                    Console.WriteLine("say MESSAGE - say somtething as serwer");
                    break;
                default:
                    Console.WriteLine("No command like " + Command + " Type help for more information");
                    break;

            }
            
        }
        void KickUser(String id, String reason)
        {
            var isnumber=false;
            if (isnumber = int.TryParse(id, out int idn))
            {
                try
                {
                   var kUser =  Listener.usersList.Find(u => u.Id.ToString().Contains(id.Trim()));
                    GlobalMessage.ServerMessage(kUser.Name + " Have Been Kicked reason:");
                    GlobalMessage.UserDisconnected(kUser);
                    kUser.Stream.Close();
                    Console.WriteLine(kUser.Name + " Have Been Kicked Reason: " + reason);
                }
                catch (NullReferenceException e) { Console.WriteLine("Wrong id"); }
            }
            else Console.WriteLine("kick userid reason");
        }
    }
}