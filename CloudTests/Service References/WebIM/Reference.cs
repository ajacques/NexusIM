﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CloudTests.WebIM {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AccountInfo", Namespace="com.adrensoftware.nexus")]
    [System.SerializableAttribute()]
    public partial class AccountInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int AccountIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool EnabledField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid GuidField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PasswordField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ProtocolTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ServerField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UsernameField;
        
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
        public int AccountId {
            get {
                return this.AccountIdField;
            }
            set {
                if ((this.AccountIdField.Equals(value) != true)) {
                    this.AccountIdField = value;
                    this.RaisePropertyChanged("AccountId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Enabled {
            get {
                return this.EnabledField;
            }
            set {
                if ((this.EnabledField.Equals(value) != true)) {
                    this.EnabledField = value;
                    this.RaisePropertyChanged("Enabled");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Guid {
            get {
                return this.GuidField;
            }
            set {
                if ((this.GuidField.Equals(value) != true)) {
                    this.GuidField = value;
                    this.RaisePropertyChanged("Guid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Password {
            get {
                return this.PasswordField;
            }
            set {
                if ((object.ReferenceEquals(this.PasswordField, value) != true)) {
                    this.PasswordField = value;
                    this.RaisePropertyChanged("Password");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProtocolType {
            get {
                return this.ProtocolTypeField;
            }
            set {
                if ((object.ReferenceEquals(this.ProtocolTypeField, value) != true)) {
                    this.ProtocolTypeField = value;
                    this.RaisePropertyChanged("ProtocolType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Server {
            get {
                return this.ServerField;
            }
            set {
                if ((object.ReferenceEquals(this.ServerField, value) != true)) {
                    this.ServerField = value;
                    this.RaisePropertyChanged("Server");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Username {
            get {
                return this.UsernameField;
            }
            set {
                if ((object.ReferenceEquals(this.UsernameField, value) != true)) {
                    this.UsernameField = value;
                    this.RaisePropertyChanged("Username");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BuddyData", Namespace="http://schemas.datacontract.org/2004/07/NexusCore.DataContracts")]
    [System.SerializableAttribute()]
    public partial class BuddyData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid mGuidField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string mNicknameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid mProtocolGuidField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CloudTests.WebIM.IMBuddyStatus mStatusField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string mStatusMessageField;
        
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
        public System.Guid mGuid {
            get {
                return this.mGuidField;
            }
            set {
                if ((this.mGuidField.Equals(value) != true)) {
                    this.mGuidField = value;
                    this.RaisePropertyChanged("mGuid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string mNickname {
            get {
                return this.mNicknameField;
            }
            set {
                if ((object.ReferenceEquals(this.mNicknameField, value) != true)) {
                    this.mNicknameField = value;
                    this.RaisePropertyChanged("mNickname");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid mProtocolGuid {
            get {
                return this.mProtocolGuidField;
            }
            set {
                if ((this.mProtocolGuidField.Equals(value) != true)) {
                    this.mProtocolGuidField = value;
                    this.RaisePropertyChanged("mProtocolGuid");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CloudTests.WebIM.IMBuddyStatus mStatus {
            get {
                return this.mStatusField;
            }
            set {
                if ((this.mStatusField.Equals(value) != true)) {
                    this.mStatusField = value;
                    this.RaisePropertyChanged("mStatus");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string mStatusMessage {
            get {
                return this.mStatusMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.mStatusMessageField, value) != true)) {
                    this.mStatusMessageField = value;
                    this.RaisePropertyChanged("mStatusMessage");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="IMBuddyStatus", Namespace="http://schemas.datacontract.org/2004/07/InstantMessage")]
    public enum IMBuddyStatus : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ONLINE = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OFFLINE = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        BUSY = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        AFK = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        IDLE = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        UNKNOWN = 5,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MessageData", Namespace="http://schemas.datacontract.org/2004/07/NexusCore.DataContracts")]
    [System.SerializableAttribute()]
    public partial class MessageData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string mMessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid mProtocolField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string mSenderField;
        
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
        public string mMessage {
            get {
                return this.mMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.mMessageField, value) != true)) {
                    this.mMessageField = value;
                    this.RaisePropertyChanged("mMessage");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid mProtocol {
            get {
                return this.mProtocolField;
            }
            set {
                if ((this.mProtocolField.Equals(value) != true)) {
                    this.mProtocolField = value;
                    this.RaisePropertyChanged("mProtocol");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string mSender {
            get {
                return this.mSenderField;
            }
            set {
                if ((object.ReferenceEquals(this.mSenderField, value) != true)) {
                    this.mSenderField = value;
                    this.RaisePropertyChanged("mSender");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="IMStatus", Namespace="http://schemas.datacontract.org/2004/07/InstantMessage")]
    public enum IMStatus : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        AVAILABLE = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        AWAY = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        BUSY = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        IDLE = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        INVISIBLE = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OFFLINE = 5,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OnThePhone = 6,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="com.adrensoftware.nexusim", ConfigurationName="WebIM.IWebIMService")]
    public interface IWebIMService {
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/Login", ReplyAction="com.adrensoftware.nexusim/IWebIMService/LoginResponse")]
        System.Guid Login(CloudTests.WebIM.AccountInfo accountInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/Disconnect", ReplyAction="com.adrensoftware.nexusim/IWebIMService/DisconnectResponse")]
        void Disconnect(System.Guid protocolId);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/GetNewContacts", ReplyAction="com.adrensoftware.nexusim/IWebIMService/GetNewContactsResponse")]
        System.Collections.Generic.List<CloudTests.WebIM.BuddyData> GetNewContacts();
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/GetAllContacts", ReplyAction="com.adrensoftware.nexusim/IWebIMService/GetAllContactsResponse")]
        System.Collections.Generic.List<CloudTests.WebIM.BuddyData> GetAllContacts();
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/GetMessages", ReplyAction="com.adrensoftware.nexusim/IWebIMService/GetMessagesResponse")]
        System.Collections.Generic.List<CloudTests.WebIM.MessageData> GetMessages();
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/SetStatusMessage", ReplyAction="com.adrensoftware.nexusim/IWebIMService/SetStatusMessageResponse")]
        void SetStatusMessage(System.Guid protocol, string message);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/SetStatus", ReplyAction="com.adrensoftware.nexusim/IWebIMService/SetStatusResponse")]
        void SetStatus(System.Guid protocol, CloudTests.WebIM.IMStatus status);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/GetStatus", ReplyAction="com.adrensoftware.nexusim/IWebIMService/GetStatusResponse")]
        CloudTests.WebIM.IMStatus GetStatus(System.Guid protocol);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.adrensoftware.nexusim/IWebIMService/SendMessage", ReplyAction="com.adrensoftware.nexusim/IWebIMService/SendMessageResponse")]
        void SendMessage(System.Guid protocol, string receiver, string message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IWebIMServiceChannel : CloudTests.WebIM.IWebIMService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WebIMServiceClient : System.ServiceModel.ClientBase<CloudTests.WebIM.IWebIMService>, CloudTests.WebIM.IWebIMService {
        
        public WebIMServiceClient() {
        }
        
        public WebIMServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WebIMServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebIMServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebIMServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Guid Login(CloudTests.WebIM.AccountInfo accountInfo) {
            return base.Channel.Login(accountInfo);
        }
        
        public void Disconnect(System.Guid protocolId) {
            base.Channel.Disconnect(protocolId);
        }
        
        public System.Collections.Generic.List<CloudTests.WebIM.BuddyData> GetNewContacts() {
            return base.Channel.GetNewContacts();
        }
        
        public System.Collections.Generic.List<CloudTests.WebIM.BuddyData> GetAllContacts() {
            return base.Channel.GetAllContacts();
        }
        
        public System.Collections.Generic.List<CloudTests.WebIM.MessageData> GetMessages() {
            return base.Channel.GetMessages();
        }
        
        public void SetStatusMessage(System.Guid protocol, string message) {
            base.Channel.SetStatusMessage(protocol, message);
        }
        
        public void SetStatus(System.Guid protocol, CloudTests.WebIM.IMStatus status) {
            base.Channel.SetStatus(protocol, status);
        }
        
        public CloudTests.WebIM.IMStatus GetStatus(System.Guid protocol) {
            return base.Channel.GetStatus(protocol);
        }
        
        public void SendMessage(System.Guid protocol, string receiver, string message) {
            base.Channel.SendMessage(protocol, receiver, message);
        }
    }
}
