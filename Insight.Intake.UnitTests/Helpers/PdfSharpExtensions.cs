﻿extern alias PdfSharpUnitTests;

using PdfSharpUnitTests::PdfSharp.Pdf;
using PdfSharpUnitTests::PdfSharp.Pdf.Content;
using PdfSharpUnitTests::PdfSharp.Pdf.Content.Objects;
using System.Collections.Generic;

namespace Insight.Intake.UnitTests.Helpers
{
    public static class PdfSharpExtensions
    {
        public static string ExtractTextAsString(this PdfPage page)
        {
            IEnumerable<string> strings = page.ExtractText();
            if(strings != null)
            {
                return string.Join("", strings);
            }

            return "";
        }

        public static IEnumerable<string> ExtractText(this PdfPage page)
        {
            var content = ContentReader.ReadContent(page);
            var text = content.ExtractText();
            return text;
        }

        public static IEnumerable<string> ExtractText(this CObject cObject)
        {
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                        foreach (var txt in ExtractText(cOperand))
                            yield return txt;
                }
            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                    foreach (var txt in ExtractText(element))
                        yield return txt;
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                yield return cString.Value;
            }
        }
    }
}
