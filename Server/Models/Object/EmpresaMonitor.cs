using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models.Object
{
    public class EmpresaMonitor
    {
        private string cnpj;
        public string nu_cnpj
        {
            get { return cnpj; }
            set { cnpj = value; }
        }

        private string fantasia;
        public string ds_fantasia
        {
            get { return fantasia; }
            set { fantasia = value; }
        }

        private string filialnum;
        public string filial
        {
            get { return filialnum; }
            set { filialnum = value; }
        }
    }
}
