using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class tbContaCorrente
    {
        public tbContaCorrente()
        {
            this.tbContaCorrente_tbLoginAdquirenteEmpresa = new List<tbContaCorrente_tbLoginAdquirenteEmpresa>();
            this.tbExtratoes = new List<tbExtrato>();
        }

        public int cdContaCorrente { get; set; }
        public int cdGrupo { get; set; }
        public string nrCnpj { get; set; }
        public string cdBanco { get; set; }
        public string nrAgencia { get; set; }
        public string nrConta { get; set; }
        public bool flAtivo { get; set; }
        public virtual ICollection<tbContaCorrente_tbLoginAdquirenteEmpresa> tbContaCorrente_tbLoginAdquirenteEmpresa { get; set; }
        public virtual grupo_empresa grupo_empresa { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual ICollection<tbExtrato> tbExtratoes { get; set; }
    }
}
