using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaBritanico.Utilidad
{
    public class ValidacionException : Exception
    {
        public string ValidacionStackTrace { get; set; }

        public ValidacionException(string msg) : base(msg) { }

        public ValidacionException(string msg, string stackTrace) : base(msg)
        {
            this.ValidacionStackTrace = stackTrace;
        }
    }
}
