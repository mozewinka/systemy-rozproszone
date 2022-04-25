using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CrackerServerLibrary
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICrackerServiceCallback))]
    public interface ICrackerService
    {
        [OperationContract(IsOneWay = true)]
        void Send();

        [OperationContract(IsOneWay = true)]
        void Receive();

    }
    public interface ICrackerServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void Print();

    }
}
