using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DupeList
{
    public static class ExcludeFiles
    {
        static string[] EXCLUDE_FILES = new string[]
        {
            "desktop.ini",
            "thumbs.db"
        };

        static string[] LOWER_PRIORITY_FILES = new string[]
        {
            "(", ")", "copy of", "- copy"
        };

        public static bool IsExcludedFile(string file)
        {
            foreach (string ex in EXCLUDE_FILES)
            {
                if (ex == file) return true;
            }
            return false;
        }

        public static bool IsLowerPriority(string file)
        {
            string lowerFile = file.ToLower();
            foreach (string ex in LOWER_PRIORITY_FILES)
            {
                if (lowerFile.Contains(ex)) return true;
            }
            return false;
        }
    }
}
