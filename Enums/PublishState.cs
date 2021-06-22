using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AmadeusBlog.Enums
{
    public enum PublishState
    {
        [Description("Production Ready")]
        ProductionReady,
        PreviewReady,
        Notready
    }
}
