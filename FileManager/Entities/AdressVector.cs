using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Entities
{
    class AdressVector
    {
        public String Source { get; private set; }
        public String Destination { get; private set; }

        public AdressVector(String source, String destination)
        {
            Source = source;
            Destination = destination;
        }
    }
}
