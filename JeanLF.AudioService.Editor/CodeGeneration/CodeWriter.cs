using System.Collections.Generic;
using System.IO;

namespace JeanLF.AudioService.Editor
{
    public static class CodeWriter
    {
        public static void WriteEnum(string filePath, string enumName, IEnumerable<string> items)
        {
            StreamWriter file = new StreamWriter(filePath,false);

            SourceFile sourceFile = new SourceFile();
            using (new NamespaceScope(sourceFile, AudioServiceEditorUtils.AudioServiceNamespace))
            {
                sourceFile.AppendLine($"public enum {enumName}");

                using (new BracesScope(sourceFile))
                {
                    foreach (string member in items)
                    {
                        if (string.IsNullOrWhiteSpace(member))
                        {
                            continue;
                        }
                        sourceFile.AppendLine($"{member},");
                    }
                }
            }

            file.Write(sourceFile.ToString());
            file.Close();
        }
    }
}
