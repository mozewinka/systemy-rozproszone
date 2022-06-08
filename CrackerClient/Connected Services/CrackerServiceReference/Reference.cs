﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrackerClient.CrackerServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DictionaryData", Namespace="http://schemas.datacontract.org/2004/07/CrackerServerLibrary")]
    [System.SerializableAttribute()]
    public partial class DictionaryData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<string> ListField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<string> List {
            get {
                return this.ListField;
            }
            set {
                if ((object.ReferenceEquals(this.ListField, value) != true)) {
                    this.ListField = value;
                    this.RaisePropertyChanged("List");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CrackerServiceReference.ICrackerService", CallbackContract=typeof(CrackerClient.CrackerServiceReference.ICrackerServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface ICrackerService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICrackerService/SendDictionary", ReplyAction="http://tempuri.org/ICrackerService/SendDictionaryResponse")]
        CrackerClient.CrackerServiceReference.DictionaryData SendDictionary();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICrackerService/SendDictionary", ReplyAction="http://tempuri.org/ICrackerService/SendDictionaryResponse")]
        System.Threading.Tasks.Task<CrackerClient.CrackerServiceReference.DictionaryData> SendDictionaryAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICrackerService/AnnounceResult")]
        void AnnounceResult(string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICrackerService/AnnounceResult")]
        System.Threading.Tasks.Task AnnounceResultAsync(string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICrackerService/AddClient")]
        void AddClient();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICrackerService/AddClient")]
        System.Threading.Tasks.Task AddClientAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICrackerServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICrackerService/Print")]
        void Print(string md5Password);
<<<<<<< HEAD
=======
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICrackerService/BruteCrack")]
        void BruteCrack(string startPosition, string endPosition, string md5Password);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ICrackerService/DictionaryCrack")]
        void DictionaryCrack(int startPosition, int endPosition, string md5Password, bool checkUpperCase, bool checkSuffix, string suffix);
>>>>>>> 345f6d9ed391109fd4f376f238f5617d6a038bbe
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICrackerServiceChannel : CrackerClient.CrackerServiceReference.ICrackerService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CrackerServiceClient : System.ServiceModel.DuplexClientBase<CrackerClient.CrackerServiceReference.ICrackerService>, CrackerClient.CrackerServiceReference.ICrackerService {
        
        public CrackerServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public CrackerServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public CrackerServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public CrackerServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public CrackerServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public CrackerClient.CrackerServiceReference.DictionaryData SendDictionary() {
            return base.Channel.SendDictionary();
        }
        
        public System.Threading.Tasks.Task<CrackerClient.CrackerServiceReference.DictionaryData> SendDictionaryAsync() {
            return base.Channel.SendDictionaryAsync();
        }
        
        public void AnnounceResult(string message) {
            base.Channel.AnnounceResult(message);
        }
        
        public System.Threading.Tasks.Task AnnounceResultAsync(string message) {
            return base.Channel.AnnounceResultAsync(message);
        }
        
        public void AddClient() {
            base.Channel.AddClient();
        }
        
        public System.Threading.Tasks.Task AddClientAsync() {
            return base.Channel.AddClientAsync();
        }
    }
}
