using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Requests.Sire.Compras
{
    public class ArchivoReporteRequest
    {
        public Guid ClienteId { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
    }
}
