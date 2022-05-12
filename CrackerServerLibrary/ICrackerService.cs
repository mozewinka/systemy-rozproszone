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
        void Print(string md5Password);

        [OperationContract(IsOneWay = true)]
        void PrintBrute(string md5Password);


        [OperationContract]
        string BruteCrack(string Sa, string Sb);

        [OperationContract]
        string AddHex(string a, string b);

        [OperationContract]
        string GetHash(string input);

    }
}
