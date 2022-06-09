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

    [DataContract]
    public class ResultData
    {
        [DataMember]
        public string ClientID { get; set; }

        [DataMember]
        public bool IsCracked { get; set; }

        [DataMember]
        public string CrackedPassword { get; set; }

        [DataMember]
        public long CrackingTime { get; set; }

        [DataMember]
        public long CrackingPerformance { get; set; }

        [DataMember]
        public string CrackingMethod { get; set; }

    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICrackerServiceCallback))]
    public interface ICrackerService
    {
        [OperationContract]
        DictionaryData SendDictionary();
        [OperationContract]
        string SendDictionaryHash();

        [OperationContract(IsOneWay = true)]
        void AnnounceResult(ResultData result);

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
        void DictionaryCrack(int startPosition, int endPosition, string md5Password, bool checkUpperCase, bool checkSuffix);
    }
}
