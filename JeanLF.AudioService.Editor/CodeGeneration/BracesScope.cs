using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JeanLF.AudioService.Editor
{
    public class BracesScope : IDisposable
    {
        private readonly SourceFile _source;

        public BracesScope(SourceFile source)
        {
            _source = source;
            _source.AppendLine("{");
            _source.Indentator.Add();
        }

        public void Dispose()
        {
            _source.Indentator.Sub();
            _source.AppendLine("}");
        }
    }
}
