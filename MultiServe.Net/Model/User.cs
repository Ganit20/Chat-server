using App3;
using MultiClientServer.Model;
using MultiClientServer.ViewModel;
using MultiServe.Net.ViewModel;
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
        public string password;
        public string email;
        public string Rank;
        public int banned;
        public string bannedFor;
        public int RoomID;
        public NetworkStream Stream;
        public TcpClient Tcp;
        public User(int id,string name,NetworkStream stream, string ip,int roomid,TcpClient tcp, string pass,string Email,string rank, int ban,string bannedFr)
        {
             Id = id;
             Name = name;
             Stream = stream;
             IP = ip;
             RoomID = roomid;
             Tcp = tcp;
             password = pass;
             email = Email;
            banned = ban;
            bannedFor = bannedFr;
            Rank = rank;
        }
        public string GetName()
        {
            return Name;
        }
        public int GetRoomID ()
        {
            return RoomID;
        }
        public void SetRoomID(int Room)
        {
            RoomID = Room;
        }
        public void DisconnectUser()
        {
            Stream.Close();
            Tcp.Close();
        }
        public void Userlist(byte[] reply) {
            try
            {
                    
                    Stream.Write(reply, 0, reply.Length);
                    Array.Clear(reply, 0, reply.Length);
            }
            catch (System.IO.IOException)
            {

            }
            catch (System.ObjectDisposedException) { }
        }
        public void RoomCreate(byte[] message)
        {
            try { 
           
            Stream.Write(message, 0, message.Length);
        }catch(ObjectDisposedException){ }
            catch (System.IO.IOException) { }
        }
        public void SendMessage(string prefix , string Message)
        {
                try
                {
                    string data = null;
                    byte[] reply;
                    data = Message;
                
                reply = new Encryption().Encrypt(prefix + data);
                var replylength = new TextOperations().byteLength(reply.Length.ToString()) ;
                byte[] k = System.Text.Encoding.ASCII.GetBytes(replylength);
                var msg = new TextOperations().addBytes(k, reply);
                    Stream.Write(msg, 0, msg.Length);
                }
                catch (ObjectDisposedException) {   }
                catch (System.IO.IOException) { GlobalMessage.UserDisconnected(this); }
          
        }
    }
}
