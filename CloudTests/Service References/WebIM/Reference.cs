﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
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
        private InstantMessage.IMBuddyStatus mStatusField;
        
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
        public InstantMessage.IMBuddyStatus mStatus {
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
}
