using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BritanicoService
{
    public class UtilidadController
    {
        public static string Url { get; set; } = ConfigurationManager.AppSettings["UrlApi"].ToString();

        public static async Task<bool> LimpiarEstudiante()
        {
            string url = $"{ UtilidadController.Url }estudiante/MarcarEstudiantesInactivosSinGrupoSinConvenio";
            
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<bool> ActualizarEstudiantesDeudores()
        {
            string url = $"{ UtilidadController.Url }estudiante/ActualizarEstudiantesDeudores";

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<bool> EnviarMailVencimientoMensualidad()
        {
            string url = $"{ UtilidadController.Url }email/EnviarMailVencimientoMensualidad";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
}
