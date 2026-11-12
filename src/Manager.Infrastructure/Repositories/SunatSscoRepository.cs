using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.Repositories
{
    public class SunatSscoRepository : ISunatSscoRepository
    {
        private readonly ManagerContext _context;

        public SunatSscoRepository(ManagerContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteHistorialAsync(string nombrePadron, string textoFechaSunat, CancellationToken cancellationToken = default)
        {
            return await _context.SunatPadronHistorials
                .AnyAsync(h => !h.IsInactive && h.NombrePadron == nombrePadron && h.TextoFechaSunat == textoFechaSunat, cancellationToken);
        }

        public async Task<int> GuardarOActualizarLoteAsync(IEnumerable<SujetoSinCapacidadOperativa> registros, string textoFechaSunat, CancellationToken cancellationToken = default)
        {
            var listaExcel = registros.ToList();

            var existentes = await _context.SujetoSinCapacidadOperativas
                .ToDictionaryAsync(x => $"{x.Ruc}|{x.NumeroResolucion}", cancellationToken);

            var nuevos = new List<SujetoSinCapacidadOperativa>();

            foreach (var item in listaExcel)
            {
                string clave = $"{item.Ruc}|{item.NumeroResolucion}";
                if (existentes.TryGetValue(clave, out var registroBd))
                {
                    registroBd.RazonSocial = item.RazonSocial;
                    registroBd.DomicilioFiscal = item.DomicilioFiscal;
                    registroBd.FechaEmisionResolucion = item.FechaEmisionResolucion;
                    registroBd.FechaFirmezaResolucion = item.FechaFirmezaResolucion;
                    registroBd.DocumentoRepresentanteLegal = item.DocumentoRepresentanteLegal;
                    registroBd.NombreRepresentanteLegal = item.NombreRepresentanteLegal;
                    registroBd.FechaPublicacion = item.FechaPublicacion;
                    registroBd.FechaActualizacionSistema = DateTime.Now;
                    registroBd.IsInactive = false;
                }
                else
                {
                    nuevos.Add(item);
                }
            }

            if (nuevos.Count > 0)
                await _context.SujetoSinCapacidadOperativas.AddRangeAsync(nuevos, cancellationToken);

            _context.SunatPadronHistorials.Add(new SunatPadronHistorial
            {
                NombrePadron = "SSCO",
                TextoFechaSunat = textoFechaSunat,
                RegistrosProcesados = listaExcel.Count,
                FechaEjecucion = DateTime.Now
            });

            await _context.SaveChangesAsync(cancellationToken);
            return nuevos.Count;
        }

        public async Task<SunatPadronHistorial?> ObtenerUltimaActualizacionAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SunatPadronHistorials
                .Where(h => !h.IsInactive && h.NombrePadron == "SSCO")
                .OrderByDescending(h => h.FechaEjecucion)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(IEnumerable<SujetoSinCapacidadOperativa> Items, int Total)> ListarPaginadoAsync(string? filtro, int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
        {
            var query = _context.SujetoSinCapacidadOperativas
                .AsNoTracking()
                .Where(x => !x.IsInactive);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string term = filtro.Trim();
                query = query.Where(x => x.Ruc.Contains(term) || x.RazonSocial.Contains(term) || x.NumeroResolucion.Contains(term));
            }

            int total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.FechaPublicacion)
                .ThenBy(x => x.RazonSocial)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task<List<SujetoSinCapacidadOperativa>> BuscarPorRucsAsync(IEnumerable<string> rucs, CancellationToken cancellationToken = default)
        {
            var listaRucs = rucs.Distinct().ToList();
            return await _context.SujetoSinCapacidadOperativas
                .AsNoTracking()
                .Where(x => !x.IsInactive && listaRucs.Contains(x.Ruc))
                .ToListAsync(cancellationToken);
        }
    }
}
