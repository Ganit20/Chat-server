﻿using MultiClientServer.ViewModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MultiServe.Net.Model
{
    class DBConnect
    {
        MySqlConnection connection;
        string server;
        string database;
        string uid;
        string password;

        public DBConnect(Dictionary<string,string> db)
        {
            try
            {
                server = db["DB-server"];
                database = db["DB-name"];
                uid = db["DB-User-id"];
                password = db["DB-Password"];
                string connectionString;
                connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
                connection = new MySqlConnection(connectionString);
                
                OpenConnection();
               
                string sqlcheck = "SELECT EXISTS(" +
                    "SELECT  `TABLE_NAME` " +
                    "FROM `INFORMATION_SCHEMA`.`TABLES` " +
                    "WHERE (`TABLE_NAME` = 'user') AND (`TABLE_SCHEMA` = '"+database+"')) " +
                    "as `is -exists`; ";
                MySqlCommand CheckTable = new MySqlCommand(sqlcheck, connection);
                object CheckRead = CheckTable.ExecuteScalar();
                int i = Convert.ToInt32(CheckRead);
                if(i==0)
                {
                    Console.WriteLine("MySQL version : {0}", connection.ServerVersion);
                    Console.WriteLine("Creating Database Table");
                    CreateDatabase();
                }
}               
            catch (KeyNotFoundException ) {
                Console.WriteLine("Open server.config and cofigure it then start again. \n\r Press any key to continue.");
                Console.ReadKey();
                Environment.Exit(1); }
           
        }
        public string rankNormal(int id)
        {
            string change = "Update USER SET p_rank = 'Normal' WHERE id = " + id + ";";
            MySqlCommand createC = new MySqlCommand(change, connection);
            createC.ExecuteNonQuery();
            return "Done";
        }
        public string rankAdmin(int id)
        {
            string change = "Update USER SET p_rank = 'admin' WHERE id = " + id + ";";
           MySqlCommand createC = new MySqlCommand(change, connection);
            createC.ExecuteNonQuery();
            return "Done";
        }
        void CreateDatabase()
        {
            string create =
                "USE " + database +

                   ";CREATE TABLE USER(" +
                   "id smallint unsigned not null auto_increment ,  " +
                   "name varchar(20) not null UNIQUE," +
                   "password varchar(20) not null , " +
                   "email varchar(40) not null UNIQUE, " +
                   "p_rank varchar(10) not null DEFAULT 'Normal', " +
                   "Banned int(1) not null DEFAULT 0," +
                   "BANNEDFOR VARCHAR(120) not null DEFAULT 'no'," +
                   "constraint pk_example primary key(id));" ;
            MySqlCommand createC = new MySqlCommand(create, connection);
            createC.ExecuteNonQuery();
            Console.WriteLine("Done");
        }
        bool OpenConnection()
        {
            try
            {
                connection.Open();
                
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Mysql Connect Error");
                Console.WriteLine("You need installed mysql server. You can configure it in server.config" );
                Task.Factory.StartNew(() => { new Logs().saveLogs(e.ToString()); });
               
                return false;
            }
        }
       public bool UserRegister(string name, string password, string email)
        {
            try
            {
                string register = "INSERT USER(name, password, email, Banned) values('" + name + "', '" + password + "', '" + email + "', 0);";
                MySqlCommand createC = new MySqlCommand(register, connection);
                createC.ExecuteNonQuery();
                Console.WriteLine("User registered " + name);
                return true;
            }catch(MySql.Data.MySqlClient.MySqlException)
            {
                return false;
            }
        }
        public bool UserLogin(string name, string password)
        {   
            string login = "SELECT password FROM USER WHERE name='"+name+"';";
            MySqlCommand loginC = new MySqlCommand(login, connection);
            object result = loginC.ExecuteScalar();
            if (Convert.ToString(result).Equals(password))
            {
                
                return true;

            }
            else return false;
        }
        public User DownloadUserInfo(string name,NetworkStream stream,TcpClient tcp)
        {
            try { 
            
                    string sql = "SELECT * FROM USER WHERE name='" + name + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    rdr.Read();
                    var IPA = ((IPEndPoint)tcp.Client.RemoteEndPoint).Address.ToString();
                    User oUser = new User(rdr.GetInt32("id"), rdr.GetString("name"), stream, IPA, 0, tcp, rdr.GetString("password"), rdr.GetString("email"), rdr.GetString("p_rank"), rdr.GetInt32("Banned"), rdr.GetString("BANNEDFOR"));
                    Listener.usersList.Add(oUser);
                    GlobalMessage.UserJoined(oUser.Name, oUser.IP);
                    Listener.Rooms.Find(e => e.id == 0).UserList.Add(oUser);
                    return oUser;
                
            }catch(MySqlException e)
            {
                Console.WriteLine(e);
                return new User(0, null, null, null, 0, null, null, null, null, 0, null);
            }
        }
        public void BanUser(int id)
        {
                string register = "Update USER SET Banned = 1 WHERE id="+id +";";
                MySqlCommand createC = new MySqlCommand(register, connection);
                createC.ExecuteNonQuery();
                Console.WriteLine("User "+ id +" banned");
            }
        public void unBanUser(int id)
        {
            string register = "Update USER SET Banned = 0 WHERE id=" + id + ";";
            MySqlCommand createC = new MySqlCommand(register, connection);
            createC.ExecuteNonQuery();
            Console.WriteLine("User " + id + " unBanned");
        }
    }
}
