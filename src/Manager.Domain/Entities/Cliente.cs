namespace Manager.Domain.Entities
{
    public class Cliente : EntityBase
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
        public DateTime Dt_registro { get; set; }

        public Token Token { get; set; }

        public Guid GrupoId { get; set; }
        public Grupo Grupo { get; set; }
    }
}
