﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by SlSvcUtil, version 5.0.61118.0
// 

namespace KBS.App.TaxonFinder.iOS.Api
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "", ConfigurationName = "Mobile")]
    public interface Mobile
    {

        [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "urn:Mobile/Register", ReplyAction = "urn:Mobile/RegisterResponse")]
        System.IAsyncResult BeginRegister(string username, string password, string deviceId, System.AsyncCallback callback, object asyncState);

        string EndRegister(System.IAsyncResult result);

        [System.ServiceModel.OperationContractAttribute(AsyncPattern = true, Action = "urn:Mobile/AddNewUser", ReplyAction = "urn:Mobile/AddNewUserResponse")]
        System.IAsyncResult BeginAddNewUser(string givenname, string surname, string mail, string password, string comment, string source, System.AsyncCallback callback, object asyncState);

        string EndAddNewUser(System.IAsyncResult result);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface MobileChannel : Mobile, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RegisterCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        public RegisterCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public string Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AddNewUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        public AddNewUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public string Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MobileClient : System.ServiceModel.ClientBase<Mobile>, Mobile
    {

        private BeginOperationDelegate onBeginRegisterDelegate;

        private EndOperationDelegate onEndRegisterDelegate;

        private System.Threading.SendOrPostCallback onRegisterCompletedDelegate;

        private BeginOperationDelegate onBeginAddNewUserDelegate;

        private EndOperationDelegate onEndAddNewUserDelegate;

        private System.Threading.SendOrPostCallback onAddNewUserCompletedDelegate;

        private BeginOperationDelegate onBeginOpenDelegate;

        private EndOperationDelegate onEndOpenDelegate;

        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;

        private BeginOperationDelegate onBeginCloseDelegate;

        private EndOperationDelegate onEndCloseDelegate;

        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;

        public MobileClient()
        {
        }

        public MobileClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public MobileClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public MobileClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public MobileClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public System.Net.CookieContainer CookieContainer
        {
            get
            {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null))
                {
                    return httpCookieContainerManager.CookieContainer;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null))
                {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else
                {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }

        public event System.EventHandler<RegisterCompletedEventArgs> RegisterCompleted;

        public event System.EventHandler<AddNewUserCompletedEventArgs> AddNewUserCompleted;

        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;

        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Mobile.BeginRegister(string username, string password, string deviceId, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginRegister(username, password, deviceId, callback, asyncState);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        string Mobile.EndRegister(System.IAsyncResult result)
        {
            return base.Channel.EndRegister(result);
        }

        private System.IAsyncResult OnBeginRegister(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            string username = ((string)(inValues[0]));
            string password = ((string)(inValues[1]));
            string deviceId = ((string)(inValues[2]));
            return ((Mobile)(this)).BeginRegister(username, password, deviceId, callback, asyncState);
        }

        private object[] OnEndRegister(System.IAsyncResult result)
        {
            string retVal = ((Mobile)(this)).EndRegister(result);
            return new object[] {
                retVal};
        }

        private void OnRegisterCompleted(object state)
        {
            if ((this.RegisterCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RegisterCompleted(this, new RegisterCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }

        public void RegisterAsync(string username, string password, string deviceId)
        {
            this.RegisterAsync(username, password, deviceId, null);
        }

        public void RegisterAsync(string username, string password, string deviceId, object userState)
        {
            if ((this.onBeginRegisterDelegate == null))
            {
                this.onBeginRegisterDelegate = new BeginOperationDelegate(this.OnBeginRegister);
            }
            if ((this.onEndRegisterDelegate == null))
            {
                this.onEndRegisterDelegate = new EndOperationDelegate(this.OnEndRegister);
            }
            if ((this.onRegisterCompletedDelegate == null))
            {
                this.onRegisterCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRegisterCompleted);
            }
            base.InvokeAsync(this.onBeginRegisterDelegate, new object[] {
                    username,
                    password,
                    deviceId}, this.onEndRegisterDelegate, this.onRegisterCompletedDelegate, userState);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Mobile.BeginAddNewUser(string givenname, string surname, string mail, string password, string comment, string source, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginAddNewUser(givenname, surname, mail, password, comment, source, callback, asyncState);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        string Mobile.EndAddNewUser(System.IAsyncResult result)
        {
            return base.Channel.EndAddNewUser(result);
        }

        private System.IAsyncResult OnBeginAddNewUser(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            string givenname = ((string)(inValues[0]));
            string surname = ((string)(inValues[1]));
            string mail = ((string)(inValues[2]));
            string password = ((string)(inValues[3]));
            string comment = ((string)(inValues[4]));
            string source = ((string)(inValues[5]));
            return ((Mobile)(this)).BeginAddNewUser(givenname, surname, mail, password, comment, source, callback, asyncState);
        }

        private object[] OnEndAddNewUser(System.IAsyncResult result)
        {
            string retVal = ((Mobile)(this)).EndAddNewUser(result);
            return new object[] {
                retVal};
        }

        private void OnAddNewUserCompleted(object state)
        {
            if ((this.AddNewUserCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.AddNewUserCompleted(this, new AddNewUserCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }

        public void AddNewUserAsync(string givenname, string surname, string mail, string password, string comment, string source)
        {
            this.AddNewUserAsync(givenname, surname, mail, password, comment, source, null);
        }

        public void AddNewUserAsync(string givenname, string surname, string mail, string password, string comment, string source, object userState)
        {
            if ((this.onBeginAddNewUserDelegate == null))
            {
                this.onBeginAddNewUserDelegate = new BeginOperationDelegate(this.OnBeginAddNewUser);
            }
            if ((this.onEndAddNewUserDelegate == null))
            {
                this.onEndAddNewUserDelegate = new EndOperationDelegate(this.OnEndAddNewUser);
            }
            if ((this.onAddNewUserCompletedDelegate == null))
            {
                this.onAddNewUserCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAddNewUserCompleted);
            }
            base.InvokeAsync(this.onBeginAddNewUserDelegate, new object[] {
                    givenname,
                    surname,
                    mail,
                    password,
                    comment,
                    source}, this.onEndAddNewUserDelegate, this.onAddNewUserCompletedDelegate, userState);
        }

        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }

        private object[] OnEndOpen(System.IAsyncResult result)
        {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }

        private void OnOpenCompleted(object state)
        {
            if ((this.OpenCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void OpenAsync()
        {
            this.OpenAsync(null);
        }

        public void OpenAsync(object userState)
        {
            if ((this.onBeginOpenDelegate == null))
            {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null))
            {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null))
            {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }

        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState)
        {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }

        private object[] OnEndClose(System.IAsyncResult result)
        {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }

        private void OnCloseCompleted(object state)
        {
            if ((this.CloseCompleted != null))
            {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }

        public void CloseAsync()
        {
            this.CloseAsync(null);
        }

        public void CloseAsync(object userState)
        {
            if ((this.onBeginCloseDelegate == null))
            {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null))
            {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null))
            {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }

        protected override Mobile CreateChannel()
        {
            return new MobileClientChannel(this);
        }

        private class MobileClientChannel : ChannelBase<Mobile>, Mobile
        {

            public MobileClientChannel(System.ServiceModel.ClientBase<Mobile> client) :
                    base(client)
            {
            }

            public System.IAsyncResult BeginRegister(string username, string password, string deviceId, System.AsyncCallback callback, object asyncState)
            {
                object[] _args = new object[3];
                _args[0] = username;
                _args[1] = password;
                _args[2] = deviceId;
                System.IAsyncResult _result = base.BeginInvoke("Register", _args, callback, asyncState);
                return _result;
            }

            public string EndRegister(System.IAsyncResult result)
            {
                object[] _args = new object[0];
                string _result = ((string)(base.EndInvoke("Register", _args, result)));
                return _result;
            }

            public System.IAsyncResult BeginAddNewUser(string givenname, string surname, string mail, string password, string comment, string source, System.AsyncCallback callback, object asyncState)
            {
                object[] _args = new object[6];
                _args[0] = givenname;
                _args[1] = surname;
                _args[2] = mail;
                _args[3] = password;
                _args[4] = comment;
                _args[5] = source;
                System.IAsyncResult _result = base.BeginInvoke("AddNewUser", _args, callback, asyncState);
                return _result;
            }

            public string EndAddNewUser(System.IAsyncResult result)
            {
                object[] _args = new object[0];
                string _result = ((string)(base.EndInvoke("AddNewUser", _args, result)));
                return _result;
            }
        }
    }
}