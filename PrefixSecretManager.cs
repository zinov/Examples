using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace AzureKeyVaultExample
{
    public class PrefixSecretManager : IKeyVaultSecretManager
    {
        private readonly string _appNamePrefix;
        public PrefixSecretManager(string appName)
        {
            _appNamePrefix = appName + "-";
        }

        public bool Load(SecretItem secret)
        {
            return secret.Identifier.Name.StartsWith(_appNamePrefix);
        }

        public string GetKey(SecretBundle secret)
        {
            return secret.SecretIdentifier.Name.Substring(_appNamePrefix.Length)
                .Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}
