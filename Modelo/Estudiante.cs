using BibliotecaBritanico.Utilidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BibliotecaBritanico.Modelo
{
    public class Estudiante : Persistencia, IPersistencia<Estudiante>
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public string CI { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaNac { get; set; }
        public bool Alergico { get; set; }
        public string Alergias { get; set; }
        public string ContactoAlternativoUno { get; set; }
        public string ContactoAlternativoUnoTel { get; set; }
        public string ContactoAlternativoDos { get; set; }
        public string ContactoAlternativoDosTel { get; set; }
        public Convenio Convenio { get; set; } //el convenio tiene la lista de estudiantes tambien      
        [JsonIgnore]
        public Grupo Grupo { get; set; } //el grupo tiene la lista de estudiantes
        public int GrupoID { get; set; }
        public int MateriaID { get; set; }
        public bool Activo { get; set; }
        public List<Mensualidad> LstMensualidades { get; set; } = new List<Mensualidad>();
        public bool Validado { get; set; }
        public bool Deudor { get; set; }
        public TipoPublicidad TipoPublicidad { get; set; }
        public DateTime FechaIngreso { get; set; }


        public Estudiante()
        {
            this.Convenio = new Convenio();
            this.Grupo = new Grupo();
        }

        public static bool ValidarEstudianteInsert(Estudiante estudiante, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (estudiante.Nombre.Equals(String.Empty) || estudiante.CI.Equals(String.Empty) || estudiante.Tel.Equals(String.Empty))
                {
                    errorMsg = "Debe ingresar Nombre, Cedula y Telefono del estudiante \n";
                }
                if (estudiante.TipoDocumento.Equals(TipoDocumento.CI) && !estudiante.CI.Equals(String.Empty) && !Herramientas.ValidarCedula(estudiante.CI))
                {
                    errorMsg += "Cedula invalida \n";
                }
                if (estudiante.FechaNac >= DateTime.Today || estudiante.FechaNac <= DateTime.MinValue)
                {
                    errorMsg += "Fecha de nacimiento invalida \n";
                }
                int edad = estudiante.CalcularEdad();
                if ((edad < 18) && (estudiante.ContactoAlternativoUno.Equals(String.Empty) || estudiante.ContactoAlternativoUnoTel.Equals(String.Empty)))
                {
                    errorMsg = "Debe ingresar datos del responsable del estudiante \n";
                }
                if (!estudiante.Email.Equals(String.Empty) && !Herramientas.ValidarMail(estudiante.Email))
                {
                    errorMsg += "Email invalido \n";
                }
                Estudiante estudianteAux = new Estudiante
                {
                    ID = 0,
                    CI = estudiante.CI
                };
                if (errorMsg.Equals(String.Empty) && Estudiante.ExisteEstudiante(estudianteAux, strCon))
                {
                    errorMsg = "Ya existe un estudiante con el Documento: " + estudiante.CI.ToString().Trim();
                }
                if (errorMsg.Equals(String.Empty))
                {
                    if (estudiante.GrupoID > 0 && estudiante.MateriaID > 0)
                    {
                        estudiante.Grupo = new Grupo
                        {
                            ID = estudiante.GrupoID
                        };
                        estudiante.Grupo.Materia.ID = estudiante.MateriaID;
                        if (!Grupo.ExisteGrupo(estudiante.Grupo, strCon))
                        {
                            errorMsg = "No existe el grupo que desea asociar al estudiante \n";
                        }
                    }
                }
                if (!errorMsg.Equals(String.Empty))
                {
                    throw new ValidacionException(errorMsg);
                }
                return true;
            }
            catch (ValidacionException ex)
            {
                throw ex;
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

        public static bool ValidarEstudianteModificar(Estudiante estudiante, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (estudiante.Nombre.Equals(String.Empty) || estudiante.CI.Equals(String.Empty) || estudiante.Tel.Equals(String.Empty))
                {
                    errorMsg = "Debe ingresar Nombre, Cedula y Telefono del estudiante \n";
                }
                if (estudiante.FechaNac >= DateTime.Today || estudiante.FechaNac <= DateTime.MinValue)
                {
                    errorMsg += "Fecha de nacimiento invalida \n";
                }
                int edad = estudiante.CalcularEdad();
                if ((edad < 18) && (estudiante.ContactoAlternativoUno.Equals(String.Empty) || estudiante.ContactoAlternativoUnoTel.Equals(String.Empty)))
                {
                    errorMsg = "Debe ingresar datos del responsable del estudiante \n";
                }
                if (!estudiante.Email.Equals(String.Empty) && !Herramientas.ValidarMail(estudiante.Email))
                {
                    errorMsg += "Email invalido \n";
                }
                if (errorMsg.Equals(String.Empty) && !Estudiante.ExisteEstudiante(estudiante, strCon))
                {
                    errorMsg += "No existe el estudiante que desea modificar";
                }
                if (errorMsg.Equals(String.Empty))
                {
                    if (estudiante.GrupoID > 0 && estudiante.MateriaID > 0)
                    {
                        estudiante.Grupo = new Grupo
                        {
                            ID = estudiante.GrupoID
                        };
                        estudiante.Grupo.Materia.ID = estudiante.MateriaID;
                        if (!Grupo.ExisteGrupo(estudiante.Grupo, strCon))
                        {
                            errorMsg = "No existe el grupo que desea asociar al estudiante \n";
                        }
                    }
                }
                if (!errorMsg.Equals(String.Empty))
                {
                    throw new ValidacionException(errorMsg);
                }
                return true;
            }
            catch (ValidacionException ex)
            {
                throw ex;
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

        public static bool ExisteEstudiante(Estudiante estudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";

            if (estudiante.ID > 0)
            {
                sql = "SELECT * FROM Estudiante WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", estudiante.ID));
            }
            else if (!estudiante.CI.Equals(String.Empty))
            {
                sql = "SELECT * FROM Estudiante WHERE CI = @CI";
                lstParametros.Add(new SqlParameter("@CI", estudiante.CI));
            }
            else
            {
                return false;
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    ok = true;
                    break;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return ok;
        }

        private int CalcularEdad()
        {
            int edad = DateTime.Today.AddTicks(-FechaNac.Ticks).Year - 1;
            return edad;
        }


        #region Persistencia

        public bool Leer(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";

            if (this.ID > 0)
            {
                sql = "SELECT * FROM Estudiante WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else if (!this.CI.Equals(String.Empty))
            {
                sql = "SELECT * FROM Estudiante WHERE CI = @CI";
                lstParametros.Add(new SqlParameter("@CI", this.CI));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Estudiante");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.Nombre = reader["Nombre"].ToString().Trim();
                    this.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    this.CI = reader["CI"].ToString().Trim();
                    this.Tel = reader["Tel"].ToString().Trim();
                    this.Email = reader["Email"].ToString().Trim();
                    this.Direccion = reader["Direccion"].ToString().Trim();
                    this.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    this.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    this.Alergias = reader["Alergias"].ToString().Trim();
                    this.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    this.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    this.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    this.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    this.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    if (this.Convenio.ID > 0)
                        this.Convenio.Leer(strCon);
                    this.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    this.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    this.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    this.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    this.Activo = Convert.ToBoolean(reader["Activo"]);
                    this.Validado = Convert.ToBoolean(reader["Validado"]);
                    this.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    this.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    ok = true;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return ok;
        }

        public bool Guardar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seGuardo = false;
            try
            {
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                DateTime fechaIngreso = DateTime.Now;
                lstParametros.Add(new SqlParameter("@FechaIngreso", fechaIngreso));
                string sql = "INSERT INTO Estudiante VALUES (@Nombre, @TipoDocumento, @CI, @Tel, @Email, @Direccion, @FechaNac, @Alergico, @Alergias, @ContactoAlternativoUno, @ContactoAlternativoUnoTel, @ContactoAlternativoDos, @ContactoAlternativoDosTel, @ConvenioID, @GrupoID, @MateriaID, @Activo, @Validado, 0, @TipoPublicidad, @FechaIngreso); SELECT CAST (SCOPE_IDENTITY() AS INT);";
                this.ID = 0;
                this.ID = Convert.ToInt32(Persistencia.EjecutarScalar(con, sql, CommandType.Text, lstParametros, null));
                if (this.ID > 0) seGuardo = true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return seGuardo;
        }

        public bool Modificar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool SeModifico = false;
            List<SqlParameter> lstParametros = this.ObtenerParametros();
            string sql = "UPDATE Estudiante SET Nombre = @Nombre, Tel = @Tel, Email = @Email, Direccion = @Direccion, FechaNac = @FechaNac, Alergico = @Alergico, Alergias = @Alergias, ContactoAlternativoUno = @ContactoAlternativoUno, ContactoAlternativoUnoTel = @ContactoAlternativoUnoTel, ContactoAlternativoDos = @ContactoAlternativoDos, ContactoAlternativoDosTel = @ContactoAlternativoDosTel, ConvenioID = @ConvenioID, GrupoID = @GrupoID, MateriaID = @MateriaID, Validado = @Validado, TipoPublicidad = @TipoPublicidad WHERE ID = @ID;";
            try
            {
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, null);
                if (res > 0) SeModifico = true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SeModifico;
        }

        //No se borran
        public bool Eliminar(string strCon)
        {
            {
                SqlConnection con = new SqlConnection(strCon);
                bool seBorro = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                string sql = "DELETE FROM Estudiante WHERE ID = @ID";
                try
                {
                    int resultado = 0;
                    resultado = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, null);
                    if (resultado > 0) seBorro = true;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return seBorro;
            }
        }

        public List<Estudiante> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            string sql = "SELECT * FROM Estudiante WHERE Validado = 1;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    if (estudiante.Convenio.ID > 0)
                        estudiante.Convenio.Leer(strCon);
                    estudiante.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = Convert.ToBoolean(reader["Validado"]);
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public List<Estudiante> GetAllActivos(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            string sql = "SELECT * FROM Estudiante WHERE Activo = 1 AND Validado = 1;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    //estudiante.Convenio.Leer(strCon);
                    estudiante.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = Convert.ToBoolean(reader["Validado"]);
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public List<Estudiante> GetAllNoActivos(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            string sql = "SELECT * FROM Estudiante WHERE Activo = 0 AND Validado = 1;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    //estudiante.Convenio.Leer(strCon);
                    estudiante.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = Convert.ToBoolean(reader["Validado"]);
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }


        public List<Estudiante> GetAllNoValidados(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            string sql = "SELECT * FROM Estudiante WHERE Validado = 0;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    //estudiante.Convenio.Leer(strCon);
                    estudiante.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = false;
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            lstParametros.Add(new SqlParameter("@TipoDocumento", this.TipoDocumento));
            lstParametros.Add(new SqlParameter("@CI", this.CI));
            lstParametros.Add(new SqlParameter("@Tel", this.Tel));
            lstParametros.Add(new SqlParameter("@Email", this.Email));
            lstParametros.Add(new SqlParameter("@Direccion", this.Direccion));
            lstParametros.Add(new SqlParameter("@FechaNac", this.FechaNac));
            lstParametros.Add(new SqlParameter("@Alergico", this.Alergico));
            lstParametros.Add(new SqlParameter("@Alergias", this.Alergias));
            lstParametros.Add(new SqlParameter("@ContactoAlternativoUno", this.ContactoAlternativoUno));
            lstParametros.Add(new SqlParameter("@ContactoAlternativoUnoTel", this.ContactoAlternativoUnoTel));
            lstParametros.Add(new SqlParameter("@ContactoAlternativoDos", this.ContactoAlternativoDos));
            lstParametros.Add(new SqlParameter("@ContactoAlternativoDosTel", this.ContactoAlternativoDosTel));
            lstParametros.Add(new SqlParameter("@TipoPublicidad", this.TipoPublicidad));
            if (this.Convenio != null)
            {
                lstParametros.Add(new SqlParameter("@ConvenioID", this.Convenio.ID));
            }
            else
            {
                lstParametros.Add(new SqlParameter("@ConvenioID", 0));
            }
            lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@Activo", this.Activo));
            lstParametros.Add(new SqlParameter("@Validado", this.Validado));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public List<Estudiante> GetAllLazy(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            string sql = "SELECT * FROM Estudiante WHERE Validado = 1;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    estudiante.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = Convert.ToBoolean(reader["Validado"]);
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public static List<Estudiante> LeerByNombre(string nombre, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Nombre", "%" + nombre + "%"));
            string sql = "SELECT * FROM Estudiante WHERE Nombre like @Nombre;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    if (estudiante.Convenio.ID > 0)
                        estudiante.Convenio.Leer(strCon);
                    estudiante.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = Convert.ToBoolean(reader["Validado"]);
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public bool LeerConMensualidad(string strCon, int anioAsociado)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";

            if (this.ID > 0)
            {
                sql = "SELECT * FROM Estudiante WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else if (!this.CI.Equals(String.Empty))
            {
                sql = "SELECT * FROM Estudiante WHERE CI = @CI";
                lstParametros.Add(new SqlParameter("@CI", this.CI));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Estudiante");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.Nombre = reader["Nombre"].ToString().Trim();
                    this.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    this.CI = reader["CI"].ToString().Trim();
                    this.Tel = reader["Tel"].ToString().Trim();
                    this.Email = reader["Email"].ToString().Trim();
                    this.Direccion = reader["Direccion"].ToString().Trim();
                    this.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    this.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    this.Alergias = reader["Alergias"].ToString().Trim();
                    this.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    this.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    this.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    this.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    this.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    if (this.Convenio.ID > 0)
                        this.Convenio.Leer(strCon);
                    this.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    this.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    this.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    this.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    this.Activo = Convert.ToBoolean(reader["Activo"]);
                    this.Validado = Convert.ToBoolean(reader["Validado"]);
                    this.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    this.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    Mensualidad mensualidad = new Mensualidad
                    {
                        ID = 0,
                        Estudiante = this,
                        AnioAsociado = anioAsociado
                    };
                    this.LstMensualidades = mensualidad.LeerPorEstudiante(strCon);
                    ok = true;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return ok;
        }

        public bool ModificarGrupo(int grupoID, int materiaID, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool SeModifico = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@GrupoID", grupoID));
            lstParametros.Add(new SqlParameter("@MateriaID", materiaID));
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            string sql = "UPDATE Estudiante SET GrupoID = @GrupoID, MateriaID = @MateriaID WHERE ID = @ID;";
            try
            {
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, null);
                if (res > 0) SeModifico = true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SeModifico;
        }

        public static List<Estudiante> LeerByConvenio(Convenio convenio, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ConvenioID", convenio.ID));
            string sql = "SELECT * FROM Estudiante WHERE ConvenioID = @ConvenioID;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio = convenio;
                    estudiante.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = Convert.ToBoolean(reader["Validado"]);
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public static List<Estudiante> GetAllByGrupo(Grupo grupo, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@GrupoID", grupo.ID));
            string sql = "SELECT * FROM Estudiante WHERE GrupoID = @GrupoID;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    estudiante.Grupo = grupo;
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = Convert.ToBoolean(reader["Validado"]);
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public static List<Estudiante> LeerEstudiantesConConvenio(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            string sql = "SELECT * FROM Estudiante WHERE ConvenioID > 0;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["ID"]);
                    estudiante.Nombre = reader["Nombre"].ToString().Trim();
                    estudiante.TipoDocumento = (TipoDocumento)Convert.ToInt32(reader["TipoDocumento"]);
                    estudiante.CI = reader["CI"].ToString().Trim();
                    estudiante.Tel = reader["Tel"].ToString().Trim();
                    estudiante.Email = reader["Email"].ToString().Trim();
                    estudiante.Direccion = reader["Direccion"].ToString().Trim();
                    estudiante.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    estudiante.Alergico = Convert.ToBoolean(reader["Alergico"]);
                    estudiante.Alergias = reader["Alergias"].ToString().Trim();
                    estudiante.ContactoAlternativoUno = reader["ContactoAlternativoUno"].ToString().Trim();
                    estudiante.ContactoAlternativoUnoTel = reader["ContactoAlternativoUnoTel"].ToString().Trim();
                    estudiante.ContactoAlternativoDos = reader["ContactoAlternativoDos"].ToString().Trim();
                    estudiante.ContactoAlternativoDosTel = reader["ContactoAlternativoDosTel"].ToString().Trim();
                    estudiante.Convenio.ID = Convert.ToInt32(reader["ConvenioID"]);
                    if (estudiante.Convenio.ID > 0)
                        estudiante.Convenio.Leer(strCon);
                    estudiante.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
                    estudiante.Validado = Convert.ToBoolean(reader["Validado"]);
                    estudiante.Deudor = Convert.ToBoolean(reader["Deudor"]);
                    estudiante.TipoPublicidad = (TipoPublicidad)Convert.ToInt32(reader["TipoPublicidad"]);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public static List<Estudiante> GetDeudores(string strCon)
        {
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            try
            {
                //cargo deudores de examenes
                lstEstudiantes = Estudiante.GetDeudoresExamen(strCon);
                //cargo deudores de mensualidades y le envio la lista con datos para que sean filtrados y no agregar dobles
                lstEstudiantes = Estudiante.GetDeudoresMensualidad(lstEstudiantes, strCon);
                lstEstudiantes = lstEstudiantes.OrderBy(e => e.ID).ToList();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstEstudiantes;
        }

        private static List<Estudiante> GetDeudoresExamen(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            string sql = "SELECT DISTINCT EE.EstudianteID FROM Examen E, ExamenEstudiante EE WHERE E.ID = EE.ExamenID AND E.GrupoID = EE.GrupoID AND EE.Pago = 0 AND E.AnioAsociado < @Anio";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Anio", DateTime.Now.Year));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    estudiante.Leer(strCon);
                    lstEstudiantes.Add(estudiante);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        //recibo lista de estudiantes ya cargada con los que deben examenes y filtro para no tener estudiantes duplicados
        private static List<Estudiante> GetDeudoresMensualidad(List<Estudiante> lstEstudiantes, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            string sql = "SELECT DISTINCT EstudianteID FROM Mensualidad WHERE Paga = 0 AND FechaVencimiento < @Fecha AND Anulado = 0";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Fecha", DateTime.Now));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    bool agregarLista = true;
                    Estudiante estudiante = new Estudiante();
                    estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    foreach (Estudiante estudianteAux in lstEstudiantes)
                    {
                        if (estudianteAux.ID == estudiante.ID)
                        {
                            agregarLista = false;
                        }
                    }
                    if (agregarLista)
                    {
                        estudiante.Leer(strCon);
                        lstEstudiantes.Add(estudiante);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstEstudiantes;
        }

        public bool BorrarGrupo(SqlConnection con, SqlTransaction tran = null)
        {
            //SqlConnection con = new SqlConnection(strCon);
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            string sql = "UPDATE Estudiante SET GrupoID = 0, MateriaID = 0, Activo = 0 WHERE ID = @ID";
            try
            {
                int resultado = 0;
                resultado = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                if (resultado > 0) seBorro = true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return seBorro;
        }

        public static ExamenEstudiante GetExamenEstudianteCuotas(Estudiante estudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            ExamenEstudiante examenEstudiante = null;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@EstudianteID", estudiante.ID));
            string sql = "SELECT * FROM ExamenEstudiante WHERE EstudianteID = @EstudianteID AND Pago = 0 AND Anulado = 0 AND ID = (SELECT MIN(ID) FROM ExamenEstudiante WHERE EstudianteID = @EstudianteID AND Pago = 0 AND Anulado = 0);";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    examenEstudiante = new ExamenEstudiante();
                    examenEstudiante.ID = Convert.ToInt32(reader["ID"]);
                    examenEstudiante.Examen.ID = Convert.ToInt32(reader["ExamenID"]);
                    examenEstudiante.Examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Examen.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Examen.LeerLazy(strCon);
                    examenEstudiante.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    examenEstudiante.Estudiante = estudiante;
                    examenEstudiante.FechaInscripcion = Convert.ToDateTime(reader["FechaInscripcion"]);
                    examenEstudiante.NotaFinal = Convert.ToDecimal(reader["NotaFinal"]);
                    examenEstudiante.NotaFinalLetra = reader["NotaFinalLetra"].ToString();
                    examenEstudiante.Aprobado = Convert.ToBoolean(reader["Aprobado"]);
                    examenEstudiante.CantCuotas = Convert.ToInt32(reader["CantCuotas"]);
                    examenEstudiante.FormaPago = (FormaPago)Convert.ToInt32(reader["FormaPago"]);
                    examenEstudiante.Pago = Convert.ToBoolean(reader["Pago"]);
                    examenEstudiante.Precio = Convert.ToDecimal(reader["Precio"]);
                    examenEstudiante.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    examenEstudiante.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    examenEstudiante.LeerCuotas(strCon);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return examenEstudiante;
        }

        public static ExamenEstudiante GetExamenEstudiantePorRendir(Estudiante estudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            ExamenEstudiante examenEstudiante = null;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@EstudianteID", estudiante.ID));
            lstParametros.Add(new SqlParameter("@Anio", DateTime.Now.Year));
            lstParametros.Add(new SqlParameter("@FechaHora", DateTime.Now));
            string sql = "SELECT * FROM ExamenEstudiante WHERE Anulado = 0 AND Aprobado = 0 AND EstudianteID = @EstudianteID AND ExamenID IN (SELECT ID FROM Examen WHERE AnioAsociado = @Anio AND FechaHora >= @FechaHora AND Calificado = 0);";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    examenEstudiante = new ExamenEstudiante();
                    examenEstudiante.ID = Convert.ToInt32(reader["ID"]);
                    examenEstudiante.Examen.ID = Convert.ToInt32(reader["ExamenID"]);
                    examenEstudiante.Examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Examen.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Examen.LeerLazy(strCon);
                    examenEstudiante.Estudiante = estudiante;
                    examenEstudiante.FechaInscripcion = Convert.ToDateTime(reader["FechaInscripcion"]);
                    examenEstudiante.NotaFinal = Convert.ToDecimal(reader["NotaFinal"]);
                    examenEstudiante.NotaFinalLetra = reader["NotaFinalLetra"].ToString();
                    examenEstudiante.Aprobado = false;
                    examenEstudiante.CantCuotas = Convert.ToInt32(reader["CantCuotas"]);
                    examenEstudiante.FormaPago = (FormaPago)Convert.ToInt32(reader["FormaPago"]);
                    examenEstudiante.Pago = Convert.ToBoolean(reader["Pago"]);
                    examenEstudiante.Precio = Convert.ToDecimal(reader["Precio"]);
                    examenEstudiante.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    examenEstudiante.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    examenEstudiante.LeerCuotas(strCon);
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return examenEstudiante;
        }

        //en la fachada se debe hacer leer de estudiante
        public bool DarDeBaja(string strCon, int mes)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            bool ok = false;
            List<SqlParameter> lstParametrosMensualidad = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sqlMensualidad = "SELECT ID FROM Mensualidad WHERE EstudianteID = @EstudianteID AND GrupoID = @GrupoID AND MateriaID = @MateriaID AND Paga = 0 AND MesAsociado > @Mes";
            lstParametrosMensualidad.Add(new SqlParameter("@EstudianteID", this.ID));
            lstParametrosMensualidad.Add(new SqlParameter("@GrupoID", this.GrupoID));
            lstParametrosMensualidad.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametrosMensualidad.Add(new SqlParameter("@Mes", mes));
            try
            {
                //primero anulo mensualidades, despues anulo examenes, por ultimo borro el grupo del estudiante y lo marco como inactivo
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sqlMensualidad, lstParametrosMensualidad, CommandType.Text);
                List<Mensualidad> lstMensualidades = new List<Mensualidad>();
                while (reader.Read())
                {
                    Mensualidad mensualidad = new Mensualidad();
                    mensualidad.ID = Convert.ToInt32(reader["ID"]);
                    lstMensualidades.Add(mensualidad);
                }
                reader.Close();
                tran = con.BeginTransaction();
                if (lstMensualidades.Count > 0)
                {
                    Mensualidad.AnularMensualidad(lstMensualidades, con, tran);
                }

                //empieza examen
                List<SqlParameter> lstParametrosExamen = new List<SqlParameter>();
                SqlDataReader readerExamen = null;
                string sqlExamen = "SELECT ID FROM ExamenEstudiante WHERE EstudianteID = @EstudianteID AND GrupoID = @GrupoID AND Aprobado = 0 AND Pago = 0";
                lstParametrosExamen.Add(new SqlParameter("@EstudianteID", this.ID));
                lstParametrosExamen.Add(new SqlParameter("@GrupoID", this.GrupoID));

                readerExamen = this.EjecutarConsulta(con, sqlExamen, lstParametrosExamen, CommandType.Text, tran);
                List<ExamenEstudiante> lstExamenEstudiante = new List<ExamenEstudiante>();
                while (readerExamen.Read())
                {
                    ExamenEstudiante examenEstudiante = new ExamenEstudiante();
                    examenEstudiante.ID = Convert.ToInt32(readerExamen["ID"]);
                    lstExamenEstudiante.Add(examenEstudiante);
                }
                readerExamen.Close();
                if (lstExamenEstudiante.Count > 0)
                {
                    ExamenEstudiante.Anular(lstExamenEstudiante, con, tran);
                }
                bool debeMensualidad = this.DebeMensualidad(con, tran, mes);
                string sqlEstudiante = "UPDATE Estudiante SET GrupoID = 0, MateriaID = 0, Activo = 0, Deudor = @Deudor, ConvenioID = 0 WHERE ID = @ID";
                List<SqlParameter> lstParametrosEstudiante = new List<SqlParameter>();
                lstParametrosEstudiante.Add(new SqlParameter("@ID", this.ID));
                lstParametrosEstudiante.Add(new SqlParameter("@Deudor", debeMensualidad));
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sqlEstudiante, lstParametrosEstudiante, CommandType.Text, tran);
                tran.Commit();
                ok = true;
            }
            catch (SqlException ex)
            {
                tran.Rollback();
                tran.Dispose();
                throw ex;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                tran.Dispose();
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return ok;
        }

        private bool DebeMensualidad(SqlConnection con, SqlTransaction tran, int mes)
        {
            bool esDeudor = false;
            string sql = "SELECT * FROM Mensualidad WHERE Paga = 0 AND MesAsociado < @MesAsociado AND EstudianteID = @EstudianteID AND GrupoID = @GrupoID";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@MesAsociado", mes));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            SqlDataReader reader = null;
            try
            {
                reader = this.EjecutarConsulta(con, sql, lstParametros, CommandType.Text, tran);
                while (reader.Read())
                {
                    esDeudor = true;
                    break;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
            }
            return esDeudor;
        }

        public bool DebeMensualidad(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool esDeudor = false;
            string sql = "SELECT * FROM Mensualidad WHERE Paga = 0 AND FechaVencimiento < @FechaActual AND EstudianteID = @EstudianteID AND Anulado = 0";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@FechaActual", DateTime.Now));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.ID));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = this.EjecutarConsulta(con, sql, lstParametros, CommandType.Text, null);
                while (reader.Read())
                {
                    esDeudor = true;
                    break;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return esDeudor;
        }

        public bool ValidarDebeMensualidadEnMatricula(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool esDeudor = false;
            string sql = "SELECT * FROM Mensualidad WHERE Paga = 0 AND FechaVencimiento < @FechaActual AND EstudianteID = @EstudianteID AND Anulado = 0 AND GrupoID <> @GrupoID AND MateriaID <> @MateriaID";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@FechaActual", DateTime.Now));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.ID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.ID));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = this.EjecutarConsulta(con, sql, lstParametros, CommandType.Text, null);
                while (reader.Read())
                {
                    esDeudor = true;
                    break;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return esDeudor;
        }

        public bool DebeExamen(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool esDeudor = false;
            string sql = "SELECT * FROM ExamenEstudiante WHERE ID IN (SELECT EE.ID FROM Examen E, ExamenEstudiante EE WHERE E.AnioAsociado < @Anio AND E.ID = EE.ExamenID AND E.GrupoID = EE.GrupoID) AND EstudianteID = @EstudianteID AND Pago = 0 AND Anulado = 0";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Anio", DateTime.Now.Year));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.ID));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = this.EjecutarConsulta(con, sql, lstParametros, CommandType.Text, null);
                while (reader.Read())
                {
                    esDeudor = true;
                    break;
                }
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return esDeudor;
        }

        public static List<DatosEscolaridad> GetEscolaridad(Estudiante estudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<DatosEscolaridad> lstDatosEscolaridad = new List<DatosEscolaridad>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@EstudianteID", estudiante.ID));
            string sql = "SELECT * FROM ExamenEstudiante WHERE EstudianteID = @EstudianteID AND Anulado = 0;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    ExamenEstudiante examenEstudiante = new ExamenEstudiante();
                    examenEstudiante.ID = Convert.ToInt32(reader["ID"]);
                    examenEstudiante.Examen.ID = Convert.ToInt32(reader["ExamenID"]);
                    examenEstudiante.Examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Examen.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Examen.Grupo.LeerConMateria(strCon);
                    Grupo grupo = examenEstudiante.Examen.Grupo;
                    Materia materia = examenEstudiante.Examen.Grupo.Materia;
                    examenEstudiante.Examen.LeerLazy(strCon);
                    examenEstudiante.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    examenEstudiante.Estudiante = estudiante;
                    examenEstudiante.FechaInscripcion = Convert.ToDateTime(reader["FechaInscripcion"]);
                    examenEstudiante.NotaFinal = Convert.ToDecimal(reader["NotaFinal"]);
                    examenEstudiante.NotaFinalLetra = reader["NotaFinalLetra"].ToString();
                    examenEstudiante.Aprobado = Convert.ToBoolean(reader["Aprobado"]);
                    examenEstudiante.CantCuotas = Convert.ToInt32(reader["CantCuotas"]);
                    examenEstudiante.FormaPago = (FormaPago)Convert.ToInt32(reader["FormaPago"]);
                    examenEstudiante.Pago = Convert.ToBoolean(reader["Pago"]);
                    examenEstudiante.Precio = Convert.ToDecimal(reader["Precio"]);
                    examenEstudiante.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    examenEstudiante.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    examenEstudiante.FaltasEnClase = Convert.ToDecimal(reader["FaltasEnClase"]);
                    examenEstudiante.NotaFinalListening = Convert.ToDecimal(reader["NotaFinalListening"]);
                    examenEstudiante.NotaFinalOral = Convert.ToDecimal(reader["NotaFinalOral"]);
                    examenEstudiante.NotaFinalWritting = Convert.ToDecimal(reader["NotaFinalWritting"]);
                    examenEstudiante.InternalAssessment = Convert.ToDecimal(reader["InternalAssessment"]);
                    if (examenEstudiante.Examen.Calificado)
                    {
                        DatosEscolaridad escolaridad = new DatosEscolaridad
                        {
                            Materia = materia,
                            Grupo = grupo,
                            ExamenEstudiante = examenEstudiante
                        };
                        lstDatosEscolaridad.Add(escolaridad);
                    }
                }
                lstDatosEscolaridad.OrderByDescending(e => e.ExamenEstudiante.ID).ToList();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstDatosEscolaridad;
        }

        public bool SetActivo(SqlConnection con, SqlTransaction tran)
        {
            bool SeModifico = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            string sql = "UPDATE Estudiante SET Activo = 1 WHERE ID = @ID;";
            try
            {
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                if (res > 0) SeModifico = true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SeModifico;
        }

        public bool SetDeudor(string strCon, bool esDeudor)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool SeModifico = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@EsDeudor", esDeudor));
            string sql = "UPDATE Estudiante SET Deudor = @EsDeudor WHERE ID = @ID;";
            try
            {
                con.Open();
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, null);
                if (res > 0) SeModifico = true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return SeModifico;
        }

        public static bool SetInactivoSinGrupoSinConvenio(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool SeModifico = false;
            string sql = "UPDATE Estudiante SET GrupoID = 0, MateriaID = 0, ConvenioID = 0, Activo = 0;";
            try
            {
                con.Open();
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, new List<SqlParameter>(), CommandType.Text, null);
                if (res > 0) SeModifico = true;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
            }
            return SeModifico;
        }

        public static List<ListaPublicidad> GetPublicidadCantidad(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<ListaPublicidad> lstPublicidad = new List<ListaPublicidad>();
            string sql = "SELECT miTabla.anio, (SELECT COUNT(id) FROM estudiante  WHERE YEAR(FechaIngreso)=miTabla.anio and tipopublicidad=0) AS Recomendacion, (SELECT COUNT(id) FROM estudiante  WHERE YEAR(FechaIngreso)=miTabla.anio and tipopublicidad=1) AS Facebook, (SELECT COUNT(id) FROM estudiante  WHERE YEAR(FechaIngreso)=miTabla.anio and tipopublicidad=2) AS Instagram, (SELECT COUNT(id) FROM estudiante  WHERE YEAR(FechaIngreso)=miTabla.anio and tipopublicidad=3) AS Twitter, (SELECT COUNT(id) FROM estudiante  WHERE YEAR(FechaIngreso)=miTabla.anio and tipopublicidad=4) AS Radio, (SELECT COUNT(id) FROM estudiante  WHERE YEAR(FechaIngreso)=miTabla.anio and tipopublicidad=5) AS Television, (SELECT COUNT(id) FROM estudiante  WHERE YEAR(FechaIngreso)=miTabla.anio and tipopublicidad=6) AS Otros FROM (SELECT YEAR(FechaIngreso) AS anio FROM estudiante  GROUP BY YEAR(FechaIngreso) ) miTabla ORDER BY mitabla.anio;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    ListaPublicidad publicidad = new ListaPublicidad();
                    publicidad.Anio = Convert.ToInt32(reader["Anio"]);
                    publicidad.Recomendacion = Convert.ToInt32(reader["Recomendacion"]);
                    publicidad.Facebook = Convert.ToInt32(reader["Facebook"]);
                    publicidad.Instagram = Convert.ToInt32(reader["Instagram"]);
                    publicidad.Twitter = Convert.ToInt32(reader["Twitter"]);
                    publicidad.Radio = Convert.ToInt32(reader["Radio"]);
                    publicidad.Television = Convert.ToInt32(reader["Television"]);
                    publicidad.Otros = Convert.ToInt32(reader["Otros"]);
                    lstPublicidad.Add(publicidad);
                }
                lstPublicidad.OrderByDescending(p => p.Anio).ToList();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            return lstPublicidad;
        }

        #endregion

        private SqlDataReader EjecutarConsulta(SqlConnection con, string sql, List<SqlParameter> listaParametros, CommandType tipo, SqlTransaction tran)
        {
            SqlDataReader reader = null;
            try
            {
                SqlCommand comando = new SqlCommand(sql, con);
                comando.CommandType = tipo;
                if (listaParametros != null)
                {
                    comando.Parameters.AddRange(listaParametros.ToArray());
                }
                if (tran != null)
                {
                    comando.Transaction = tran;
                }
                reader = comando.ExecuteReader();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            return reader;
        }

    }
}
