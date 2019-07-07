using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaBritanico.Modelo
{
    public class ExamenEstudianteCuota
    {
        public int ID { get; set; }
        public int NroCuota { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaPago { get; set; }
        public bool CuotaPaga { get; set; }

        public ExamenEstudianteCuota(int ID, int NroCuota, decimal Precio, bool CuotaPaga)
        {
            this.ID = ID;
            this.NroCuota = NroCuota;
            this.Precio = Precio;
            this.FechaPago = FechaPago;
            this.CuotaPaga = CuotaPaga;
        }

        public ExamenEstudianteCuota() { }

    }
}
