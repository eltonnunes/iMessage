using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models.Object
{
    public class FilaBootICard
    {
        public class FilaTbAdquirente
        {
            public int cdAdquirente { get; set; }
            public string nmAdquirente { get; set; }
        }

        public class FilaEmpresa
        {
            public string nu_cnpj { get; set; }
            public string ds_fantasia { get; set; }
            public string filial { get; set; }
        }

        public class FilaGrupoEmpresa
        {
            public int id_grupo { get; set; }
            public string ds_nome { get; set; }
            public byte cdPrioridade { get; set; }
        }

        public class FilaWebpagesUsers
        {
            public int id_users { get; set; }
            public string ds_login { get; set; }
        }

        public int id { get; set; }
        public string dsModalidade { get; set; }
        public DateTime dtInicio { get; set; }
        public DateTime dtFim { get; set; }
        public byte stProcessamento { get; set; }
        public Nullable<DateTime> dtProcessamento { get; set; }
        public DateTime dtInsert { get; set; }
        public FilaTbAdquirente tbAdquirente { get; set; }
        public FilaEmpresa empresa { get; set; }
        public FilaGrupoEmpresa grupo_empresa { get; set; }
        public Nullable<int> cdUser { get; set; }
        //public FilaWebpagesUsers webpages_Users { get; set; }
        public FilaWebpagesUsers webpages_UsersInsert { get; set; }

        public override string ToString()
        {
            return grupo_empresa.ds_nome.ToUpper() + " //  " +
                empresa.ds_fantasia.ToUpper() + (empresa.filial == null ? "" : " " + empresa.filial.ToUpper()) + " // " +
                tbAdquirente.nmAdquirente.ToUpper() + " (" +
                dsModalidade.ToUpper() + ") de " + dtInicio.ToShortDateString() + " a " + dtFim.ToShortDateString() +
                " => Inserido por " + webpages_UsersInsert.ds_login + " em " + dtInsert.ToShortDateString();
        }
    }
}