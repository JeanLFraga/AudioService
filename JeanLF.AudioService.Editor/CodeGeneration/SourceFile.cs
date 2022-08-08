using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JeanLF.AudioService.Editor
{
    public class Indentator
    {
        private StringBuilder _builder;

        private int _lastAmount = 0;
        private int _amount = 0;

        public Indentator()
        {
            _builder = new StringBuilder();
        }

        public void Add()
        {
            _amount++;
        }

        public void Sub()
        {
            _amount--;
        }

        public override string ToString()
        {
            if (_lastAmount == _amount)
            {
                return _builder.ToString();
            }

            _builder.Clear();
            for (int i = 0; i < _amount; i++)
            {
                _builder.Append("    ");
            }

            _lastAmount = _amount;
            return _builder.ToString();
        }
    }

    public class SourceFile
    {
        public readonly Indentator Indentator = new Indentator();
        private readonly StringBuilder _builder = new StringBuilder();

        public void AppendLine(string content = "")
        {
            _builder.AppendLine($"{Indentator}{content}");
        }

        public void Append(string content)
        {
            _builder.Append($"{Indentator}{content}");
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
