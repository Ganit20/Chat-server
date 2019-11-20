using MultiClientServer.Model;
using MultiServe.Net.Model;
using Newtonsoft.Json;
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
        string Person;
        User G;
        public async void SetCommandAsync(string Command,string person)
        {
            Person = person;
            Command += " ";
            var g = Listener.usersList.Find(e => e.Name.Equals(person));
            G = g;
            switch (Command.Substring(0, Command.IndexOf(' ', 0, Command.Length)) )
            {
                case "say":
                    new Logs().saveLogs(Command);
                    var d5 = Command.IndexOf(' ', 0, Command.Length);
                    String message = Command.Substring(d5);
                    GlobalMessage.ServerMessage( message);
                    break;
                case "admin":
                    var d = Command.IndexOf(' ', 0, Command.Length);

                    int id;
                    int.TryParse(Command.Substring(d),out id);
                    
                    sendAdminInfo( new DBConnect(Listener.config).rankAdmin(id));
                    break;
                    
                case "ban":
                    new Logs().saveLogs(Command);
                    if (Command.Length > 5)
                    {
                        try
                        {
                            var d1 = Command.IndexOf(' ', 0, Command.Length);
                            var d22 = Command.IndexOf(' ', d1, Command.Length - d1);
                            string id1 = Command.Substring(d1, d22 - 1);
                            string reason = Command.Substring(d22);
                            BanUser(id1, reason);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("ban ID REASON");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ban ID REASON");
                    }
                    break;
                case "unban":
                    new Logs().saveLogs(Command);
                    if (Command.Length > 5)
                    {
                        try
                        {
                            var d1 = Command.IndexOf(' ', 0, Command.Length);
                            var d22 = Command.IndexOf(' ', d1, Command.Length - d1);
                            string id1 = Command.Substring(d1, d22 - 1);
                            string reason = Command.Substring(d22);
                            unBanUser(id1);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("unban ID ");
                        }
                    }
                    else
                    {
                        Console.WriteLine("unban ID ");
                    }
                    break;
                case "kick":
                    new Logs().saveLogs(Command);
                    if (Command.Length > 5)
                    {
                        try
                        {
                            var d11 = Command.IndexOf(' ', 0, Command.Length);
                            var d222 = Command.IndexOf(' ', d11, Command.Length - d11);
                            string id11 = Command.Substring(d11, d222 - 1);
                            string reason = Command.Substring(d222);
                            KickUser(id11, reason);
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
                    if (person == "server")
                    {
                        foreach (var user in Listener.usersList)
                        {
                            Console.WriteLine($"\r\n ID: {user.Id} NAME: {user.Name} IP: {user.Rank} Room ID: {user.RoomID}");
                        }
                    }
                    else
                    {
                        string send = "";
                        foreach (var user in Listener.usersList)
                        {
                            send+=($"\r\n ID: {user.Id} NAME: {user.Name} IP: {user.Rank} Room ID: {user.RoomID}\n\r" );
                        }
                        sendAdminInfo(send);
                    }
                            break;
                case "help":
                    if (person == "server")
                    {
                        Console.WriteLine("list - show all users online and their id");
                        Console.WriteLine("kick ID REASON - kicks user and tells why");
                        Console.WriteLine("ban ID REASON - kicks user and tells why");
                        Console.WriteLine("say MESSAGE - say somtething as serwer");
                    }
                    else
                    {
                        sendAdminInfo("list - show all users online and their id \n\r" +
                        "kick ID REASON - kicks user and tells why \n\r" +
                        "ban ID REASON - kicks user and tells why \n\r" +
                       "say MESSAGE - say somtething as serwer \n\r");
                    }
                    break;
                default:
                    if (G == null)
                    {
                        Console.WriteLine("No command like " + Command + " Type help for more information");
                    }else
                    {
                        sendAdminInfo("No command like " + Command + " Type help for more information");
                    }
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
                    GlobalMessage.ServerMessage(kUser.Name + " Have Been Kicked By: " + Person + " reason: " + reason);
                    GlobalMessage.UserDisconnected(kUser);
                    kUser.Stream.Close();
                    Console.WriteLine(kUser.Name + " Have Been Kicked By: " + Person + " Reason: " + reason);
                }
                catch (NullReferenceException ) { Console.WriteLine("Wrong id"); }
            }
            else Console.WriteLine("kick userid reason");
        } void BanUser(String id, String reason)
        {
            var isnumber=false;
            if (isnumber = int.TryParse(id, out int idn))
            {
                try
                {

                   var kUser =  Listener.usersList.Find(u => u.Id.ToString().Contains(id.Trim()));
                    new DBConnect(Listener.config).BanUser(kUser.Id);
                    GlobalMessage.ServerMessage(kUser.Name + " Have Been Banned By: " + Person + "Reason: " + reason);
                    GlobalMessage.UserDisconnected(kUser);
                    kUser.Stream.Close();
                    Console.WriteLine(kUser.Name + " Have Been Banned By: " + Person + "Reason: " + reason );
                }
                catch (NullReferenceException ) { Console.WriteLine("Wrong id"); }
            }
            else Console.WriteLine("ban userid reason");
        }
        void unBanUser(String id)
        {
            var isnumber = false;
            if (isnumber = int.TryParse(id, out int idn))
            {
                try { 
                    new DBConnect(Listener.config).unBanUser(idn);
                    Console.WriteLine(idn + " Have Been unBanned By: " + Person );
                }
                catch (NullReferenceException) { Console.WriteLine("Wrong id"); }
            }
            else Console.WriteLine("ban userid reason");
        }
        void sendAdminInfo(string info)
        {
            if (G == null)
            {
                Console.WriteLine(info);
            }
            else
            {
                var send = info;
                var json = JsonConvert.SerializeObject(new Msg_Info() { From = "info", Message = send });
                G.SendMessage("MSG?" + json + "?END");
            }
        }
    }
}