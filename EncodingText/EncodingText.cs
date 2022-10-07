namespace EncodingText
{
    public static class CustomEncoding
    {
        private readonly static ushort _key = 0x0088;

        public static string EncodeDecrypt(string str)
        {
            string newStr = "";  
            foreach (var c in str.ToArray())  
                newStr += TopSecret(c, _key);
            return newStr;
        }

        private static char TopSecret(char character, ushort secretKey)
        {
            character = (char)(character ^ secretKey);
            return character;
        }
    }
}