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
                        await email.Enviar(Fachada_001.Conexion, paramEmail, paramClave);
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en CrearEmail | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en ModificarEmail | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en EliminarEmail | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en ObtenerEmails | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en ObtenerEmailsEntreFechas | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en GetEmail | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                    await email.Enviar(Fachada_001.Conexion, paramEmail, paramClave);
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Email", "Error en EnviarEmailsPendientes | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en CrearConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ModificarConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en EliminarConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ObtenerConvenios | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en ObtenerConveniosByAnio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Convenio", "Error en GetConvenio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en CrearParametro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en ModificarParametro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en EliminarParametro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en ObtenerParametros | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Parametro", "Error en GetParametro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en CrearPago | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en ModificarPago | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en EliminarPago | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en ObtenerPagos | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Pago", "Error en GetPago | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en CrearMateria | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ModificarMateria | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en ObtenerMaterias | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Materia", "Error en GetMateria | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en CrearLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en ModificarLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en EliminarLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en ObtenerLibros | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Libro", "Error en GetLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en CrearEmpresa | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en ModificarEmpresa | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en EliminarEmpresa | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en ObtenerEmpresas | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Empresa", "Error en GetEmpresa | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en CrearSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en ModificarSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en EliminarSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en ObtenerSucursales | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Sucursal", "Error en GetSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en CrearEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ModificarEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en EliminarEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantes | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudiantesActivos | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en ObtenerEstudianteByNombre | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetEstudianteConMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
            }
        }

        public ExamenEstudiante GetExamenPendienteByEstudiante(Estudiante estudiante)
        {
            try
            {
                List<ExamenEstudiante> lstExamenEstudiante = ExamenEstudiante.GetByEstudiante(estudiante, Fachada_001.Conexion);
                if (lstExamenEstudiante.Count > 0)
                {
                    return ExamenEstudiante.GetExamenPendientePorEstudiante(lstExamenEstudiante);
                }
                return null;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetExamenPendienteByEstudiante | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Estudiante", "Error en GetExamenPendienteByEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en CrearFuncionario | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ModificarFuncionario | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en EliminarFuncionario | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ObtenerFuncionarios | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en ObtenerFuncionariosPorSucursal | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en GetFuncionario | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
            }
        }

        public Funcionario Login(Funcionario funcionario)
        {
            try
            {
                if (!Herramientas.ValidarCedula(funcionario.CI))
                {
                    throw new ValidacionException("Cedula invalida");
                }
                return Funcionario.Login(funcionario, Fachada_001.Conexion);
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en Login | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Funcionario", "Error en Login | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en CrearMatricula | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ModificarMatricula | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en EliminarMatricula | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ObtenerMatriculas | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en ObtenerMatriculasByAnio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Matricula", "Error en GetMatricula | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en CrearVentaLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ModificarVentaLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en EliminarVentaLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ObtenerVentaLibros | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en ObtenerVentaLibrosByEstado | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("VentaLibro", "Error en GetVentaLibro | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en CrearExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ModificarExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en EliminarExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenes | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenesByGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en ObtenerExamenesByAnioAsociado | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Examen", "Error en GetExamen | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en CrearGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ModificarGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en EliminarGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerGrupos | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerGruposByAnio | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en GetGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Grupo", "Error en ObtenerEstudiantesByGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en CrearMatriculaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en ModificarMatriculaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en EliminarMatriculaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en ObtenerMatriculaEstudiantes | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("MatriculaEstudiante", "Error en GetMatriculaEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en CrearMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ModificarMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en EliminarMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
            }
        }

        public List<Mensualidad> ObtenerMensualidades()
        {
            {
                try
                {
                    Mensualidad mensualidad = new Mensualidad();
                    List<Mensualidad> lstMensualidades = mensualidad.GetAllLazy(Fachada_001.Conexion);
                    return lstMensualidades;
                }
                catch (ValidacionException ex)
                {
                    throw ex;
                }
                catch (SqlException ex)
                {
                    Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidades | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidades | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                    throw ex;
                }
            }
        }

        public List<Mensualidad> ObtenerMensualidadesByEstudianteGrupo(Mensualidad mensualidad)
        {
            try
            {
                List<Mensualidad> lstMensualidades = mensualidad.GetAllByEstudianteGrupo(Fachada_001.Conexion);
                return lstMensualidades;
            }
            catch (ValidacionException ex)
            {
                throw ex;
            }
            catch (SqlException ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidadesByEstudianteGrupo | " + ex.Message, LogErrorTipo.Sql, Fachada_001.Conexion);
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en ObtenerMensualidadesByEstudianteGrupo | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("Mensualidad", "Error en GetMensualidad | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en CrearExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en ModificarExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en EliminarExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en ObtenerExamenesEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
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
                throw ex;
            }
            catch (Exception ex)
            {
                Herramientas.CrearLogError("ExamenEstudiante", "Error en GetExamenEstudiante | " + ex.Message, LogErrorTipo.Interno, Fachada_001.Conexion);
                throw ex;
            }
        }

        #endregion



    }
}
