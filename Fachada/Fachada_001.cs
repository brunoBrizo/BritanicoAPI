using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaBritanico.Modelo;
using System.Data.SqlClient;
using DBConnection;
using BibliotecaBritanico.Utilidad;
using System.IO;

namespace BibliotecaBritanico.Fachada
{
    public class Fachada_001
    {
        //EMPRESA, SUCURSAL, MATERIAS Y GRUPOS SE MANTIENEN EN MEMORIA CUANDO LEVANTA LA APP
        private static Fachada_001 fachada = null;
        private static string Conexion { get; set; } = AppSettings.GetConexionStringDesencriptado();

        private Fachada_001() { }

        public static Fachada_001 getInstancia()


        {
            if (fachada == null)
            {
                fachada = new Fachada_001();
            }
            return fachada;
        }


        //pendientes
        #region Email

        public async Task<Email> CrearEmail(Email email)
        {
            try
            {
                if (Email.ValidarEmail(email))
                {
                    email.Enviado = true;
                    if (email.Guardar(Fachada_001.Conexion))
                    {
                        //datos de email
                        Parametro paramEmail = new Parametro
                        {
                            ID = 1
                        };
                        paramEmail.Leer(Fachada_001.Conexion);
                        Parametro paramClave = new Parametro
                        {
                            ID = 3
                        };
                        paramClave.Leer(Fachada_001.Conexion);
                        await email.Enviar(Fachada_001.Conexion, paramEmail, paramClave, String.Empty);
                        return email;
                    }

                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en CrearEmail | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando email | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en CrearEmail | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando email | Error: " + ex.Message);
            }
        }

        public async Task<bool> EnviarMailPorGrupo(Email email, Grupo grupo)
        {
            try
            {
                bool ret = await email.EnviarMailPorGrupo(grupo, Fachada_001.Conexion);
                return ret;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en EnviarMailPorGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error enviando email al grupo | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en EnviarMailPorGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error enviando email al grupo | Error: " + ex.Message);
            }
        }

        public async Task<bool> EnviarMailVencimientoMensualidad()
        {
            try
            {
                Email email = new Email();
                bool ret = await email.EnviarMailVencimientoMensualidad(Fachada_001.Conexion);
                return ret;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en EnviarMailVencimientoMensualidad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                return false;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en EnviarMailVencimientoMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                return false;
            }
        }

        public bool ModificarEmail(Email email)
        {
            try
            {
                if (Email.ValidarEmail(email))
                {
                    if (Email.ExisteEmail(email, Fachada_001.Conexion))
                    {
                        if (email.Modificar(Fachada_001.Conexion))
                            return true;
                    }
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en ModificarEmail | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando email | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en ModificarEmail | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando email | Error: " + ex.Message);
            }
        }

        public bool EliminarEmail(Email email)
        {
            try
            {
                if (Email.ExisteEmail(email, Fachada_001.Conexion))
                {
                    if (email.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en EliminarEmail | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error eliminando email | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en EliminarEmail | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando email | Error: " + ex.Message);
            }
        }

        public List<Email> ObtenerEmails()
        {
            try
            {
                Email email = new Email();
                List<Email> lstEmails = email.GetAll(Fachada_001.Conexion);
                return lstEmails;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en ObtenerEmails | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo emails | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en ObtenerEmails | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo emails | Error: " + ex.Message);
            }
        }

        public List<Email> ObtenerEmailsEntreFechas(DateTime desde, DateTime hasta)
        {
            try
            {
                Email email = new Email();
                List<Email> lstEmails = email.GetEntreFechas(desde, hasta, Fachada_001.Conexion);
                return lstEmails;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en ObtenerEmailsEntreFechas | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo emails entre fechas | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en ObtenerEmailsEntreFechas | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo emails entre fechas | Error: " + ex.Message);
            }
        }

        public Email GetEmail(Email email)
        {
            try
            {
                if (email.Leer(Fachada_001.Conexion))
                    return email;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en GetEmail | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo email | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en GetEmail | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo email | Error: " + ex.Message);
            }
        }

        public async Task EnviarEmailsPendientes()
        {
            try
            {
                //datos de email
                Parametro paramEmail = new Parametro
                {
                    ID = 1
                };
                paramEmail.Leer(Fachada_001.Conexion);
                Parametro paramClave = new Parametro
                {
                    ID = 3
                };
                paramClave.Leer(Fachada_001.Conexion);
                Email emailAux = new Email();
                List<Email> lstEmails = emailAux.GetPendientesEntreFechas(DateTime.Now.AddDays(-7), DateTime.Now, Fachada_001.Conexion);
                foreach (Email email in lstEmails)
                {
                    await email.Enviar(Fachada_001.Conexion, paramEmail, paramClave, String.Empty);
                    email.Enviado = true;
                    email.Modificar(Fachada_001.Conexion);
                }
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Email", "Error en EnviarEmailsPendientes | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error enviando emails pendientes | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en EnviarEmailsPendientes | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error enviando emails pendientes | Error: " + ex.Message);
            }
        }


        #endregion


        #region Convenio

        public Convenio CrearConvenio(Convenio convenio)
        {
            try
            {
                if (Convenio.ValidarConvenioInsert(convenio, Fachada_001.Conexion))
                {
                    if (convenio.Guardar(Fachada_001.Conexion))
                        return convenio;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en CrearConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando convenio | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en CrearConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando convenio | Error: " + ex.Message);
            }
        }

        public bool ModificarConvenio(Convenio convenio)
        {
            try
            {
                if (Convenio.ValidarConvenioModificar(convenio, Fachada_001.Conexion))
                {
                    if (convenio.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ModificarConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando convenio | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ModificarConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando convenio | Error: " + ex.Message);
            }
        }

        public bool EliminarConvenio(Convenio convenio)
        {
            try
            {
                if (convenio.LeerLazy(Fachada_001.Conexion))
                {
                    if (convenio.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en EliminarConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("No se puede eliminar el convenio | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en EliminarConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando convenio | Error: " + ex.Message);
            }
        }

        public List<Convenio> ObtenerConvenios()
        {
            try
            {
                Convenio convenio = new Convenio();
                List<Convenio> lstConvenios = convenio.GetAll(Fachada_001.Conexion);
                return lstConvenios;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ObtenerConvenios | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo convenios | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ObtenerConvenios | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo convenios | Error: " + ex.Message);
            }
        }

        public List<Convenio> ObtenerConveniosByAnio(int anio)
        {
            try
            {
                Convenio convenio = new Convenio();
                List<Convenio> lstConvenios = convenio.GetAllByAnio(anio, Fachada_001.Conexion);
                return lstConvenios;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ObtenerConveniosByAnio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo convenios por año | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ObtenerConveniosByAnio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo convenios por año | Error: " + ex.Message);
            }
        }

        public Convenio GetConvenio(Convenio convenio)
        {
            try
            {
                if (convenio.Leer(Fachada_001.Conexion))
                    return convenio;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en GetConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo convenio | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en GetConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo convenio | Error: " + ex.Message);
            }
        }

        public decimal GetMontoAPagarPorConvenio(Convenio convenio)
        {
            try
            {
                if (convenio.Leer(Fachada_001.Conexion))
                    return convenio.GetMontoAPagar(Fachada_001.Conexion);
                throw new ValidacionException("No existe el convenio");
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en GetMontoAPagarPorConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo monto a pagar por convenio | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en GetMontoAPagarPorConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo monto a pagar por convenio | Error: " + ex.Message);
            }
        }

        #endregion


        #region Parametro

        public Parametro CrearParametro(Parametro parametro)
        {
            try
            {
                if (Parametro.ValidarParametro(parametro, Fachada_001.Conexion))
                {
                    if (parametro.Guardar(Fachada_001.Conexion))
                        return parametro;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en CrearParametro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando parametro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en CrearParametro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando parametro | Error: " + ex.Message);
            }
        }

        public bool ModificarParametro(Parametro parametro)
        {
            try
            {
                if (Parametro.ExisteParametroByID(parametro.ID, Fachada_001.Conexion))
                {
                    if (parametro.Modificar(Fachada_001.Conexion))
                        return true;
                }
                else
                {
                    throw new ValidacionException("No existe el parametro que desea modificar");
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en ModificarParametro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando parametro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en ModificarParametro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando parametro | Error: " + ex.Message);
            }
        }

        public bool EliminarParametro(Parametro parametro)
        {
            try
            {
                if (Parametro.ExisteParametroByID(parametro.ID, Fachada_001.Conexion))
                {
                    if (parametro.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en EliminarParametro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error eliminando parametro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en EliminarParametro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando parametro | Error: " + ex.Message);
            }
        }

        public List<Parametro> ObtenerParametros()
        {
            try
            {
                Parametro parametro = new Parametro();
                List<Parametro> lstParametros = parametro.GetAll(Fachada_001.Conexion);
                return lstParametros;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en ObtenerParametros | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo parametros | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en ObtenerParametros | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo parametros | Error: " + ex.Message);
            }
        }

        public Parametro GetParametro(Parametro parametro)
        {
            try
            {
                if (parametro.Leer(Fachada_001.Conexion))
                    return parametro;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en GetParametro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo parametro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en GetParametro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo parametro | Error: " + ex.Message);
            }
        }

        #endregion


        #region Pago

        public Pago CrearPago(Pago pago)
        {
            try
            {
                if (Pago.ValidarPago(pago))
                {
                    if (pago.Guardar(Fachada_001.Conexion))
                        return pago;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Pago", "Error en CrearPago | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando pago | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en CrearPago | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando pago | Error: " + ex.Message);
            }
        }

        public bool ModificarPago(Pago pago)
        {
            try
            {
                if (Pago.ValidarPagoModificar(pago, Fachada_001.Conexion))
                {
                    if (pago.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Pago", "Error en ModificarPago | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando pago | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en ModificarPago | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando pago | Error: " + ex.Message);
            }
        }

        public bool EliminarPago(Pago pago)
        {
            try
            {
                if (pago.LeerLazy(Fachada_001.Conexion))
                {
                    if (pago.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Pago", "Error en EliminarPago | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error eliminando pago | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en EliminarPago | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando pago | Error: " + ex.Message);
            }
        }

        public List<Pago> ObtenerPagos()
        {
            try
            {
                Pago pago = new Pago();
                List<Pago> lstPagos = pago.GetAll(Fachada_001.Conexion);
                return lstPagos;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Pago", "Error en ObtenerPagos | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo pagos | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en ObtenerPagos | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo pagos | Error: " + ex.Message);
            }
        }

        public List<Pago> ObtenerPagos(DateTime desde, DateTime hasta, int concepto)
        {
            try
            {
                Pago pago = new Pago();
                List<Pago> lstPagos = pago.GetAll(desde, hasta, concepto, Fachada_001.Conexion);
                return lstPagos;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Pago", "Error en ObtenerPagos | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo pagos | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en ObtenerPagos | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo pagos | Error: " + ex.Message);
            }
        }

        public Pago GetPago(Pago pago)
        {
            try
            {
                if (pago.Leer(Fachada_001.Conexion))
                    return pago;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Pago", "Error en GetPago | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo pago | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en GetPago | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo pago | Error: " + ex.Message);
            }
        }

        #endregion


        #region Materia

        public Materia CrearMateria(Materia materia)
        {
            try
            {
                if (Materia.ValidarMateriaInsert(materia, Fachada_001.Conexion))
                {
                    if (materia.Guardar(Fachada_001.Conexion))
                        return materia;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Materia", "Error en CrearMateria | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando materia | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en CrearMateria | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando materia | Error: " + ex.Message);
            }
        }

        public bool ModificarMateria(Materia materia)
        {
            try
            {
                if (Materia.ValidarMateriaModificar(materia, Fachada_001.Conexion))
                {
                    if (materia.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ModificarMateria | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando materia | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ModificarMateria | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando materia | Error: " + ex.Message);
            }
        }

        public bool EliminarMateria(Materia materia)
        {
            try
            {
                if (materia.LeerLazy(Fachada_001.Conexion))
                {
                    if (materia.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Materia", "Error en EliminarMateria | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar la materia");
                }
                else
                {
                    throw new Exception("Error eliminando materia | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en EliminarMateria | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
            }
        }

        public List<Materia> ObtenerMaterias()
        {
            try
            {
                Materia materia = new Materia();
                List<Materia> lstMaterias = materia.GetAll(Fachada_001.Conexion);
                return lstMaterias;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ObtenerMaterias | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo materias | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ObtenerMaterias | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo materias | Error: " + ex.Message);
            }
        }

        public Materia GetMateria(Materia materia)
        {
            try
            {
                if (materia.Leer(Fachada_001.Conexion))
                    return materia;

                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Materia", "Error en GetMateria | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo materia | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en GetMateria | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo materia | Error: " + ex.Message);
            }
        }

        public List<MateriaHistorial> ObtenerMateriaHistorialByMateria(Materia materia)
        {
            try
            {
                return MateriaHistorial.GetAllByMateria(materia, Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ObtenerMateriaHistorialByMateria | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo historial de materia | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ObtenerMateriaHistorialByMateria | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo mathistorial de materia | Error: " + ex.Message);
            }
        }

        public List<MateriaHistorial> ObtenerMateriaHistorialByMateriaAnio(Materia materia, int anio)
        {
            try
            {
                return MateriaHistorial.GetAllByMateriaAnio(materia, anio, Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ObtenerMateriaHistorialByMateriaAnio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo historial de materia por año | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ObtenerMateriaHistorialByMateriaAnio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo mathistorial de materia por año | Error: " + ex.Message);
            }
        }

        public bool ModificarMateriaHistorialExamenPrecio(MateriaHistorial materiaHistorial)
        {
            try
            {
                if (MateriaHistorial.ExisteMateriaHistorial(materiaHistorial, Fachada_001.Conexion))
                    materiaHistorial.ModificarExamenPrecio(Fachada_001.Conexion);
                return true;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ModificarMateriaHistorialExamenPrecio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando precio de examen de la materia | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ModificarMateriaHistorialExamenPrecio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando precio de examen de la materia | Error: " + ex.Message);
            }
        }

        #endregion


        #region Libro

        public Libro CrearLibro(Libro libro)
        {
            try
            {
                if (Libro.ValidarInsertLibro(Fachada_001.Conexion, libro))
                {
                    if (libro.Guardar(Fachada_001.Conexion))
                        return libro;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Libro", "Error en CrearLibro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando libro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en CrearLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando libro | Error: " + ex.Message);
            }
        }

        public bool ModificarLibro(Libro libro)
        {
            try
            {
                if (Libro.ValidarModificarLibro(Fachada_001.Conexion, libro))
                {
                    if (libro.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Libro", "Error en ModificarLibro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando libro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en ModificarLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando libro | Error: " + ex.Message);
            }
        }

        public bool EliminarLibro(Libro libro)
        {
            try
            {
                if (Libro.ExisteLibro(Fachada_001.Conexion, libro))
                {
                    if (libro.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                else
                {
                    throw new ValidacionException("No existe el libro");
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Libro", "Error en EliminarLibro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar el libro");
                }
                else
                {
                    throw new Exception("Error eliminando libro | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en EliminarLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando libro | Error: " + ex.Message);
            }
        }

        public List<Libro> ObtenerLibros()
        {
            try
            {
                Libro libro = new Libro();
                List<Libro> lstLibros = libro.GetAll(Fachada_001.Conexion);
                return lstLibros;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Libro", "Error en ObtenerLibros | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo libros | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en ObtenerLibros | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo libros | Error: " + ex.Message);
            }
        }

        public Libro GetLibro(Libro libro)
        {
            try
            {
                if (libro.Leer(Fachada_001.Conexion))
                    return libro;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Libro", "Error en GetLibro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo libro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en GetLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo libro | Error: " + ex.Message);
            }
        }

        #endregion


        #region Empresa

        public Empresa CrearEmpresa(Empresa empresa)
        {
            try
            {
                if (Empresa.ValidarEmpresaInsert(empresa, Fachada_001.Conexion))
                {
                    if (empresa.Guardar(Fachada_001.Conexion))
                        return empresa;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en CrearEmpresa | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando empresa | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en CrearEmpresa | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando empresa | Error: " + ex.Message);
            }
        }

        public bool ModificarEmpresa(Empresa empresa)
        {
            try
            {
                if (Empresa.ValidarEmpresaModificar(empresa, Fachada_001.Conexion))
                {
                    if (empresa.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en ModificarEmpresa | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando empresa | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en ModificarEmpresa | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando empresa | Error: " + ex.Message);
            }
        }

        public bool EliminarEmpresa(Empresa empresa)
        {
            try
            {
                if (empresa.LeerLazy(Fachada_001.Conexion))
                {
                    if (empresa.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en EliminarEmpresa | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar la empresa");
                }
                else
                {
                    throw new Exception("Error eliminando empresa | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en EliminarEmpresa | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando empresa | Error: " + ex.Message);
            }
        }

        public List<Empresa> ObtenerEmpresas()
        {
            try
            {
                Empresa empresa = new Empresa();
                List<Empresa> lstEmpresas = empresa.GetAll(Fachada_001.Conexion);
                return lstEmpresas;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en ObtenerEmpresas | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo empresas | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en ObtenerEmpresas | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo empresas | Error: " + ex.Message);
            }
        }

        public Empresa GetEmpresa(Empresa empresa)
        {
            try
            {
                if (empresa.Leer(Fachada_001.Conexion))
                    return empresa;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en GetEmpresa | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo empresa | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en GetEmpresa | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo empresa | Error: " + ex.Message);
            }
        }

        #endregion


        #region Sucursal

        public Sucursal CrearSucursal(Sucursal sucursal)
        {
            try
            {
                if (Sucursal.ValidarSucursal(sucursal))
                {
                    if (sucursal.Guardar(Fachada_001.Conexion))
                        return sucursal;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en CrearSucursal | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando sucursal | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en CrearSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando sucursal | Error: " + ex.Message);
            }
        }

        public bool ModificarSucursal(Sucursal sucursal)
        {
            try
            {
                if (Sucursal.ValidarSucursal(sucursal))
                {
                    if (sucursal.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en ModificarSucursal | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando sucursal | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en ModificarSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando sucursal | Error: " + ex.Message);
            }
        }

        public bool EliminarSucursal(Sucursal sucursal)
        {
            try
            {
                if (sucursal.LeerLazy(Fachada_001.Conexion))
                {
                    if (sucursal.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en EliminarSucursal | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar la sucursal");
                }
                else
                {
                    throw new Exception("Error eliminando sucursal | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en EliminarSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando sucursal | Error: " + ex.Message);
            }
        }

        public List<Sucursal> ObtenerSucursales()
        {
            try
            {
                Sucursal sucursal = new Sucursal();
                List<Sucursal> lstSucursales = sucursal.GetAll(Fachada_001.Conexion);
                return lstSucursales;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en ObtenerSucursales | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo sucursales | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en ObtenerSucursales | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo sucursales | Error: " + ex.Message);
            }
        }

        public Sucursal GetSucursal(Sucursal sucursal)
        {
            try
            {
                if (sucursal.Leer(Fachada_001.Conexion))
                    return sucursal;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en GetSucursal | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo sucursal | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en GetSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo sucursal | Error: " + ex.Message);
            }
        }

        #endregion


        #region Estudiante

        public Estudiante CrearEstudiante(Estudiante estudiante)
        {
            try
            {
                if (Estudiante.ValidarEstudianteInsert(estudiante, Fachada_001.Conexion))
                {
                    if (estudiante.Guardar(Fachada_001.Conexion))
                        return estudiante;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en CrearEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en CrearEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando estudiante | Error: " + ex.Message);
            }
        }

        public bool ModificarEstudiante(Estudiante estudiante)
        {
            try
            {
                if (Estudiante.ValidarEstudianteModificar(estudiante, Fachada_001.Conexion))
                {
                    if (estudiante.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ModificarEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ModificarEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando estudiante | Error: " + ex.Message);
            }
        }

        public bool EliminarEstudiante(Estudiante estudiante)
        {
            try
            {
                if (Estudiante.ExisteEstudiante(estudiante, Fachada_001.Conexion))
                {
                    if (estudiante.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en EliminarEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar el estudiante");
                }
                else
                {
                    throw new Exception("Error eliminando estudiante | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en EliminarEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando estudiante | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudiantes()
        {
            try
            {
                Estudiante estudiante = new Estudiante();
                List<Estudiante> lstEstudiantes = estudiante.GetAll(Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantes | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantes | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudiantesActivos()
        {
            try
            {
                Estudiante estudiante = new Estudiante();
                List<Estudiante> lstEstudiantes = estudiante.GetAllActivos(Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesActivos | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes activos | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesActivos | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes activos | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudiantesNoActivos()
        {
            try
            {
                Estudiante estudiante = new Estudiante();
                List<Estudiante> lstEstudiantes = estudiante.GetAllNoActivos(Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesNoActivos | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes no activos | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesNoActivos | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes no activos | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudiantesNoValidados()
        {
            try
            {
                Estudiante estudiante = new Estudiante();
                List<Estudiante> lstEstudiantes = estudiante.GetAllNoValidados(Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesNoValidados | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes no validados | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesNoValidados | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes no validados | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudianteByNombre(Estudiante estudiante)
        {
            try
            {
                List<Estudiante> lstEstudiantes = Estudiante.LeerByNombre(estudiante.Nombre, Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudianteByNombre | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiante por nombre | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudianteByNombre | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiante por nombre | Error: " + ex.Message);
            }
        }

        public Estudiante GetEstudiante(Estudiante estudiante)
        {
            try
            {
                if (estudiante.Leer(Fachada_001.Conexion))
                    return estudiante;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiante | Error: " + ex.Message);
            }
        }

        public bool ExisteEstudiante(Estudiante estudiante)
        {
            try
            {
                return Estudiante.ExisteEstudiante(estudiante, Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ExisteEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error buscando estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ExisteEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error buscando estudiante | Error: " + ex.Message);
            }
        }

        public Estudiante GetEstudianteConMensualidad(Estudiante estudiante, int anioAsociado)
        {
            try
            {
                if (estudiante.LeerConMensualidad(Fachada_001.Conexion, anioAsociado))
                    return estudiante;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetEstudianteConMensualidad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error buscando estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetEstudianteConMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error buscando estudiante | Error: " + ex.Message);
            }
        }

        public List<Examen> GetExamenPendienteByEstudiante(Estudiante estudiante)
        {
            try
            {
                if (estudiante.Leer(Fachada_001.Conexion))
                {
                    if (estudiante.DebeExamen(Fachada_001.Conexion))
                    {
                        throw new ValidacionException("El estudiante tiene exámenes anteriores IMPAGOS");
                    }
                    return Examen.GetExamenPendientePorEstudiante(estudiante, Fachada_001.Conexion);
                }
                else
                    throw new ValidacionException("No existe el estudiante");
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetExamenPendienteByEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examen pendiente de un estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetExamenPendienteByEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examen pendiente de un estudiante | Error: " + ex.Message);
            }
        }

        public ExamenEstudiante GetExamenEstudiantePorRendir(Estudiante estudiante)
        {
            try
            {
                if (estudiante.Leer(Fachada_001.Conexion))
                {
                    return Estudiante.GetExamenEstudiantePorRendir(estudiante, Fachada_001.Conexion);
                }
                else
                    throw new ValidacionException("No existe el estudiante");
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetExamenEstudiantePorRendir | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examen por rendir de un estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetExamenEstudiantePorRendir | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examen por rendir de un estudiante | Error: " + ex.Message);
            }
        }

        public bool ModificarEstudianteGrupo(Estudiante estudiante)
        {
            try
            {
                if (estudiante.ModificarGrupo(estudiante.GrupoID, estudiante.MateriaID, Fachada_001.Conexion))
                    return true;
                return false;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ModificarEstudianteGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                string errorMsg = "Error al actualizar el Grupo del estudiante. Puede hacerlo manualmente desde el listado de Estudiantes";
                throw new Exception(errorMsg);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ModificarEstudianteGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                string errorMsg = "Error al actualizar el Grupo del estudiante. Puede hacerlo manualmente desde el listado de Estudiantes";
                throw new Exception(errorMsg);
            }
        }

        public List<Estudiante> ObtenerEstudiantesByConvenio(Convenio convenio)
        {
            try
            {
                List<Estudiante> lstEstudiantes = Estudiante.LeerByConvenio(convenio, Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesByConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes por convenio | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesByConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes por convenio | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudiantesConConvenio()
        {
            try
            {
                List<Estudiante> lstEstudiantes = Estudiante.LeerEstudiantesConConvenio(Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesConConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes con convenio | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesConConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes con convenio | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudiantesDeudores()
        {
            try
            {
                List<Estudiante> lstEstudiantes = Estudiante.GetDeudores(Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesDeudores | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes deudores | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesDeudores | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes deudores | Error: " + ex.Message);
            }
        }

        public ExamenEstudiante ObtenerExamenEstudianteCuotas(Estudiante estudiante)
        {
            try
            {
                estudiante.Leer(Fachada_001.Conexion);
                ExamenEstudiante examenEstudiante = Estudiante.GetExamenEstudianteCuotas(estudiante, Fachada_001.Conexion);
                return examenEstudiante;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerExamenEstudianteCuotas | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo el examen del estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerExamenEstudianteCuotas | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo el examen del estudiante | Error: " + ex.Message);
            }
        }

        public bool DarBajaEstudiante(Estudiante estudiante, int mes)
        {
            try
            {
                estudiante.Leer(Fachada_001.Conexion);
                if (estudiante.GrupoID < 1)
                {
                    throw new ValidacionException("El estudiante no esta asociado a ningun grupo");
                }
                bool ret = estudiante.DarDeBaja(Fachada_001.Conexion, mes);
                return ret;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en DarBajaEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error dando de baja a estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en DarBajaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error dando de baja a estudiante | Error: " + ex.Message);
            }
        }

        public List<DatosEscolaridad> ObtenerEscolaridad(Estudiante estudiante)
        {
            try
            {
                estudiante.Leer(Fachada_001.Conexion);
                List<DatosEscolaridad> lstDatosEscolaridad = Estudiante.GetEscolaridad(estudiante, Fachada_001.Conexion);
                return lstDatosEscolaridad;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEscolaridad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo escolaridad del estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEscolaridad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo escolaridad del estudiante | Error: " + ex.Message);
            }
        }

        public void MarcarEstudianteComoDeudor()
        {
            try
            {
                List<Estudiante> lstEstudiante = this.ObtenerEstudiantesDeudores();
                foreach (Estudiante estudiante in lstEstudiante)
                {
                    estudiante.SetDeudor(Fachada_001.Conexion, true);
                }
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en MarcarEstudianteComoDeudor | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en MarcarEstudianteComoDeudor | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
            }
        }

        public void MarcarEstudiantesInactivosSinGrupoSinConvenio()
        {
            try
            {
                Estudiante.SetInactivoSinGrupoSinConvenio(Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en MarcarEstudiantesInactivosSinGrupoSinConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                //throw new Exception("Error obteniendo escolaridad del estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en MarcarEstudiantesInactivosSinGrupoSinConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                //throw new Exception("Error obteniendo escolaridad del estudiante | Error: " + ex.Message);
            }
        }

        public List<ListaPublicidad> ObtenerPublicidadCantidad()
        {
            try
            {
                return Estudiante.GetPublicidadCantidad(Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerPublicidadCantidad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo datos de publicidad | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerPublicidadCantidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo datos de publicidad | Error: " + ex.Message);
            }
        }

        #endregion


        #region Funcionario

        public Funcionario CrearFuncionario(Funcionario funcionario)
        {
            try
            {
                if (Funcionario.ValidarFuncionarioInsert(funcionario, Fachada_001.Conexion))
                {
                    if (funcionario.Guardar(Fachada_001.Conexion))
                        return funcionario;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en CrearFuncionario | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando funcionario | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en CrearFuncionario | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando funcionario | Error: " + ex.Message);
            }
        }

        public bool ModificarFuncionario(Funcionario funcionario)
        {
            try
            {
                if (Funcionario.ValidarFuncionarioModificar(funcionario, Fachada_001.Conexion))
                {
                    if (funcionario.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ModificarFuncionario | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando funcionario | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ModificarFuncionario | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando funcionario | Error: " + ex.Message);
            }
        }

        public bool EliminarFuncionario(Funcionario funcionario)
        {
            try
            {
                if (Funcionario.ExisteFuncionarioByID(funcionario.ID, Fachada_001.Conexion))
                {
                    if (funcionario.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en EliminarFuncionario | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar al funcionario");
                }
                else
                {
                    throw new Exception("Error eliminando funcionario | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en EliminarFuncionario | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando funcionario | Error: " + ex.Message);
            }
        }

        public List<Funcionario> ObtenerFuncionarios()
        {
            try
            {
                Funcionario funcionario = new Funcionario();
                List<Funcionario> lstFuncionarios = funcionario.GetAll(Fachada_001.Conexion);
                return lstFuncionarios;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ObtenerFuncionarios | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo funcionarios | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ObtenerFuncionarios | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo funcionarios | Error: " + ex.Message);
            }
        }

        public List<Funcionario> ObtenerFuncionariosPorSucursal(Sucursal sucursal)
        {
            try
            {
                Funcionario funcionario = new Funcionario();
                List<Funcionario> lstFuncionarios = funcionario.GetBySucursal(sucursal, Fachada_001.Conexion);
                return lstFuncionarios;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ObtenerFuncionariosPorSucursal | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo funcionarios por sucursal | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ObtenerFuncionariosPorSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo funcionarios por sucursal | Error: " + ex.Message);
            }
        }

        public Funcionario GetFuncionario(Funcionario funcionario)
        {
            try
            {
                if (funcionario.Leer(Fachada_001.Conexion))
                    return funcionario;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en GetFuncionario | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo funcionario | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en GetFuncionario | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo funcionario | Error: " + ex.Message);
            }
        }

        public Funcionario Login(Funcionario funcionario)
        {
            try
            {
                //if (!Herramientas.ValidarCedula(funcionario.CI))
                //{
                //    throw new ValidacionException("Cedula invalida");
                //}
                return funcionario.Login(Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en Login | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error accediendo a la base de datos durante el Login. Inténtelo nuevamente.");
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en Login | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error accediendo a la base de datos durante el Login. Inténtelo nuevamente | Error: " + ex.Message);
            }
        }

        public async Task<string> OlvideMiPassword(Funcionario funcionario)
        {
            try
            {
                if (funcionario.Leer(Fachada_001.Conexion))
                {
                    await funcionario.OlvideMiPassword(Fachada_001.Conexion);
                    return funcionario.Email;
                }
                throw new ValidacionException("No existe el funcionario");
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en OlvideMiPassword | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error reestableciendo contraseña de funcionario | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en OlvideMiPassword | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error reestableciendo contraseña de funcionario | Error: " + ex.Message);
            }
        }



        #endregion


        #region Matricula

        public Matricula CrearMatricula(Matricula matricula)
        {
            try
            {
                if (Matricula.ValidarMatriculaInsert(matricula, Fachada_001.Conexion))
                {
                    if (matricula.Guardar(Fachada_001.Conexion))
                        return matricula;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en CrearMatricula | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando matricula | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en CrearMatricula | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando matricula | Error: " + ex.Message);
            }
        }

        public bool ModificarMatricula(Matricula matricula)
        {
            try
            {
                if (Matricula.ValidarMatriculaModificar(matricula, Fachada_001.Conexion))
                {
                    if (matricula.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ModificarMatricula | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando matricula | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ModificarMatricula | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando matricula | Error: " + ex.Message);
            }
        }

        public bool EliminarMatricula(Matricula matricula)
        {
            try
            {
                if (Matricula.ExisteByID(matricula.ID, Fachada_001.Conexion))
                {
                    if (matricula.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en EliminarMatricula | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar la matricula");
                }
                else
                {
                    throw new Exception("Error eliminando matricula | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en EliminarMatricula | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando matricula | Error: " + ex.Message);
            }
        }

        public List<Matricula> ObtenerMatriculas()
        {
            try
            {
                Matricula matricula = new Matricula();
                List<Matricula> lstMatriculas = matricula.GetAll(Fachada_001.Conexion);
                return lstMatriculas;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ObtenerMatriculas | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo matriculas | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ObtenerMatriculas | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo matriculas | Error: " + ex.Message);
            }
        }

        public List<Matricula> ObtenerMatriculasByAnio(int anio)
        {
            try
            {
                List<Matricula> lstMatriculas = Matricula.GetAllByAnio(anio, Fachada_001.Conexion);
                return lstMatriculas;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ObtenerMatriculasByAnio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo matriculas por año | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ObtenerMatriculasByAnio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo matriculas por año | Error: " + ex.Message);
            }
        }


        public Matricula GetMatricula(Matricula matricula)
        {
            try
            {
                if (matricula.Leer(Fachada_001.Conexion))
                    return matricula;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en GetMatricula | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo matricula | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en GetMatricula | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo matricula | Error: " + ex.Message);
            }
        }

        #endregion


        #region VentaLibro

        public VentaLibro CrearVentaLibro(VentaLibro venta)
        {
            try
            {
                if (VentaLibro.ValidarVentaLibroInsert(venta))
                {
                    if (venta.Guardar(Fachada_001.Conexion))
                        return venta;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en CrearVentaLibro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando la venta del libro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en CrearVentaLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando la venta del libro | Error: " + ex.Message);
            }
        }

        public bool ModificarVentaLibro(VentaLibro venta)
        {
            try
            {
                if (VentaLibro.ValidarVentaLibroModificar(venta, Fachada_001.Conexion))
                {
                    if (venta.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ModificarVentaLibro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando la venta del libro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ModificarVentaLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando la venta del libro | Error: " + ex.Message);
            }
        }

        public bool EliminarVentaLibro(VentaLibro venta)
        {
            try
            {
                if (VentaLibro.ExisteVentaLibro(venta, Fachada_001.Conexion))
                    if (venta.Eliminar(Fachada_001.Conexion))
                        return true;
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en EliminarVentaLibro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar la venta del libro");
                }
                else
                {
                    throw new Exception("Error eliminando la venta del libro | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en EliminarVentaLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando la venta del libro | Error: " + ex.Message);
            }
        }

        public List<VentaLibro> ObtenerVentaLibros()
        {
            try
            {
                VentaLibro venta = new VentaLibro();
                List<VentaLibro> lstVentas = venta.GetAll(Fachada_001.Conexion);
                return lstVentas;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ObtenerVentaLibros | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo ventas de libros | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ObtenerVentaLibros | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo ventas de libros | Error: " + ex.Message);
            }
        }

        public List<VentaLibro> ObtenerVentaLibrosByEstado(VentaLibro venta)
        {
            try
            {
                List<VentaLibro> lstVentas = venta.GetByEstado(Fachada_001.Conexion);
                return lstVentas;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ObtenerVentaLibrosByEstado | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo ventas de libros por estado | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ObtenerVentaLibrosByEstado | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo ventas de libros por estado | Error: " + ex.Message);
            }
        }

        public VentaLibro GetVentaLibro(VentaLibro venta)
        {
            try
            {
                if (venta.Leer(Fachada_001.Conexion))
                    return venta;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en GetVentaLibro | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo la venta del libro | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en GetVentaLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo la venta del libro | Error: " + ex.Message);
            }
        }

        #endregion


        #region Examen

        public Examen CrearExamen(Examen examen)
        {
            try
            {
                if (Examen.ValidarExamenInsert(examen, Fachada_001.Conexion))
                {
                    if (examen.Guardar(Fachada_001.Conexion))
                        return examen;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en CrearExamen | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en CrearExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando examen | Error: " + ex.Message);
            }
        }

        public bool ModificarExamen(Examen examen)
        {
            try
            {
                if (Examen.ValidarExamenModificar(examen, Fachada_001.Conexion))
                {
                    if (examen.Modificar(Fachada_001.Conexion))
                        return true; ;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ModificarExamen | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ModificarExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando examen | Error: " + ex.Message);
            }
        }

        public bool EliminarExamen(Examen examen)
        {
            try
            {
                if (examen.ExisteExamen(Fachada_001.Conexion))
                {
                    if (examen.Eliminar(Fachada_001.Conexion))
                        return true; ;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en EliminarExamen | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar el examen");
                }
                else
                {
                    throw new Exception("Error eliminando examen | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en EliminarExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando examen | Error: " + ex.Message);
            }
        }

        public List<Examen> ObtenerExamenes()
        {
            try
            {
                Examen examen = new Examen();
                List<Examen> lstExamenes = examen.GetAll(Fachada_001.Conexion);
                return lstExamenes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenes | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenes | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes | Error: " + ex.Message);
            }
        }

        public List<Examen> ObtenerExamenesByGrupo(Examen examen)
        {
            try
            {
                List<Examen> lstExamenes = examen.GetByGrupo(Fachada_001.Conexion);
                return lstExamenes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenesByGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes por grupo | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenesByGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes por grupo | Error: " + ex.Message);
            }
        }

        public List<Examen> ObtenerExamenesByAnioAsociado(Examen examen)
        {
            try
            {
                List<Examen> lstExamenes = examen.GetByAnio(Fachada_001.Conexion);
                return lstExamenes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenesByAnioAsociado | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes por año | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenesByAnioAsociado | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes por año | Error: " + ex.Message);
            }
        }

        public Examen GetExamen(Examen examen)
        {
            try
            {
                if (examen.Leer(Fachada_001.Conexion))
                    return examen;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en GetExamen | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en GetExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examen | Error: " + ex.Message);
            }
        }

        public Examen GetExamenByMateriaAnio(Examen examen)
        {
            try
            {
                if (examen.LeerByMateriaAnio(Fachada_001.Conexion))
                {
                    return examen;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en GetExamenByMateriaAnio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examen por materia | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en GetExamenByMateriaAnio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examen por materia | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudiantesByExamen(Examen examen)
        {
            try
            {
                List<Estudiante> lstEstudiantes = examen.GetListaEstudiantes(Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerEstudiantesByExamen | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes por examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerEstudiantesByExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes por examen | Error: " + ex.Message);
            }
        }

        public bool InscribirEstudianteConvenioAExamen(Examen examen)
        {
            try
            {
                return examen.InscribirEstudianteConConvenio(Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Examen", "Error en InscribirEstudianteConvenioAExamen | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error inscribiendo estudiantes con convenio a examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en InscribirEstudianteConvenioAExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error inscribiendo estudiantes con convenio a examen | Error: " + ex.Message);
            }
        }


        #endregion


        #region Grupo

        public Grupo CrearGrupo(Grupo grupo)
        {
            try
            {
                if (Grupo.ValidarGrupoInsert(grupo, Fachada_001.Conexion))
                {
                    if (grupo.Guardar(Fachada_001.Conexion))
                        return grupo;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en CrearGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando grupo | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en CrearGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando grupo | Error: " + ex.Message);
            }
        }

        public bool ModificarGrupo(Grupo grupo)
        {
            try
            {
                if (Grupo.ValidarGrupoModificar(grupo, Fachada_001.Conexion))
                {
                    if (grupo.Modificar(Fachada_001.Conexion))
                        return true; ;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ModificarGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando grupo | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ModificarGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando grupo | Error: " + ex.Message);
            }
        }

        public bool EliminarGrupo(Grupo grupo)
        {
            try
            {
                if (Grupo.ExisteGrupo(grupo, Fachada_001.Conexion))
                {
                    if (grupo.Eliminar(Fachada_001.Conexion))
                        return true; ;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en EliminarGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar el grupo");
                }
                else
                {
                    throw new Exception("Error eliminando grupo | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en EliminarGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando grupo | Error: " + ex.Message);
            }
        }

        public List<Grupo> ObtenerGrupos()
        {
            try
            {
                Grupo grupo = new Grupo();
                List<Grupo> lstGrupos = grupo.GetAll(Fachada_001.Conexion);
                return lstGrupos;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerGrupos | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo grupos | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerGrupos | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo grupos | Error: " + ex.Message);
            }
        }

        public List<Grupo> ObtenerGruposByAnio(int anio)
        {
            try
            {
                Grupo grupo = new Grupo();
                List<Grupo> lstGrupos = grupo.GetAllByAnio(anio, Fachada_001.Conexion);
                return lstGrupos;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerGruposByAnio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo grupos por año | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerGruposByAnio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo grupos por año | Error: " + ex.Message);
            }
        }

        public Grupo GetGrupo(Grupo grupo)
        {
            try
            {
                if (grupo.Leer(Fachada_001.Conexion))
                    return grupo;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en GetGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo grupo | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en GetGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo grupo | Error: " + ex.Message);
            }
        }

        public List<Estudiante> ObtenerEstudiantesByGrupo(Grupo grupo)
        {
            try
            {
                List<Estudiante> lstEstudiantes = grupo.GetEstudiantes(Fachada_001.Conexion);
                return lstEstudiantes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerEstudiantesByGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes por grupo | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerEstudiantesByGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo estudiantes por grupo | Error: " + ex.Message);
            }
        }


        #endregion


        #region MatriculaEstudiante

        public MatriculaEstudiante CrearMatriculaEstudiante(MatriculaEstudiante matricula)
        {
            try
            {
                if (MatriculaEstudiante.ValidarMatriculaEstudianteInsert(matricula, Fachada_001.Conexion))
                {
                    if (matricula.Guardar(Fachada_001.Conexion))
                        return matricula;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en CrearMatriculaEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando la matricula del estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en CrearMatriculaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando la matricula del estudiante | Error: " + ex.Message);
            }
        }

        public bool ModificarMatriculaEstudiante(MatriculaEstudiante matricula)
        {
            try
            {
                if (MatriculaEstudiante.ValidarMatriculaEstudianteModificar(matricula, Fachada_001.Conexion))
                {
                    if (matricula.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en ModificarMatriculaEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando la matricula del estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en ModificarMatriculaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando la matricula del estudiante | Error: " + ex.Message);
            }
        }

        public bool EliminarMatriculaEstudiante(MatriculaEstudiante matricula)
        {
            try
            {
                if (MatriculaEstudiante.ExisteMatriculaEstudiante(matricula, Fachada_001.Conexion))
                {
                    if (matricula.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en EliminarMatriculaEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar la matricula del estudiante");
                }
                else
                {
                    throw new Exception("Error eliminando la matricula del estudiante | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en EliminarMatriculaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando la matricula del estudiante | Error: " + ex.Message);
            }
        }

        public List<MatriculaEstudiante> ObtenerMatriculaEstudiantes()
        {
            try
            {
                MatriculaEstudiante matriculaEstudiante = new MatriculaEstudiante();
                List<MatriculaEstudiante> lstMatriculas = matriculaEstudiante.GetAll(Fachada_001.Conexion);
                return lstMatriculas;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en ObtenerMatriculaEstudiantes | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo matriculas de estudiantes | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en ObtenerMatriculaEstudiantes | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo matriculas de estudiantes | Error: " + ex.Message);
            }
        }

        public MatriculaEstudiante GetMatriculaEstudiante(MatriculaEstudiante matriculaEstudiante)
        {
            try
            {
                if (matriculaEstudiante.Leer(Fachada_001.Conexion))
                    return matriculaEstudiante;
                else
                    return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en GetMatriculaEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo la matricula del estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en GetMatriculaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo la matricula del estudiante | Error: " + ex.Message);
            }
        }

        #endregion


        #region Mensualidad

        public Mensualidad CrearMensualidad(Mensualidad mensualidad)
        {
            try
            {
                if (Mensualidad.ValidarMensualidadInsert(mensualidad, Fachada_001.Conexion))
                {
                    if (mensualidad.Guardar(Fachada_001.Conexion))
                        return mensualidad;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en CrearMensualidad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error creando mensualidad | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en CrearMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error creando mensualidad | Error: " + ex.Message);
            }
        }

        public bool ModificarMensualidad(Mensualidad mensualidad)
        {
            try
            {
                if (Mensualidad.ValidarMensualidadModificar(mensualidad, Fachada_001.Conexion))
                {
                    if (mensualidad.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ModificarMensualidad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando mensualidad | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ModificarMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando mensualidad | Error: " + ex.Message);
            }
        }

        public bool EliminarMensualidad(Mensualidad mensualidad)
        {
            try
            {
                if (Mensualidad.ExisteMensualidadByID(mensualidad, Fachada_001.Conexion))
                {
                    if (mensualidad.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en EliminarMensualidad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar la mensualidad");
                }
                else
                {
                    throw new Exception("Error eliminando mensualidad | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en EliminarMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando mensualidad | Error: " + ex.Message);
            }
        }

        public List<Mensualidad> ObtenerMensualidades()
        {
            {
                try
                {
                    Mensualidad mensualidad = new Mensualidad();
                    List<Mensualidad> lstMensualidades = mensualidad.GetAllLazy(Fachada_001.Conexion);
                    Mensualidad.CalcularRecargo(lstMensualidades, Fachada_001.Conexion);
                    return lstMensualidades;
                }
                catch (ValidacionException ex)
                {
                    throw ex;
                }
                catch (SqlException ex)
                {
                    Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidades | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                    throw new Exception("Error obteniendo mensualidades | Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidades | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                    throw new Exception("Error obteniendo mensualidades | Error: " + ex.Message);
                }
            }
        }

        public List<Mensualidad> ObtenerMensualidadesByEstudiante(Mensualidad mensualidad)
        {
            try
            {
                List<Mensualidad> lstMensualidades = mensualidad.GetAllByEstudiante(Fachada_001.Conexion);
                Mensualidad.CalcularRecargo(lstMensualidades, Fachada_001.Conexion);
                return lstMensualidades;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidadesByEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo mensualidades por estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidadesByEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo mensualidades por estudiante | Error: " + ex.Message);
            }
        }

        public List<Mensualidad> ObtenerMensualidadesImpagasByEstudiante(Mensualidad mensualidad)
        {
            try
            {
                List<Mensualidad> lstMensualidades = mensualidad.GetAllImpagasByEstudiante(Fachada_001.Conexion);
                Mensualidad.CalcularRecargo(lstMensualidades, Fachada_001.Conexion);
                return lstMensualidades;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidadesImpagasByEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo mensualidades impagas por estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidadesImpagasByEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo mensualidades impagas por estudiante | Error: " + ex.Message);
            }
        }

        public Mensualidad GetMensualidad(Mensualidad mensualidad)
        {
            try
            {
                if (mensualidad.Leer(Fachada_001.Conexion))
                    return mensualidad;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en GetMensualidad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo mensualidad | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en GetMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo mensualidad | Error: " + ex.Message);
            }
        }

        public bool PagarMensualidad(List<Mensualidad> lstMensualidades)
        {
            try
            {
                if (Mensualidad.PagarMensualidad(lstMensualidades, Fachada_001.Conexion))
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = lstMensualidades[0].Estudiante.ID
                    };
                    if (!estudiante.DebeExamen(Fachada_001.Conexion) && !estudiante.DebeMensualidad(Fachada_001.Conexion))
                    {
                        estudiante.SetDeudor(Fachada_001.Conexion, false);
                    }
                    return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en PagarMensualidad | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error pagando mensualidad | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en PagarMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error pagando mensualidad | Error: " + ex.Message);
            }
        }

        public bool PagarMensualidadByConvenio(Convenio convenio, int funcionarioID)
        {
            try
            {
                if (Mensualidad.PagarMensualidadByConvenio(convenio, funcionarioID, Fachada_001.Conexion))
                {
                    List<Estudiante> lstEstudiantes = Estudiante.LeerByConvenio(convenio, Fachada_001.Conexion);
                    foreach (Estudiante estudiante in lstEstudiantes)
                    {
                        if (!estudiante.DebeExamen(Fachada_001.Conexion) && !estudiante.DebeMensualidad(Fachada_001.Conexion))
                        {
                            estudiante.SetDeudor(Fachada_001.Conexion, false);
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en PagarMensualidadByConvenio | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error pagando mensualidades del convenio | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en PagarMensualidadByConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error pagando mensualidades del convenio | Error: " + ex.Message);
            }
        }

        #endregion


        #region ExamenEstudiante

        public ExamenEstudiante CrearExamenEstudiante(ExamenEstudiante examenEstudiante)
        {
            try
            {
                if (ExamenEstudiante.ValidarExamenEstudianteInsert(examenEstudiante, Fachada_001.Conexion))
                {
                    if (examenEstudiante.Guardar(Fachada_001.Conexion))
                        return examenEstudiante;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en CrearExamenEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error inscribiendo el estudiante al examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en CrearExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error inscribiendo el estudiante al examen | Error: " + ex.Message);
            }
        }

        public bool ModificarExamenEstudiante(ExamenEstudiante examenEstudiante)
        {
            try
            {
                if (ExamenEstudiante.ValidarExamenEstudianteModificar(examenEstudiante, Fachada_001.Conexion))
                {
                    if (examenEstudiante.Modificar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en ModificarExamenEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error modificando la inscripción del estudiante a un examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en ModificarExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error modificando la inscripción del estudiante a un examen | Error: " + ex.Message);
            }
        }

        public bool PagarCuotaExamenEstudiante(ExamenEstudiante examenEstudiante)
        {
            try
            {
                ExamenEstudiante examenEstudianteAux = new ExamenEstudiante
                {
                    ID = examenEstudiante.ID,
                    Examen = examenEstudiante.Examen,
                    Estudiante = examenEstudiante.Estudiante
                };
                if (examenEstudianteAux.Leer(Fachada_001.Conexion))
                {
                    if (examenEstudiante.PagarCuota(Fachada_001.Conexion))
                    {
                        Estudiante estudiante = new Estudiante
                        {
                            ID = examenEstudiante.Estudiante.ID
                        };
                        if (!estudiante.DebeExamen(Fachada_001.Conexion) && !estudiante.DebeMensualidad(Fachada_001.Conexion))
                        {
                            estudiante.SetDeudor(Fachada_001.Conexion, false);
                        }
                        if (!examenEstudiante.Pago)
                        {
                            examenEstudiante.LstCuotas.Clear();
                            examenEstudiante.Leer(Fachada_001.Conexion);
                            decimal totalCuotas = 0;
                            foreach (ExamenEstudianteCuota cuota in examenEstudiante.LstCuotas)
                            {
                                if (cuota.CuotaPaga)
                                {
                                    totalCuotas += cuota.Precio;
                                }
                            }
                            if (totalCuotas >= examenEstudiante.Precio)
                            {
                                examenEstudiante.MarcarComoPago(Fachada_001.Conexion);
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en PagarCuotaExamenEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error pagando la cuota de un examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en PagarCuotaExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error pagando la cuota de un examen | Error: " + ex.Message);
            }
        }

        public bool AsignarResultadoListaExamenEstudiante(List<ExamenEstudiante> lstExamenEstudiante)
        {
            try
            {
                if (ExamenEstudiante.AsignarResultadoByLista(lstExamenEstudiante, Fachada_001.Conexion))
                {
                    return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en AsignarResultadoListaExamenEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error asignando resultados a los examenes | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en AsignarResultadoListaExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error asignando resultados a los examenes | Error: " + ex.Message);
            }
        }

        public bool EliminarExamenEstudiante(ExamenEstudiante examenEstudiante)
        {
            try
            {
                if (ExamenEstudiante.ExisteExamenEstudiante(examenEstudiante, Fachada_001.Conexion))
                {
                    if (examenEstudiante.Eliminar(Fachada_001.Conexion))
                        return true;
                }
                return false;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en EliminarExamenEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                if (ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    throw new Exception("No se puede eliminar el examen del estudiante");
                }
                else
                {
                    throw new Exception("Error eliminando el examen del estudiante | Error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en EliminarExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error eliminando el examen del estudiante | Error: " + ex.Message);
            }
        }

        public List<ExamenEstudiante> ObtenerExamenEstudiantes()
        {
            try
            {
                ExamenEstudiante examenEstudiante = new ExamenEstudiante();
                List<ExamenEstudiante> lstExamenes = examenEstudiante.GetAll(Fachada_001.Conexion);
                return lstExamenes;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en ObtenerExamenesEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes de los estudiantes | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en ObtenerExamenesEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes de los estudiantes | Error: " + ex.Message);
            }
        }

        public ExamenEstudiante GetExamenEstudiante(ExamenEstudiante examenEstudiante)
        {
            try
            {
                if (examenEstudiante.Leer(Fachada_001.Conexion))
                    return examenEstudiante;
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo el examen del estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo el examen del estudiante | Error: " + ex.Message);
            }
        }

        public List<ExamenEstudiante> GetExamenesNoPagosByEstudiante(Estudiante estudiante)
        {
            try
            {
                return ExamenEstudiante.GetNoPagosByEstudiante(estudiante, Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenesNoPagosByEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes sin pagar por estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenesNoPagosByEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes sin pagar por estudiante | Error: " + ex.Message);
            }
        }

        public List<ExamenEstudiante> GetExamenesActualesByEstudiante(Estudiante estudiante)
        {
            try
            {
                return ExamenEstudiante.GetActualesByEstudiante(estudiante, Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenesActualesByEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes por estudiante | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenesActualesByEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo examenes por estudiante | Error: " + ex.Message);
            }
        }

        public List<ExamenEstudiante> GetExamenEstudianteByExamen(Examen examen)
        {
            try
            {
                if (examen.Leer(Fachada_001.Conexion))
                {
                    List<ExamenEstudiante> lstExamenes = ExamenEstudiante.GetByExamen(examen, Fachada_001.Conexion);
                    return lstExamenes;
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenEstudianteByExamen | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw new Exception("Error obteniendo inscripciones a un examen | Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenEstudianteByExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw new Exception("Error obteniendo inscripciones a un examen | Error: " + ex.Message);
            }
        }

        #endregion



    }
}
