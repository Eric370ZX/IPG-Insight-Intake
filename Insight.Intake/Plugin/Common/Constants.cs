using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight.Intake.Plugin.Common
{
    public enum PluginStage
    {
        PreValidation = 10,
        PreOperation = 20,
        PostOperation = 40
    }
}
