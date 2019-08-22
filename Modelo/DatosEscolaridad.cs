using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BibliotecaBritanico.Modelo
{
    public class DatosEscolaridad
    {
        public Materia Materia { get; set; }
        public Grupo Grupo { get; set; }
        public ExamenEstudiante ExamenEstudiante { get; set; }
    }
}
