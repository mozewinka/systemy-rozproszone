using CrackerClient.CrackerServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CrackerClient
{
    public class CallbackHandler : ICrackerServiceCallback
    {
        public void Print()
        {
            throw new NotImplementedException();
        }
    }

    internal class Program
    {
        private static void Main()
        {
            CallbackHandler callbackHandler = new CallbackHandler();
            InstanceContext instanceContext = new InstanceContext(callbackHandler);
            CrackerServiceClient client = new CrackerServiceClient(instanceContext);

            _ = Console.ReadLine();
            client.Close();
        }
    }
}
