using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Repositories
{
    public interface ISunatSscoRepository
    {
        Task<bool> ExisteHistorialAsync(string nombrePadron, string textoFechaSunat, CancellationToken cancellationToken = default);
        Task<int> GuardarOActualizarLoteAsync(IEnumerable<SujetoSinCapacidadOperativa> registros, string textoFechaSunat, CancellationToken cancellationToken = default);
        Task<SunatPadronHistorial?> ObtenerUltimaActualizacionAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<SujetoSinCapacidadOperativa> Items, int Total)> ListarPaginadoAsync(string? filtro, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);
        Task<List<SujetoSinCapacidadOperativa>> BuscarPorRucsAsync(IEnumerable<string> rucs, CancellationToken cancellationToken = default);
    }
}
