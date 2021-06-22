using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmadeusBlog.Enums
{
    public enum BlogRoles
    {
        [Display(Name ="Admin")]
        Administrator,

        [Display(Name = "Mod")]
        Moderator,

        //[Display(Name ="Blogger")]
        //Blogger
    }
}
