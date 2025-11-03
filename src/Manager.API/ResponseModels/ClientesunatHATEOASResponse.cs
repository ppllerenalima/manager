using Manager.Domain.Responses.ClienteResponses;
using Newtonsoft.Json;
using RiskFirst.Hateoas.Models;

namespace Manager.API.ResponseModels
{
    public class ClientesunatHATEOASResponse : ILinkContainer
    {
        private Dictionary<string, Link> _links;
        public ClienteResponse Data;

        [JsonProperty(PropertyName = "_links")]
        public Dictionary<string, Link> Links
        {
            get => _links ??= new Dictionary<string, Link>();
            set => _links = value;
        }

        public void AddLink(string id, Link link)
        {
            Links.Add(id, link);
        }
    }
}