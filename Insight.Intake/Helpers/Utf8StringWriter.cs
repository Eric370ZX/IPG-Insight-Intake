using System.IO;
using System.Text;

namespace Insight.Intake.Helpers
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
