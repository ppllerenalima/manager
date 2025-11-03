namespace Manager.Domain.Responses.ClienteResponses
{
    public class ClienteResponse
    {
        public Guid Id { get; set; }
        public bool IsInactive { get; set; }

        public string Ruc { get; set; }
        public string Razonsocial { get; set; }
        public string Numero { get; set; }
        public string Direccion { get; set; }
        public string? Image { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool tienePermiso { get; set; }
        public DateTime FechaRegistro { get; set; }

        public Guid UserId { get; set; }
        public Guid GrupoId { get; set; }
    }
}