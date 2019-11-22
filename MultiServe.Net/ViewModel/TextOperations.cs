using App3;
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
        public Room_info ReadRoom_info(NetworkStream Stream, string info)
        {
            byte[] bytes = new byte[LengthStream(info)];
            Int32 leng = Stream.Read(bytes, 0, bytes.Length);
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
            var messag = new Encryption().Decrypt(bytes);
            var UserJson = JsonConvert.DeserializeObject<Room_info>(messag);
            return UserJson;
        }
        public void RoomChanger(User changer, Room_info OldRoom, Room_info NewRoom)
        {
            OldRoom.UserList.Remove(changer);
            NewRoom.UserList.Add(changer);
            changer.SetRoomID(NewRoom.id);
            GlobalMessage.SendUserList();
            new Room_info().Check(OldRoom.id);
            changer.SendMessage("SSG?","You are chatting at: " + NewRoom.name );
            new Logs().saveLogs(DateTime.UtcNow + changer.Name + " Changed room to " + NewRoom.name);
        }
        public string byteLength(string leng)
        {
            var length = leng + "?" ;
            while (length.IndexOf("?") != 3)
            {
                length = "0" + length;
            }
            return length;
        }
        public string MessageLength(string leng)
        {
            var length = leng.Length + "?" + leng;
            while (length.IndexOf("?") != 3)
            {
                length = "0" + length;
            }
            return length;
        }

        public int LengthStream(string info)
        {
            byte[] ByteLength = new byte[4];
            string d = info.Substring(info.IndexOf('?') + 1, info.LastIndexOf('?') - info.IndexOf('?') - 1);
            int bl = int.Parse(d);
            return bl;
        }
        public byte[] addBytes(byte[] a1, byte[] a2)
        {
            byte[] newArray = new byte[a1.Length + a2.Length];
            System.Buffer.BlockCopy(a1, 0, newArray, 0, a1.Length);
            System.Buffer.BlockCopy(a2, 0, newArray, a1.Length, a2.Length);
            return newArray;
        }
    }
}
