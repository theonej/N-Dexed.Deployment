namespace N_Dexed.Deployment.Security
{
    public interface IEncryptor
    {
        string EncryptValue(string value);
        string DecryptValue(string encryptedValue);
    }
}
