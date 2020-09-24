using Microsoft.Extensions.Configuration;

namespace Qf.Extensions.Configuration.Encryption
{
    public class EncryptionConfigurationSource : FileConfigurationSource
    {
        public string Password { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new EncryptionConfigurationProvider(this);
        }
    }
}
