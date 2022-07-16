﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CloudTests.NexusCore {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserLocationData", Namespace="http://schemas.datacontract.org/2004/07/NexusCore.DataContracts")]
    [System.SerializableAttribute()]
    public partial class UserLocationData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int AccuracyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime LastUpdatedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LatitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LongitudeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReverseGeocodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int RowIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CloudTests.NexusCore.LocationServiceType ServiceTypeField;
        
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
        public int Accuracy {
            get {
                return this.AccuracyField;
            }
            set {
                if ((this.AccuracyField.Equals(value) != true)) {
                    this.AccuracyField = value;
                    this.RaisePropertyChanged("Accuracy");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime LastUpdated {
            get {
                return this.LastUpdatedField;
            }
            set {
                if ((this.LastUpdatedField.Equals(value) != true)) {
                    this.LastUpdatedField = value;
                    this.RaisePropertyChanged("LastUpdated");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Latitude {
            get {
                return this.LatitudeField;
            }
            set {
                if ((this.LatitudeField.Equals(value) != true)) {
                    this.LatitudeField = value;
                    this.RaisePropertyChanged("Latitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Longitude {
            get {
                return this.LongitudeField;
            }
            set {
                if ((this.LongitudeField.Equals(value) != true)) {
                    this.LongitudeField = value;
                    this.RaisePropertyChanged("Longitude");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ReverseGeocode {
            get {
                return this.ReverseGeocodeField;
            }
            set {
                if ((object.ReferenceEquals(this.ReverseGeocodeField, value) != true)) {
                    this.ReverseGeocodeField = value;
                    this.RaisePropertyChanged("ReverseGeocode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int RowId {
            get {
                return this.RowIdField;
            }
            set {
                if ((this.RowIdField.Equals(value) != true)) {
                    this.RowIdField = value;
                    this.RaisePropertyChanged("RowId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CloudTests.NexusCore.LocationServiceType ServiceType {
            get {
                return this.ServiceTypeField;
            }
            set {
                if ((this.ServiceTypeField.Equals(value) != true)) {
                    this.ServiceTypeField = value;
                    this.RaisePropertyChanged("ServiceType");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="LocationServiceType", Namespace="http://schemas.datacontract.org/2004/07/NexusCore.DataContracts")]
    public enum LocationServiceType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        GoogleLatitude = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        FireEagle = 1,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ContactLocationInfo", Namespace="http://schemas.datacontract.org/2004/07/NexusCore.DataContracts")]
    [System.SerializableAttribute()]
    public partial class ContactLocationInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CloudTests.NexusCore.AccountInfo AccountInfoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int LocationIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool MessagableField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private CloudTests.NexusCore.LocationServiceType ServiceTypeField;
        
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
        public CloudTests.NexusCore.AccountInfo AccountInfo {
            get {
                return this.AccountInfoField;
            }
            set {
                if ((object.ReferenceEquals(this.AccountInfoField, value) != true)) {
                    this.AccountInfoField = value;
                    this.RaisePropertyChanged("AccountInfo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int LocationId {
            get {
                return this.LocationIdField;
            }
            set {
                if ((this.LocationIdField.Equals(value) != true)) {
                    this.LocationIdField = value;
                    this.RaisePropertyChanged("LocationId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Messagable {
            get {
                return this.MessagableField;
            }
            set {
                if ((this.MessagableField.Equals(value) != true)) {
                    this.MessagableField = value;
                    this.RaisePropertyChanged("Messagable");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public CloudTests.NexusCore.LocationServiceType ServiceType {
            get {
                return this.ServiceTypeField;
            }
            set {
                if ((this.ServiceTypeField.Equals(value) != true)) {
                    this.ServiceTypeField = value;
                    this.RaisePropertyChanged("ServiceType");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="AccountInfo", Namespace="com.nexus-im")]
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
        private CloudTests.NexusCore.IMProtocolStatus StatusField;
        
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
        public CloudTests.NexusCore.IMProtocolStatus Status {
            get {
                return this.StatusField;
            }
            set {
                if ((this.StatusField.Equals(value) != true)) {
                    this.StatusField = value;
                    this.RaisePropertyChanged("Status");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="IMProtocolStatus", Namespace="http://schemas.datacontract.org/2004/07/InstantMessage")]
    public enum IMProtocolStatus : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ONLINE = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        CONNECTING = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ERROR = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OFFLINE = 3,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MyAccountInformation", Namespace="http://schemas.datacontract.org/2004/07/NexusCore.DataContracts")]
    [System.SerializableAttribute()]
    public partial class MyAccountInformation : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string mFirstNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string mUsernameField;
        
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
        public string mFirstName {
            get {
                return this.mFirstNameField;
            }
            set {
                if ((object.ReferenceEquals(this.mFirstNameField, value) != true)) {
                    this.mFirstNameField = value;
                    this.RaisePropertyChanged("mFirstName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string mUsername {
            get {
                return this.mUsernameField;
            }
            set {
                if ((object.ReferenceEquals(this.mUsernameField, value) != true)) {
                    this.mUsernameField = value;
                    this.RaisePropertyChanged("mUsername");
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
    [System.ServiceModel.ServiceContractAttribute(Namespace="com.nexusim.core", ConfigurationName="NexusCore.CoreService")]
    public interface CoreService {
        
        // CODEGEN: Generating message contract since the wrapper namespace () of message LogoutRequest does not match the default value (com.nexusim.core)
        [System.ServiceModel.OperationContractAttribute(Action="urn:JSCoreService/Logout", ReplyAction="urn:JSCoreService/LogoutResponse")]
        CloudTests.NexusCore.LogoutResponse Logout(CloudTests.NexusCore.LogoutRequest request);
        
        // CODEGEN: Generating message contract since the wrapper namespace () of message GetLocationRequest does not match the default value (com.nexusim.core)
        [System.ServiceModel.OperationContractAttribute(Action="urn:JSCoreService/GetLocation", ReplyAction="urn:JSCoreService/GetLocationResponse")]
        CloudTests.NexusCore.GetLocationResponse GetLocation(CloudTests.NexusCore.GetLocationRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.nexusim.core/CoreService/Login", ReplyAction="com.nexusim.core/CoreService/LoginResponse")]
        void Login(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.nexusim.core/CoreService/CookieLogin", ReplyAction="com.nexusim.core/CoreService/CookieLoginResponse")]
        string CookieLogin(string username, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.nexusim.core/CoreService/LoginWithToken", ReplyAction="com.nexusim.core/CoreService/LoginWithTokenResponse")]
        void LoginWithToken(string token);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.nexusim.core/CoreService/GetLocationData", ReplyAction="com.nexusim.core/CoreService/GetLocationDataResponse")]
        System.Collections.Generic.List<CloudTests.NexusCore.ContactLocationInfo> GetLocationData();
        
        [System.ServiceModel.OperationContractAttribute(Action="com.nexusim.core/CoreService/GetMultipleLocations", ReplyAction="com.nexusim.core/CoreService/GetMultipleLocationsResponse")]
        System.Collections.Generic.List<CloudTests.NexusCore.UserLocationData> GetMultipleLocations(System.Collections.Generic.List<int> rowIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="com.nexusim.core/CoreService/GetAccounts", ReplyAction="com.nexusim.core/CoreService/GetAccountsResponse")]
        System.Collections.Generic.List<CloudTests.NexusCore.AccountInfo> GetAccounts();
        
        [System.ServiceModel.OperationContractAttribute(Action="com.nexusim.core/CoreService/GetMyAccountInfo", ReplyAction="com.nexusim.core/CoreService/GetMyAccountInfoResponse")]
        CloudTests.NexusCore.MyAccountInformation GetMyAccountInfo();
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Logout", WrapperNamespace="", IsWrapped=true)]
    public partial class LogoutRequest {
        
        public LogoutRequest() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="LogoutResponse", WrapperNamespace="", IsWrapped=true)]
    public partial class LogoutResponse {
        
        public LogoutResponse() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetLocation", WrapperNamespace="", IsWrapped=true)]
    public partial class GetLocationRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public int rowId;
        
        public GetLocationRequest() {
        }
        
        public GetLocationRequest(int rowId) {
            this.rowId = rowId;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetLocationResponse", WrapperNamespace="", IsWrapped=true)]
    public partial class GetLocationResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public CloudTests.NexusCore.UserLocationData GetLocationResult;
        
        public GetLocationResponse() {
        }
        
        public GetLocationResponse(CloudTests.NexusCore.UserLocationData GetLocationResult) {
            this.GetLocationResult = GetLocationResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface CoreServiceChannel : CloudTests.NexusCore.CoreService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CoreServiceClient : System.ServiceModel.ClientBase<CloudTests.NexusCore.CoreService>, CloudTests.NexusCore.CoreService {
        
        public CoreServiceClient() {
        }
        
        public CoreServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CoreServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CoreServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CoreServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        CloudTests.NexusCore.LogoutResponse CloudTests.NexusCore.CoreService.Logout(CloudTests.NexusCore.LogoutRequest request) {
            return base.Channel.Logout(request);
        }
        
        public void Logout() {
            CloudTests.NexusCore.LogoutRequest inValue = new CloudTests.NexusCore.LogoutRequest();
            CloudTests.NexusCore.LogoutResponse retVal = ((CloudTests.NexusCore.CoreService)(this)).Logout(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        CloudTests.NexusCore.GetLocationResponse CloudTests.NexusCore.CoreService.GetLocation(CloudTests.NexusCore.GetLocationRequest request) {
            return base.Channel.GetLocation(request);
        }
        
        public CloudTests.NexusCore.UserLocationData GetLocation(int rowId) {
            CloudTests.NexusCore.GetLocationRequest inValue = new CloudTests.NexusCore.GetLocationRequest();
            inValue.rowId = rowId;
            CloudTests.NexusCore.GetLocationResponse retVal = ((CloudTests.NexusCore.CoreService)(this)).GetLocation(inValue);
            return retVal.GetLocationResult;
        }
        
        public void Login(string username, string password) {
            base.Channel.Login(username, password);
        }
        
        public string CookieLogin(string username, string password) {
            return base.Channel.CookieLogin(username, password);
        }
        
        public void LoginWithToken(string token) {
            base.Channel.LoginWithToken(token);
        }
        
        public System.Collections.Generic.List<CloudTests.NexusCore.ContactLocationInfo> GetLocationData() {
            return base.Channel.GetLocationData();
        }
        
        public System.Collections.Generic.List<CloudTests.NexusCore.UserLocationData> GetMultipleLocations(System.Collections.Generic.List<int> rowIds) {
            return base.Channel.GetMultipleLocations(rowIds);
        }
        
        public System.Collections.Generic.List<CloudTests.NexusCore.AccountInfo> GetAccounts() {
            return base.Channel.GetAccounts();
        }
        
        public CloudTests.NexusCore.MyAccountInformation GetMyAccountInfo() {
            return base.Channel.GetMyAccountInfo();
        }
    }
}
