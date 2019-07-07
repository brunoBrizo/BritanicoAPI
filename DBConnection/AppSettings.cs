using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace DBConnection
{
    public class AppSettings
    {
        private static string rutaConexion = @"C:\BritanicoApp\Conexion\connection.txt";
        private static Configuration config = ConfigurationManager.OpenExeConfiguration(AppSettings.rutaConexion);

        public AppSettings() { }


        public static string GetConnectionStringEncriptado(string key)
        {
            string strConexion = "";
            try
            {
                StreamReader sr = new StreamReader(AppSettings.rutaConexion);
                strConexion = sr.ReadLine();
                sr.Close();
                return strConexion;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool GuardarConnectionString(string value)
        {
            try
            {
                if (!File.Exists(AppSettings.rutaConexion))
                {
                    StreamWriter auxST = File.CreateText(AppSettings.rutaConexion);
                    auxST.Close();
                }
                StreamWriter sw = new StreamWriter(AppSettings.rutaConexion, false);
                sw.Write(value);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetConexionStringDesencriptado()
        {
            string strConexion = "";
            try
            {
                StreamReader sr = new StreamReader(AppSettings.rutaConexion);
                strConexion = sr.ReadLine();
                sr.Close();
                strConexion = Encriptacion.Desencriptar(strConexion);
                return strConexion;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
