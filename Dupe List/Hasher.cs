using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace DupeList
{
    public static class Hasher
    {
        static MD5 md = MD5.Create();

        public static string Md5(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return bytesToHex(md.ComputeHash(stream));
            }
        }

        private static string bytesToHex(byte[] bytes)
        {
            string ret = "";
            foreach (byte b in bytes)
            {
                ret += b.ToString("x2").ToLower();
            }
            return ret;
        }
    }
}
