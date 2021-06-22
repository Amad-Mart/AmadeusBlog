using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmadeusBlog.Enums
{
    public enum ModerationType
    {
        [Display(Name ="Political Propaganda")]
        PoliticalPropaganda,
        [Display(Name ="Offensive")]
        Language,
        [Display(Name ="Drug References")]
        Drugs,
        [Display(Name ="Threatening Speech")]
        Threatening,
        [Display(Name ="Sexual Content")]
        Sexual,
        [Display(Name ="Fraud")]
        Fraud
    }
}
