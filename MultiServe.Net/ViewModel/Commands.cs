﻿using MultiClientServer.Model;
using MultiServe.Net.Model;
using MultiServe.Net.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiClientServer.ViewModel
{
    class Commands
    {
        string Person;
        User G;
        public async void SetCommandAsync(string Command, string person)
        {
            Task.Factory.StartNew(() => { new Logs().saveLogs(person + " : " + Command); });
            Person = person;
            Command += " ";
            var g = Listener.usersList.Find(e => e.Name.Equals(person));
            G = g;
            switch (Command.Substring(0, Command.IndexOf(' ', 0, Command.Length)) )
            {
                case "say":
                    
                    var d5 = Command.IndexOf(' ', 0, Command.Length);
                    string message = Command.Substring(d5);
                    GlobalMessage.ServerMessage( message);
                    break;
                case "admin":
                    var d = Command.IndexOf(' ', 0, Command.Length);

                    int id;
                    int.TryParse(Command.Substring(d),out id);
                    var gu = Listener.usersList.Find(e => e.Id==id);
                    if(G!=null) {
                        
                        var json = JsonConvert.SerializeObject(new Msg_Info() { From = "info", Message = "Permission Granted" });
                        G.SendMessage("MSG?" , json );
                    }
                    sendAdminInfo( new DBConnect(Listener.config).rankAdmin(id));
                    break;
                case "de-admin":
                    var de = Command.IndexOf(' ', 0, Command.Length);

                    int ide;
                    int.TryParse(Command.Substring(de), out id);
                    var ge = Listener.usersList.Find(e => e.Id == id);
                    if (ge != null)
                    {
                        var json = JsonConvert.SerializeObject(new Msg_Info() { From = "info", Message = "Permission Lost" });
                        G.SendMessage("MSG?" , json);
                    }
                    sendAdminInfo(new DBConnect(Listener.config).rankNormal(id));
                    break;

                case "ban":
                    if (Command.Length > 5)
                    {
                        try
                        {
                            var d1 = Command.IndexOf(' ', 2);
                            var d22 = Command.IndexOf(' ', d1 + 1);
                            string id1 = Command.Substring(d1, d22 - d1);
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
                case "clear":
                    if (person == "Server") { Console.Clear(); }
                   
                    
                    break;
                case "d-room":
                    
                    if (Command.Length > 5)
                    {
                        try
                        {
                            var d1 = Command.IndexOf(' ', 0, Command.Length);
                            var d22 = Command.IndexOf(' ', d1);
                            string id1 = Command.Substring(d1+1);
                            var c = Listener.Rooms.Find(e => e.id == int.Parse(id1));
                            if (c.name != "Main")
                            {
                                foreach (var item in c.UserList)
                                {
                                    item.SetRoomID(0);
                                    Listener.Rooms.Find(e => e.id == 0).UserList.Add(item);
                                    var json = JsonConvert.SerializeObject(new Msg_Info() { From = "info", Message = "Your channel has been deleted you are chatting at: Main" });
                                    G.SendMessage("MSG?" , json );  
                                }
                                GlobalMessage.ServerMessage("Room " + c.name + " has been deleted by: " + person);
                                Listener.Rooms.Remove(c);
                                GlobalMessage.SendRoomList();
                            }
                            else
                            {
                                if (person != "Server")
                                    sendAdminInfo("You can not delete main you sneaky bastard");
                                else Console.WriteLine("You can not delete main you sneaky bastard");
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("d-room ID");
                        }catch(System.NullReferenceException)
                        {
                            Console.WriteLine("Wrong ID");
                        }
                    }
                    else
                    {
                        Console.WriteLine("d-room ID");
                    }
                    break;
                case "kick":
                    if (Command.Length > 5)
                    {
                        try
                        {
                            var d11 = Command.IndexOf(' ', 4);
                            var d222 = Command.IndexOf(' ', d11+1);
                            string id11 = Command.Substring(d11, d222 - d11);
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
                    if (person == "Server")
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
                case "room-list":
                    if (person == "Server")
                    {
                        foreach (var user in Listener.Rooms)
                        {
                            Console.WriteLine($"\r\n ID: {user.id} NAME: {user.name} USERS: {user.UserList.Count}");
                        }
                    }
                    else
                    {
                        string send = "";
                        foreach (var user in Listener.Rooms)
                        {
                            send += ($"\r\n ID: {user.id} NAME: {user.name} USERS: {user.UserList.Count}");
                        }
                        sendAdminInfo(send);
                    }
                    break;
                case "help":
                    if (person == "Server")
                    {
                        Console.WriteLine("list - show all users online and their id");
                        Console.WriteLine("kick ID REASON - kicks user and tells why");
                        Console.WriteLine("ban ID REASON - kicks user and tells why");
                        Console.WriteLine("say MESSAGE - say somtething as server");
                        Console.WriteLine("unban ID - unban user");
                        Console.WriteLine("admin ID - grant admin permissions");
                        Console.WriteLine("de-admin ID - delete admin permissions ");
                        Console.WriteLine("room-list - list of rooms NAME ID and Users");
                        Console.WriteLine("d-room ID - list of rooms NAME ID and Users");
                        Console.WriteLine("clear - clear server Console");
                    }
                    else
                    {
                        sendAdminInfo("list - show all users online and their id \n\r" +
                        "kick ID REASON - kicks user and tells why \n\r" +
                        "ban ID REASON - kicks user and tells why \n\r" +
                        "unban ID - unban user \n\r" +
                       "say MESSAGE - say somtething as server \n\r" +
                       "admin ID - grant admin permissions \n\r" +
                        "de-admin ID - delete admin permissions \n\r" +
                        "room-list - list of rooms NAME ID and Users\n\r" +
                        "d-room ID - Delete room\n\r"
                        ) ;
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
        void KickUser(string id, string reason)
        {
            var isnumber=false;
            if (isnumber = int.TryParse(id, out int idn))
            {
                try
                {
                   var kUser =  Listener.usersList.Find(u => u.Id.ToString().Contains(id.Trim()));
                    GlobalMessage.ServerMessage(kUser.Name + " Have Been Kicked By: " + Person + " Reason: " + reason);
                    GlobalMessage.UserDisconnected(kUser);
                    kUser.Stream.Close();
                    Console.WriteLine(kUser.Name + " Have Been Kicked By: " + Person + " Reason: " + reason);
                }
                catch (NullReferenceException ) { Console.WriteLine("Wrong id"); }
            }
            else Console.WriteLine("kick userid reason");
        } void BanUser(string id, string reason)
        {
            var isnumber=false;
            if (isnumber = int.TryParse(id, out int idn))
            {
                try
                {

                   var kUser =  Listener.usersList.Find(u => u.Id.ToString().Contains(id.Trim()));
                    new DBConnect(Listener.config).BanUser(kUser.Id);
                    GlobalMessage.ServerMessage(kUser.Name + " Have Been Banned By: " + Person + " Reason: " + reason);
                    GlobalMessage.UserDisconnected(kUser);
                    kUser.Stream.Close();
                    Console.WriteLine(kUser.Name + " Have Been Banned By: " + Person + "Reason: " + reason );
                }
                catch (NullReferenceException ) { Console.WriteLine("Wrong id"); }
            }
            else Console.WriteLine("ban userid reason");
        }
        void unBanUser(string id)
        {
            var isnumber = false;
            if (isnumber = int.TryParse(id, out int idn))
            {
                try { 
                    new DBConnect(Listener.config).unBanUser(idn);
                    Console.WriteLine(idn + " Have Been unBanned By: " + Person);
                        sendAdminInfo(idn + " Have Been unBanned");
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
                G.SendMessage("MSG?" , json );
            }
        }
    }
}