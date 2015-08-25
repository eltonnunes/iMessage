using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class BandeirasTef
    {
        public int IdBandeira { get; set; }
        public string DescricaoBandeira { get; set; }
        public int IdGrupo { get; set; }
        public string CodBandeiraERP { get; set; }
        public decimal CodBandeiraHostPagamento { get; set; }
        public Nullable<decimal> TaxaAdministracao { get; set; }
        public Nullable<int> IdTipoPagamento { get; set; }
        public string Sacado { get; set; }
    }
}
