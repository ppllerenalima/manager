namespace Manager.Domain.Requests.Cliente
{
    public class AddClienteRequest
    {
        public string Ruc { get; set; }
        public string Razonsocial { get; set; }
        public string Numero { get; set; }
        public string Direccion { get; set; }
        public string? Image { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool IsInactive { get; set; } = false;
        public Guid GrupoId { get; set; }
    }
}