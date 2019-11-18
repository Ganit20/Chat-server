using MultiClientServer.ViewModel;
using MultiServe.Net.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiServe.Net.ViewModel
{
    class TextOperations
    {
        public Room_info ReadRoom_info(NetworkStream Stream,byte[] message)
        {
            Int32 p = Stream.Read(message, 0, message.Length);
            var b = System.Text.Encoding.ASCII.GetString(message, 0, message.Length);
            var o = JsonConvert.DeserializeObject<Room_info>(b);
            return o;
        }
        public void RoomChanger(User changer,Room_info OldRoom, Room_info NewRoom)
        {
            OldRoom.UserList.Remove(changer);
            NewRoom.UserList.Add(changer);
            changer.setRoomID(NewRoom.id);
            GlobalMessage.SendUserList();
            new Room_info().Check(OldRoom.id);
            changer.SendMessage("SSG?You are chatting at: " + NewRoom.name + "?END");
            new Logs().saveLogs(DateTime.UtcNow + changer.Name + " Changed room to " + NewRoom.name);
        }
        public string MessageLength(string leng)
        {
            var length = leng.Length+ "?" +leng;
            while (length.IndexOf("?")!= 3)
            {
                length = "0" + length;
            }
            return length;
        }
    }
}
