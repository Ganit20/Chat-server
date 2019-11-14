using MultiClientServer.Model;
using MultiClientServer.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiServe.Net.Model
{
    public class User
    {
        public int Id;
        public string Name;
        public string IP;
        public int RoomID;
        public NetworkStream Stream;
        public TcpClient Tcp;
        public User(int id,string name,NetworkStream stream, string ip,int roomid,TcpClient tcp)
        {
             Id = id;
             Name = name;
             Stream = stream;
             IP = ip;
             RoomID = roomid;
             Tcp = tcp;
        }
        public string getName()
        {
            return Name;
        }
        public int getRoomID()
        {
            return RoomID;
        }
        public void setRoomID(int Room)
        {
            RoomID = Room;
        }
        public void DisconnectUser()
        {
            Stream.Close();
            Tcp.Close();
        }
        public void Userlist(string message) {
            try
            {
      
                    var reply = System.Text.Encoding.ASCII.GetBytes(message);
                    Stream.Write(reply, 0, reply.Length);
                    Array.Clear(reply, 0, reply.Length);
           
            }
            catch (System.IO.IOException)
            {

            }
            catch (System.ObjectDisposedException) { }
        }
        public void RoomCreate(string message)
        {try { 
            var reply = System.Text.Encoding.ASCII.GetBytes(message);
            Stream.Write(reply, 0, reply.Length);
            
        }catch(ObjectDisposedException)
            {

            }catch(System.IO.IOException)
            {

            }
        }
        public void SendMessage(string Message)
        {

                try
                {
                    String data = null;
                    Byte[] reply;
                    data = Message;
                    String msgleng = data.Length.ToString();
                    data = msgleng + data;
                    while (data.IndexOf('?', 0, data.Length) < 3)
                    {
                        data = "0" + data;
                    }
                    reply = System.Text.Encoding.ASCII.GetBytes(data);

                    Stream.Write(reply, 0, reply.Length);

                }
                catch (ObjectDisposedException)
                {

                }
                catch (System.IO.IOException)
                {

                }
          
        }
        //public void userDisconnect()
        //{
        //    Listener.usersList.Remove(this);
        //    var a = Listener.Rooms.Find(e => e.id == RoomID);
        //    a.UserList.Remove(this);
        
           
        //}
    }
}
