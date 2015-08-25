using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class webpages_RoleLevels
    {
        public webpages_RoleLevels()
        {
            this.webpages_Roles = new List<webpages_Roles>();
        }

        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public virtual ICollection<webpages_Roles> webpages_Roles { get; set; }
    }
}
