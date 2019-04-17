using System.ServiceModel;

namespace WCF_chat
{
    public class ServerUser
    {
        public int Id { get; set; }
        public string ServerUserName { get; set; }
        public OperationContext operationContext { get; set; }
    }
}
