using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcidCode.Core.Helpers
{
    internal static class SyntaxTreeHelpers
    {
        public static Task<SyntaxTree> ParseTextTaskAsync(string text)
        {
            SyntaxTree result = null;

            return Task.Run<SyntaxTree>(() =>
            {
                result = CSharpSyntaxTree.ParseText(text);

                return result;
            });
        }

    }
}
