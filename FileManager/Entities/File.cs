using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Entities
{
    public class File
    {
        public String Fullpath { get; set; }
        public String Name { get; set; }
        public DateTime LastChange { get; set; }
        public long Length { get; set; }

        public File(String fullpath, DateTime lastChange, long length)
        {
            Fullpath = fullpath;
            Name = fullpath.Substring(fullpath.LastIndexOf('\\') + 1);
            LastChange = lastChange;
            Length = length;
        }
    }
}
