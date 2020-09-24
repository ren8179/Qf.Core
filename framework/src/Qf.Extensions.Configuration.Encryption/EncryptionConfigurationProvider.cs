using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Qf.Extensions.Configuration.Encryption
{
    public class EncryptionConfigurationProvider : FileConfigurationProvider
    {
        public EncryptionConfigurationProvider(EncryptionConfigurationSource source) : base(source)
        {
            _password = source.Password;
        }

        private readonly string _password;

        public override void Load(Stream stream)
        {
            try
            {
                Data = EncryptionConfigurationFileParser.Parse(stream, _password);
            }
            catch (Exception e)
            {
                throw new FormatException(e.Message);
            }
        }
    }
}
