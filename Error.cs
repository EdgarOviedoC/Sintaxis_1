using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Sintaxis_1 {
    public class Error : Exception
    {
        public Error(string message, int line) : base( message +" en linea " + line) { }
        public Error(string message, StreamWriter log) : base("Error " + message) {
            log.WriteLine("Error " + message);
        }
        public Error(string message, StreamWriter log, int line) : base("Error: " + message + " on line " + line) {
            log.WriteLine("Error: " + message + " on line " + line);
        }
    }
}