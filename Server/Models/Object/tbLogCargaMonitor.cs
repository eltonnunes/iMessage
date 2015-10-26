using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Object
{
    public class tbLogCargaMonitor
    {
        public int idLogCarga { get; set; }
        public System.DateTime dtCompetencia { get; set; }
        //public string nrCNPJ { get; set; }
        //public int cdAdquirente { get; set; }
        public bool flStatusVendasCredito { get; set; }
        public bool flStatusVendasDebito { get; set; }
        public bool flStatusPagosCredito { get; set; }
        public bool flStatusPagosDebito { get; set; }
        public bool flStatusPagosAntecipacao { get; set; }
        public bool flStatusReceber { get; set; }
    }

    public class tbLogCargasMonitor
    {
        public int idLogCarga { get; set; }
        public System.DateTime dtCompetencia { get; set; }
        //public string nrCNPJ { get; set; }
        //public int cdAdquirente { get; set; }
        public bool flStatusVendasCredito { get; set; }
        public bool flStatusVendasDebito { get; set; }
        public bool flStatusPagosCredito { get; set; }
        public bool flStatusPagosDebito { get; set; }
        public bool flStatusPagosAntecipacao { get; set; }
        public bool flStatusReceber { get; set; }
        public List<tbLogCargaDetalheMonitor> tbLogCargasDetalheMonitor { get; set; } // para cada dsModalidade, o último detalhe
    }

    public class tbLogCargaDetalheMonitor
    {
        public Int32 idLogCargaDetalhe { get; set; }
        public DateTime dtExecucaoIni { get; set; }
        public Nullable<DateTime> dtExecucaoFim { get; set; }
        public byte flStatus { get; set; }
        public string dsMensagem { get; set; }
        public string dsModalidade { get; set; }
        public Nullable<int> qtTransacoes { get; set; }
        public Nullable<decimal> vlTotalProcessado { get; set; }
    }
}
