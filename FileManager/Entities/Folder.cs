using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Entities
{
    public class Folder
    {
        public String Fullpath { get; set; }
        public String Name { get; set; }
        public DateTime LastChange { get; set; }

        public Folder(String fullpath, DateTime lastChange)
        {
            Fullpath = fullpath;
            Name = fullpath.Substring(fullpath.LastIndexOf('\\') + 1);
            LastChange = lastChange;
        }
    }
}
