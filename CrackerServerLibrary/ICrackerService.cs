using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CrackerServerLibrary
{
    [DataContract]
    public class DictionaryData
    {
        [DataMember]
        public List<string> List { get; set; }
    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICrackerServiceCallback))]
    public interface ICrackerService
    {
        [OperationContract]
        DictionaryData SendDictionary();

        [OperationContract(IsOneWay = true)]
        void Receive();

    }
    public interface ICrackerServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void Print();

    }
}
