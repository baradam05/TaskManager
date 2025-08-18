using System.Security.Cryptography;
using System.Text;
//https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-9.0&utm_source=chatgpt.com
public static class PasswordDecoder
{
    private static readonly string key = "1234567910111213";
    private static readonly string iv = "abcdefghijklmnop";

    public static string Encode(string password)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor();

            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (StreamWriter sw = new StreamWriter(cs))
            {
                sw.Write(password);
                sw.Close();
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string Decode(string password)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor();

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(password)))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
