using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;

namespace BritanicoService
{
    partial class Britanico : ServiceBase
    {
        bool bandera = false;
        bool limpiar = false;
        bool deudores = false;
        bool email = false;

        public Britanico()
        {
            InitializeComponent();
            ApiHelper.InicializarCliente();
        }

        protected override void OnStart(string[] args)
        {
            lapsoEmail.Start();
            lapsoDeudores.Start();
            lapsoLimpiarEstudiante.Start();
        }

        protected override void OnStop()
        {
            lapsoEmail.Stop();
            lapsoLimpiarEstudiante.Stop();
            lapsoDeudores.Stop();
        }


        private async void LapsoLimpiarEstudiante_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            EventLog.WriteEntry("Ejecuta el metodo de limpiar estudiantes al iniciar el año", EventLogEntryType.Information);
            if (DateTime.Now.Month == 1 && DateTime.Now.Day == 1)
            {
                if (!limpiar)
                {
                    limpiar = true;
                    if (bandera) return;
                    try
                    {
                        string url = $"{ UtilidadController.Url }estudiante/MarcarEstudiantesInactivosSinGrupoSinConvenio";
                        EventLog.WriteEntry(url, EventLogEntryType.Warning);
                        bandera = true;
                        bool res = await UtilidadController.LimpiarEstudiante();
                        if (!res)
                            EventLog.WriteEntry("No se ejecuto el metodo para limpiar datos de los estudiantes", EventLogEntryType.Error);
                        else
                        {
                            EventLog.WriteEntry("Se limpiaron los datos de los estudiantes", EventLogEntryType.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                    }
                    bandera = false;
                }
            }
            else
            {
                limpiar = false;
            }
        }

        private async void LapsoDeudores_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            EventLog.WriteEntry("Ejecuta el metodo de actualizar deudores", EventLogEntryType.Information);
            if (DateTime.Now.Day == 11)
            {
                if (!deudores)
                {
                    deudores = true;
                    if (bandera) return;
                    try
                    {
                        bandera = true;
                        bool res = await UtilidadController.ActualizarEstudiantesDeudores();
                        if (!res)
                            EventLog.WriteEntry("No se ejecuto el metodo para actualizar deudores", EventLogEntryType.Error);
                        else
                        {
                            EventLog.WriteEntry("Se actualizaron los deudores", EventLogEntryType.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                    }
                    bandera = false;
                }
            }
            else
            {
                deudores = false;
            }
        }

        private async void LapsoEmail_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            EventLog.WriteEntry("Ejecuta el metodo para enviar email por vencimiento de mensualidades", EventLogEntryType.Information);
            if (DateTime.Now.Day == 8)
            {
                if (!email)
                {
                    email = true;
                    if (bandera) return;
                    try
                    {
                        bandera = true;
                        bool res = await UtilidadController.EnviarMailVencimientoMensualidad();
                        if (!res)
                            EventLog.WriteEntry("No se ejecuto el metodo para enviar email por vencimiento de mensualidades", EventLogEntryType.Error);
                        else
                        {
                            EventLog.WriteEntry("Se envio email por vencimiento de mensualidades", EventLogEntryType.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                    }
                    bandera = false;
                }
            }
            else
            {
                email = false;
            }
        }

    }
}
