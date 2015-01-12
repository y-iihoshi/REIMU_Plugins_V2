using System.Linq;
using System.Text;

namespace ReimuPlugins.Common
{
    public static class StringExtensions
    {
        public static string ToCStr(this string str)
        {
            return str.Contains('\0') ? str : str + '\0';
        }

        public static string Convert(this string str, Encoding src, Encoding dst)
        {
            return dst.GetString(Encoding.Convert(src, dst, src.GetBytes(str)));
        }
    }
}
