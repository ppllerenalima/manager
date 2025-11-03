namespace Manager.Domain.Entities
{
    public class Cliente : EntityBase
    {
        public string Ruc { get; set; } = default!;
        public string Razonsocial { get; set; } = default!;
        public string Numero { get; set; } = default!;
        public string Direccion { get; set; } = default!;
        public string? Image { get; set; }

        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool tienePermiso { get; set; } = false;
        public DateTime Dt_registro { get; set; }

        public Token Token { get; set; } = default!;

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid GrupoId { get; set; }
        public Grupo Grupo { get; set; } = default!;

        // 🔹 Relación inversa
        public ICollection<PerTributario> PeriodosTributarios { get; set; } = new List<PerTributario>();
    }
}
