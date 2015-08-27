using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Object
{
    public class FiltroMonitorCargas
    {
        private string data;
        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        private string status; // '0' : EM EXECUÇÃO | '1' : CARREGADO COM SUCESSO | '2' : CARREGADO COM ERRO | '3' : RE-EXECUÇÃO | '4' : ERRO DE SENHA | '7' : ELEGÍVEL | '-1' : NÃO CARREGADO
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        private Int32 idGrupo;
        public Int32 IdGrupo
        {
            get { return idGrupo; }
            set { idGrupo = value; }
        }

        private string nuCnpj;
        public string NuCnpj
        {
            get { return nuCnpj; }
            set { nuCnpj = value; }
        }

        private Int32 cdAdquirente;
        public Int32 CdAdquirente
        {
            get { return cdAdquirente; }
            set { cdAdquirente = value; }
        }


    }
}
