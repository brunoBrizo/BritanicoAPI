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
                string sql = "INSERT INTO Estudiante VALUES (@Nombre, @TipoDocumento, @CI, @Tel, @Email, @Direccion, @FechaNac, @Alergico, @Alergias, @ContactoAlternativoUno, @ContactoAlternativoUnoTel, @ContactoAlternativoDos, @ContactoAlternativoDosTel, @ConvenioID, @GrupoID, @MateriaID, @Activo, @Validado); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE Estudiante SET Nombre = @Nombre, Tel = @Tel, Email = @Email, Direccion = @Direccion, FechaNac = @FechaNac, Alergico = @Alergico, Alergias = @Alergias, ContactoAlternativoUno = @ContactoAlternativoUno, ContactoAlternativoUnoTel = @ContactoAlternativoUnoTel, ContactoAlternativoDos = @ContactoAlternativoDos, ContactoAlternativoDosTel = @ContactoAlternativoDosTel, ConvenioID = @ConvenioID, GrupoID = @GrupoID, MateriaID = @MateriaID, Activo = @Activo, Validado = @Validado WHERE ID = @ID;";
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
            lstParametros.Add(new SqlParameter("@ConvenioID", this.Convenio.ID));
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

        #endregion


    }
}
