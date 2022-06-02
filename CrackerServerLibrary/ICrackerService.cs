using System;
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
        void AnnounceResult(string message);

        [OperationContract(IsOneWay = true)]
        void AddClient();
    }
    public interface ICrackerServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void Print(string md5Password);

        [OperationContract(IsOneWay = true)]
        void BruteCrack(string startPosition, string endPosition, string md5Password);

        [OperationContract(IsOneWay = true)]
        void DictionaryCrack(int startPosition, int endPosition, string md5Password, bool checkUpperCase, bool checkSuffix, String suffix);
    }
}
