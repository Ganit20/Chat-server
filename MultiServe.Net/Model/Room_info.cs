using MultiClientServer.ViewModel;
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

        public void SendRoom(string msg)
        {
            try{
                foreach (var user in UserList)
                {
                    user.SendMessage(msg);
                } }catch (System.InvalidOperationException) { }

        }
        public void SendRoomList(string msg)
        {
            try { 
            foreach (var user in UserList)
            {
                user.RoomCreate(msg);
            }
        }catch (System.InvalidOperationException) { }

}
        public void UserListSend(string msg)
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
            Room_info roomt = new Room_info()
            { name = name, RoomCreator = creator, id = Listener.roomid,isPassword=ispassword,password=password };
            Listener.Rooms.Add(roomt);
            Listener.roomid++;
            Console.Write("Room: " + name + " created by: " +creator.Name + "\r\n");
            new Logs().saveLogs("Room: " + name + " created by: " + creator.Name);
            GlobalMessage.sendRoomList();
        }
    public void Check(int Roomid)
        {
            if(UserList.Count==0 && Roomid !=0)
            {
                var thisRoom = Listener.Rooms.Find(e => e.id == Roomid);
                Listener.Rooms.Remove(thisRoom);
                GlobalMessage.sendRoomList();
            }
        }
    }
}
