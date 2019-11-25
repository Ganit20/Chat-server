using MultiClientServer.Model;
using MultiClientServer.ViewModel;
using MultiServe.Net.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiServe.Net.Model
{
    class Room_info
    {
        public string name { get; set; }
        public User RoomCreator { get; set; }
        public string RoomCreatorName { get; set; }
        public string NewRoom {get; set;}
        public int id { get; set; }
        public bool isPassword { get; set; }
        public string password { get; set; }
        public List<User> UserList = new List<User>();

        public void SendRoom(string prefix,string msg)
        {
            try{
                foreach (var user in UserList)
                {
                    user.SendMessage(prefix,msg);
                } }catch (System.InvalidOperationException) { }

        }
        public void SendRoomList(byte[] msg)
        {
            try { 
            foreach (var user in UserList)
            {
                user.RoomCreate(msg);
            }
        }catch (System.InvalidOperationException) { }

}
       public void UserListSend(byte[] msg)
        {
            try
            {
                foreach (var user in UserList)
                {
                    user.Userlist(msg);
                }

            }
            catch (System.InvalidOperationException) { }
        }

public void Create(string name, User creator,bool ispassword, string password)
        {
            try
            {
                Room_info roomt = new Room_info()
                { name = name, RoomCreator = creator, id = Listener.roomid, isPassword = ispassword, password = password };
                if (!Listener.Rooms.Exists(e =>e.name.Equals(roomt.name)))
                {
                    Listener.Rooms.Add(roomt);
                    Listener.roomid++;
                    Console.Write("Room: " + name + " created by: " + creator.Name + "\r\n");
                    Task.Factory.StartNew(() => { new Logs().saveLogs("Room: " + name + " created by: " + creator.Name); });
                    GlobalMessage.SendRoomList();
                }else
                {
                    var msg = new Msg_Info()
                    {
                        From = "SERVER:",
                        Message = "Name is already taken ",
                        MsgTime = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()
                    };
                    var msgJson = JsonConvert.SerializeObject(msg);
                    var c = new TextOperations().MessageLength( msgJson);
                    creator.SendMessage("MSG?",c);
                }
            }catch(System.ArgumentException)
            {
                var msg = new Msg_Info()
                {
                    From = "SERVER:",
                    Message = "Name is already taken ",
                    MsgTime = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString()
                };
                var msgJson = JsonConvert.SerializeObject(msg);
                var c = new TextOperations().MessageLength(msgJson);
                creator.SendMessage("MSG?",c);
            }
        }
    public void Check(int Roomid)
        {
            if(UserList.Count==0 && Roomid !=0)
            {
                var thisRoom = Listener.Rooms.Find(e => e.id == Roomid);
                Listener.Rooms.Remove(thisRoom);
                GlobalMessage.SendRoomList();
            }
        }
    }
}
