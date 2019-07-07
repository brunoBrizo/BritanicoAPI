using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaBritanico.Modelo
{
    public class GrupoDia
    {
        public int ID { get; set; }
        public string Dia { get; set; }


        public GrupoDia() { }

        public GrupoDia(int id, string dia)
        {
            this.ID = id;
            this.Dia = dia;
        }
    }
}
