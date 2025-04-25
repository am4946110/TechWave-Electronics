using Microsoft.AspNetCore.DataProtection;

namespace TechWave_Electronics.Models
{
    public class YourService
    {
        private readonly IKeyManager _keyManager;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public YourService(IKeyManager keyManager, IDataProtectionProvider dataProtectionProvider)
        {
            _keyManager = keyManager;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string EncryptData(string data)
        {
            var key = _keyManager.GetKey();
            var protector = _dataProtectionProvider.CreateProtector(key);
            return protector.Protect(data);
        }

        public string DecryptData(string encryptedData)
        {
            var key = _keyManager.GetKey();
            var protector = _dataProtectionProvider.CreateProtector(key);
            return protector.Unprotect(encryptedData);
        }
    }
}
