using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;

namespace Qf.Extensions.Configuration.Encryption
{
    public static class EncryptionConfigurationExtensions
    {
        public static IConfigurationBuilder AddEncryptionFile(this IConfigurationBuilder builder, string path)
        {
            return AddEncryptionFile(builder, provider: null, path: path, password: path, optional: false, reloadOnChange: false);
        }
        public static IConfigurationBuilder AddEncryptionFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddEncryptionFile(builder, provider: null, path: path, password: path, optional: optional, reloadOnChange: false);
        }
        public static IConfigurationBuilder AddEncryptionFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddEncryptionFile(builder, provider: null, path: path, password: path, optional: optional, reloadOnChange: reloadOnChange);
        }
        public static IConfigurationBuilder AddEncryptionFile(this IConfigurationBuilder builder, IFileProvider provider, string path, string password, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.", nameof(path));
            }
            if (string.IsNullOrEmpty(password))
            {
                password = path;
            }
            return builder.AddEncryptionFile(s =>
            {
                s.Password = password;
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ResolveFileProvider();
            });
        }
        public static IConfigurationBuilder AddEncryptionFile(this IConfigurationBuilder builder, Action<EncryptionConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}
