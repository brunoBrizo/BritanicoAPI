using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibliotecaBritanico.Utilidad
{
    public abstract class Herramientas
    {
        public static bool ValidarCedula(string cedula)
        {
            bool retorno = false;
            if (cedula.Length < 7) return false;
            if (cedula.Length == 7) cedula = "0" + cedula;
            int a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0, contador = 0, restar = 0;
            int.TryParse(cedula[0].ToString(), out a);
            int.TryParse(cedula[1].ToString(), out b);
            int.TryParse(cedula[2].ToString(), out c);
            int.TryParse(cedula[3].ToString(), out d);
            int.TryParse(cedula[4].ToString(), out e);
            int.TryParse(cedula[5].ToString(), out f);
            int.TryParse(cedula[6].ToString(), out g);
            int.TryParse(cedula[7].ToString(), out h);
            int aux = (a * 2) + (b * 9) + (c * 8) + (d * 7) + (e * 6) + (f * 3) + (g * 4);
            for (int i = aux; contador < 1; i = i + 1)
            {
                int resto = i % 10;
                if (resto == 0)
                {
                    contador = 1;
                    restar = i;
                }
            }
            if (h == (restar - aux)) retorno = true;
            return retorno;
        }

        public static bool ValidarMail(string mail)
        {
            string expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(mail, expresion))
            {
                if (Regex.Replace(mail, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool ValidarPassword(string password)
        {
            if (password.Length <= 5)
            {
                return false;
            }
            return true;
        }

        public static bool ValidarRUT(string RUT)
        {
            bool ok = true;
            int DigRut, Digito, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, D11, Total;
            decimal Aux, Resto;
            if (RUT == "" || RUT.Length != 12)
            {
                ok = false;
            }
            else
            {
                int.TryParse(RUT.Substring(0, 1), out D1);
                int.TryParse(RUT.Substring(1, 1), out D2);
                int.TryParse(RUT.Substring(2, 1), out D3);
                int.TryParse(RUT.Substring(3, 1), out D4);
                int.TryParse(RUT.Substring(4, 1), out D5);
                int.TryParse(RUT.Substring(5, 1), out D6);
                int.TryParse(RUT.Substring(6, 1), out D7);
                int.TryParse(RUT.Substring(7, 1), out D8);
                int.TryParse(RUT.Substring(8, 1), out D9);
                int.TryParse(RUT.Substring(9, 1), out D10);
                int.TryParse(RUT.Substring(10, 1), out D11);

                Total = D1 * 4 + D2 * 3 + D3 * 2 + D4 * 9 + D5 * 8 + D6 * 7 + D7 * 6 + D8 * 5 + D9 * 4 + D10 * 3 + D11 * 2;
                Aux = Total / 11;
                Aux = Math.Floor(Aux);
                Resto = Total - (Aux * 11);

                if (Resto == 1)
                {
                    ok = false;
                }
                else
                {
                    if (Resto == 0)
                    {
                        Digito = 0;
                    }
                    else
                    {
                        int RestoInteger;
                        int.TryParse(Resto.ToString(), out RestoInteger);
                        Digito = 11 - RestoInteger;
                    }
                    int.TryParse(RUT.Substring(11, 1), out DigRut);
                    if (Digito != DigRut)
                    {
                        ok = false;
                    }
                }
            }
            return ok;
        }

        public static decimal ObtenerNumerador(string tipo, string strCon)
        {
            try
            {
                Numerador num = new Numerador();
                decimal valorRetorno = 0;

                if (tipo.Equals(String.Empty))
                {
                    return valorRetorno;
                }
                num.Tipo = tipo;
                if (num.Leer(strCon))
                {
                    valorRetorno = num.Valor + 1;
                    num.Valor += 1;
                }
                num.Modificar(strCon);
                return valorRetorno;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CrearLogError(string LogErrLocation, string LogErrMsg, LogErrorTipo LogErrTipo, string strCon)
        {
            try
            {
                LogError log = new LogError(LogErrLocation, LogErrMsg, LogErrTipo);
                log.Guardar(strCon);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
