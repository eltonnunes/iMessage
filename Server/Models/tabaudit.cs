using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class tabaudit
    {
        public string cod_usuario { get; set; }
        public string data { get; set; }
        public string hora { get; set; }
        public string modulo { get; set; }
        public string codacao { get; set; }
        public string dadoacao { get; set; }
    }
}
