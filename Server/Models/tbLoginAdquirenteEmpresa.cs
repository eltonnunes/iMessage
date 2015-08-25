using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class tbLoginAdquirenteEmpresa
    {
        public tbLoginAdquirenteEmpresa()
        {
            this.tbContaCorrente_tbLoginAdquirenteEmpresa = new List<tbContaCorrente_tbLoginAdquirenteEmpresa>();
            this.tbExecucaoLogs = new List<tbExecucaoLog>();
        }

        public int cdLoginAdquirenteEmpresa { get; set; }
        public int cdAdquirente { get; set; }
        public int cdGrupo { get; set; }
        public string nrCNPJ { get; set; }
        public string dsLogin { get; set; }
        public string dsSenha { get; set; }
        public string cdEstabelecimento { get; set; }
        public Nullable<System.DateTime> dtAlteracao { get; set; }
        public byte stLoginAdquirente { get; set; }
        public byte stLoginAdquirenteEmpresa { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual ICollection<tbContaCorrente_tbLoginAdquirenteEmpresa> tbContaCorrente_tbLoginAdquirenteEmpresa { get; set; }
        public virtual ICollection<tbExecucaoLog> tbExecucaoLogs { get; set; }
        public virtual empresa empresa { get; set; }
    }
}
