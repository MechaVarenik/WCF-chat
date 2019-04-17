using System.ServiceModel;
using System.Collections.Generic;


namespace WCF_chat
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IServiceChat" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServerChatCallback))]
    public interface IServiceChat
    {
        [OperationContract]
        int Connect(string name);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int id);

        [OperationContract(IsOneWay = true)]
        void SendOnlineUsers(string msg, int id);

        [OperationContract]
        bool InsertUser(string name, string password);

        [OperationContract]
        bool Check_User(string name, string password);

        [OperationContract]
        List<string> GetOnlineUsers();
    }

    public interface IServerChatCallback
    {
        [OperationContract (IsOneWay = true)]
        void MsgCallback(string msg);

        [OperationContract(IsOneWay = true)]
        void UsersCallback(string name);
    }
}