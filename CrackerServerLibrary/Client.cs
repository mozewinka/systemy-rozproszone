namespace CrackerServerLibrary
{
    public class Client
    {
        public string ClientID { get; set; }
        public ICrackerServiceCallback Callback { get; set; }
    }
}
