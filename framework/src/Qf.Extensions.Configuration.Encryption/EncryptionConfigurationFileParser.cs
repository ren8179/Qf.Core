using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Qf.Extensions.Configuration.Encryption
{
    internal class EncryptionConfigurationFileParser
    {
        private EncryptionConfigurationFileParser() { }

        private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Stack<string> _context = new Stack<string>();
        private string _currentPath;

        public static IDictionary<string, string> Parse(Stream input, string password)
            => new EncryptionConfigurationFileParser().ParseStream(input, password);

        private IDictionary<string, string> ParseStream(Stream input, string password)
        {
            _data.Clear();

            var jsonReaderOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };
            byte[] buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length);
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            var bytesDecrypted = AES.GetDecryptedByteArray(buffer, passwordBytes);
            var content = Encoding.GetEncoding("gb2312").GetString(bytesDecrypted);
            content = content.Replace("\0", "");
            using (var doc = JsonDocument.Parse(content, jsonReaderOptions))
            {
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                    throw new FormatException($"Unsupported JSON token '{doc.RootElement.ValueKind}' was found.");
                VisitElement(doc.RootElement);
            }
            return _data;
        }

        private void VisitElement(JsonElement element)
        {
            foreach (var property in element.EnumerateObject())
            {
                EnterContext(property.Name);
                VisitValue(property.Value);
                ExitContext();
            }
        }

        private void VisitValue(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    VisitElement(value);
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var arrayElement in value.EnumerateArray())
                    {
                        EnterContext(index.ToString());
                        VisitValue(arrayElement);
                        ExitContext();
                        index++;
                    }
                    break;

                case JsonValueKind.Number:
                case JsonValueKind.String:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    var key = _currentPath;
                    if (_data.ContainsKey(key))
                    {
                        throw new FormatException($"A duplicate key '{key}' was found.");
                    }
                    _data[key] = value.ToString();
                    break;

                default:
                    throw new FormatException($"Unsupported JSON token '{value.ValueKind}' was found.");
            }
        }

        private void EnterContext(string context)
        {
            _context.Push(context);
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }

        private void ExitContext()
        {
            _context.Pop();
            _currentPath = ConfigurationPath.Combine(_context.Reverse());
        }
    }
}
