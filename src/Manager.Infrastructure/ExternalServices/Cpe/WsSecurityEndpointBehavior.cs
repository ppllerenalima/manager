using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Serialization;

namespace Manager.Infrastructure.ExternalServices.Cpe
{
    public class WsSecurityEndpointBehavior : IEndpointBehavior
    {
        private readonly string password;
        private readonly string username;

        public WsSecurityEndpointBehavior(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new WsSecurityMessageInspector(username, password));
        }
        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class WsSecurityMessageInspector : IClientMessageInspector
    {
        private readonly string password;
        private readonly string username;

        public WsSecurityMessageInspector(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var header = new Security
            {
                UsernameToken =
            {
                Password = new Password
                {
                    Value = password,
                    Type =
                        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"
                },
                Username = username
            }
            };
            request.Headers.Add(header);
            return null;
        }

        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
    }

    public class Password
    {
        [XmlAttribute] public string Type { get; set; }

        [XmlText] public string Value { get; set; }
    }

    [XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public class UsernameToken
    {
        [XmlElement] public string Username { get; set; }

        [XmlElement] public Password Password { get; set; }
    }

    public class Security : MessageHeader
    {
        public Security()
        {
            UsernameToken = new UsernameToken();
        }

        public UsernameToken UsernameToken { get; set; }

        public override string Name => GetType().Name;

        public override string Namespace => "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        public override bool MustUnderstand => true;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            var serializer = new XmlSerializer(typeof(UsernameToken));
            serializer.Serialize(writer, UsernameToken);
        }
    }
}
