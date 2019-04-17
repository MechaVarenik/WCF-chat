using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WCF_chat
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
       public class ServiceChat: IServiceChat
    {
        List<ServerUser> users = new List <ServerUser>();
        List<string> names = new List<string>();
        int nextId = 1;

        public int Connect(string name)
        {
            ServerUser user = new ServerUser()
            {
                Id = nextId,
                ServerUserName = name,
                operationContext = OperationContext.Current
            };
            nextId++;
            SendMsg(": " + user.ServerUserName + " подключился к чату!", 0);
            users.Add(user);
            names.Add(user.ServerUserName);
            return user.Id;
            }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.Id == id);
            if (user != null)
            {
                string discUser = user.ServerUserName;
                users.Remove(user);
                names.Remove(user.ServerUserName);
                SendMsg(": " + user.ServerUserName + " покинул чат!", 0);
            }

        }

        public void SendMsg(string msg, int id)
        {
           foreach(var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();

                var user = users.FirstOrDefault(i => i.Id == id);
                if (user != null)
                {
                    answer += " " + user.ServerUserName + ":" + " ";
                }
                answer += msg;
               item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }

        }

        public void SendOnlineUsers(string msg, int id)
        {
            foreach (var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();

                var user = users.FirstOrDefault(i => i.Id == id);
                if (user != null)
                {
                 item.operationContext.GetCallbackChannel<IServerChatCallback>().UsersCallback(user.ServerUserName);
                }
               
            }

        }

        public bool InsertUser(string name, string password)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    var nm = (from p in db.Users
                                where p.Name == name
                                select p).FirstOrDefault();
                    if (nm != null)
                    {
                        throw new Exception("This nickname already exists");
                    }
                    User us = new User();
                    us.Name = name;
                    us.Password = password;
                    db.Users.Add(us);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return false;
        }

        public bool Check_User(string name, string password)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    var user = (from p in db.Users
                                where p.Name == name && p.Password == password
                                select p).FirstOrDefault();
                    if (user == null)
                    {
                        throw new Exception("User does not exist");
                    }
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return false;
        }

        public List<string> GetOnlineUsers()
        {
            return names;
        }
    }
    [DataContract]
    public class User
    {
        [DataMember]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Password { get; set; }
    }

    public class ServerUser
    {
        static List<ServerUser> users = new List<ServerUser>();

        public int Id { get; set; }
        public string ServerUserName { get; set; }
        public OperationContext operationContext { get; set; }

    }
}
