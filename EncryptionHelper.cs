using System.Text;

namespace SAAS_Query_API
{
    public class EncryptionHelper
    {
            public static string ToHexString(string str)
            {
                var sb = new StringBuilder();

                var bytes = Encoding.Unicode.GetBytes(str);
                foreach (var t in bytes)
                {
                    sb.Append(t.ToString("X2"));
                }

                return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
            }
            public static string FromHexString(string hexString)
            {
                var bytes = new byte[hexString.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
            }
            public static string Encrypt(string txtValue, string Key = "AmitLalJoshi")
            {
                try
                {
                    int i;
                    string TextChar;
                    string KeyChar;

                    string retMsg = "";
                    int ind = 1;

                    for (i = 1; i <= Convert.ToInt32(txtValue.Length); i++)
                    {
                        TextChar = txtValue.Substring(i - 1, 1);
                        ind = i % Key.Length;
                        KeyChar = Key.Substring((ind));
                        byte str1 = Encoding.ASCII.GetBytes(TextChar)[0];
                        byte str2 = Encoding.ASCII.GetBytes(KeyChar)[0];
                        var encData = str1 ^ str2;
                        retMsg = retMsg + Convert.ToChar(encData).ToString();
                    }

                    return retMsg;
                }
                catch (Exception ex)
                {
                    return "error";
                }
            }
    }
}
