﻿using Server.Models.Object;
using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class tbBandeira
    {
        public tbBandeira()
        {
            this.Recebimentos = new List<Recebimento>();
            this.tbBancoParametros = new List<tbBancoParametro>();
            this.tbRecebimentoAjustes = new List<tbRecebimentoAjuste>();
            this.tbRecebimentoResumoManuals = new List<tbRecebimentoResumoManual>();
            this.tbResumoVendas = new List<tbResumoVenda>();
        }

        public int cdBandeira { get; set; }
        public string dsBandeira { get; set; }
        public int cdAdquirente { get; set; }
        public string dsTipo { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual ICollection<Recebimento> Recebimentos { get; set; }
        public virtual ICollection<tbBancoParametro> tbBancoParametros { get; set; }
        public virtual ICollection<tbRecebimentoAjuste> tbRecebimentoAjustes { get; set; }
        public virtual ICollection<tbRecebimentoResumoManual> tbRecebimentoResumoManuals { get; set; }
        public virtual ICollection<tbResumoVenda> tbResumoVendas { get; set; }
    }
}
