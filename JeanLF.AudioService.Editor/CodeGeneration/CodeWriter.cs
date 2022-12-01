using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeanLF.AudioService.Editor
{
    public static class CodeWriter
    {
        public static bool WriteEnum(string filePath, string enumName, IEnumerable<string> items)
        {
            IEnumerable<string> filtered = items.GroupBy(name => name).Select(nameGroup => nameGroup.Key);
            FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter stream = new StreamWriter(file, Encoding.ASCII, 1024, true);

            SourceFile sourceFile = new SourceFile();
            using (new NamespaceScope(sourceFile, AudioServiceEditorUtils.CoreNamespace))
            {
                sourceFile.AppendLine($"public enum {enumName}");

                using (new BracesScope(sourceFile))
                {
                    foreach (string member in filtered)
                    {
                        if (string.IsNullOrWhiteSpace(member))
                        {
                            continue;
                        }
                        sourceFile.AppendLine($"{member},");
                    }
                }
            }

            string content = sourceFile.ToString();

            stream.Write(content);
            stream.Dispose();
            bool result = IsContentEqual(file, content);
            file.Close();

            return result;
        }

        private static bool IsContentEqual(FileStream file, string content)
        {
            long oldPos = file.Position;
            int fileByte;
            int contentByte;

            byte[] bytes = Encoding.ASCII.GetBytes(content);
            file.Seek(0, SeekOrigin.Begin);

            if (file.Length != bytes.Length)
            {
                return false;
            }

            int index = 0;
            do
            {
                fileByte = file.ReadByte();
                contentByte = bytes[index];

                index++;
            }
            while ((fileByte == contentByte && fileByte != -1) && index < bytes.Length - 1);

            file.Position = oldPos;
            return ((fileByte - contentByte) == 0);
        }
    }
}
