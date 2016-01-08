using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class tbFilaBootICard
    {

        public int id { get; set; }
        public string nrCNPJ { get; set; }
        public int cdAdquirente { get; set; }
        public string dsModalidade { get; set; }
        public DateTime dtInicio { get; set; }
        public DateTime dtFim { get; set; }
        public byte stProcessamento { get; set; }
        public Nullable<int> cdUser { get; set; }
        public Nullable<DateTime> dtProcessamento { get; set; }
        public DateTime dtInsert { get; set; }
        public int cdUserInsert { get; set; }
        public virtual empresa empresa { get; set; }
        public virtual tbAdquirente tbAdquirente { get; set; }
        public virtual webpages_Users webpages_Users { get; set; }
        public virtual webpages_Users webpages_UsersInsert { get; set; }
    }
}
