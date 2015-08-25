using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class webpages_UsersInRoles
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public bool RolePrincipal { get; set; }
        public virtual webpages_Membership webpages_Membership { get; set; }
        public virtual webpages_Roles webpages_Roles { get; set; }
    }
}
