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
    public class Grupo : Persistencia, IPersistencia<Grupo>
    {
        public int ID { get; set; }
        [JsonIgnore]
        public Sucursal Sucursal { get; set; }
        public int SucursalID { get; set; }
        [JsonIgnore]
        public Materia Materia { get; set; }
        public int MateriaID { get; set; }
        [JsonIgnore]
        public Funcionario Funcionario { get; set; }
        public int FuncionarioID { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public decimal Precio { get; set; } //deberia ser el precio de la materia, pero puede variar
        public int Anio { get; set; }
        public bool Activo { get; set; }
        public List<GrupoDia> LstDias { get; set; } = new List<GrupoDia>();


        public Grupo()
        {
            this.Sucursal = new Sucursal();
            this.Materia = new Materia();
            this.Funcionario = new Funcionario();
        }

        public static bool ValidarGrupoInsert(Grupo grupo, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (grupo.Materia.ID < 1)
                {
                    errorMsg = "Debe asignar el grupo a una materia \n";
                }
                if (grupo.HoraInicio.Equals(String.Empty) || grupo.HoraFin.Equals(String.Empty))
                {
                    errorMsg += "Debe ingresar hora de inicio y fin \n";
                }
                if (grupo.Anio < 2000 || grupo.Anio > 3000)
                {
                    errorMsg += "Año del grupo incorrecto \n";
                }
                if (grupo.Precio < 1)
                {
                    errorMsg += "Debe ingresar precio \n";
                }
                if (grupo.LstDias.Count > 0)
                {
                    bool errorEnDias = false;
                    foreach (GrupoDia dia in grupo.LstDias)
                    {
                        if (dia.Dia.Equals(String.Empty))
                        {
                            errorEnDias = true;
                        }
                    }
                    if (errorEnDias)
                    {
                        errorMsg += "Debe ingresar los dias del grupo \n";
                    }
                }
                else
                {
                    errorMsg += "Debe ingresar los dias del grupo \n";
                }
                if (errorMsg.Equals(String.Empty) && Grupo.ExisteGrupo(grupo, strCon))
                {
                    errorMsg = "Ya existe el Grupo";
                }
                if (!errorMsg.Equals(String.Empty))
                {
                    throw new ValidacionException(errorMsg);
                }
                return true;
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

        public static bool ValidarGrupoModificar(Grupo grupo, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (grupo.Materia.ID < 1)
                {
                    errorMsg = "Debe asignar el grupo a una materia \n";
                }
                if (grupo.HoraInicio.Equals(String.Empty) || grupo.HoraFin.Equals(String.Empty))
                {
                    errorMsg += "Debe ingresar hora de inicio y fin \n";
                }
                if (grupo.Anio < 2000 || grupo.Anio > 3000)
                {
                    errorMsg += "Año del grupo incorrecto \n";
                }
                if (grupo.Precio < 1)
                {
                    errorMsg += "Debe ingresar precio \n";
                }
                if (grupo.LstDias.Count > 0)
                {
                    bool errorEnDias = false;
                    foreach (GrupoDia dia in grupo.LstDias)
                    {
                        if (dia.Dia.Equals(String.Empty))
                        {
                            errorEnDias = true;
                        }
                    }
                    if (errorEnDias)
                    {
                        errorMsg += "Debe ingresar los dias del grupo \n";
                    }
                }
                else
                {
                    errorMsg += "Debe ingresar los dias del grupo \n";
                }
                if (errorMsg.Equals(String.Empty) && !Grupo.ExisteGrupo(grupo, strCon))
                {
                    errorMsg = "No existe el Grupo que desea actualizar";
                }
                if (!errorMsg.Equals(String.Empty))
                {
                    throw new ValidacionException(errorMsg);
                }
                return true;
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

        public static bool ExisteGrupo(Grupo grupo, string strCon)
        {
            {
                SqlConnection con = new SqlConnection(strCon);
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";
                if (grupo.ID > 0 && grupo.Materia.ID > 0)
                {
                    sql = "SELECT * FROM Grupo WHERE ID = @ID AND MateriaID = @MateriaID";
                    lstParametros.Add(new SqlParameter("@ID", grupo.ID));
                    lstParametros.Add(new SqlParameter("@MateriaID", grupo.Materia.ID));
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
        }



        #region Persistencia


        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public bool LeerConMateria(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (this.ID > 0)
            {
                sql = "SELECT * FROM Grupo WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Grupo");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    this.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    this.Materia.Leer(strCon);
                    this.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    this.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    this.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.HoraInicio = reader["HoraInicio"].ToString().Trim();
                    this.HoraFin = reader["HoraFin"].ToString().Trim();
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.Anio = Convert.ToInt32(reader["Anio"]);
                    this.Activo = Convert.ToBoolean(reader["Activo"]);

                    //this.LeerDias(strCon);
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

        public bool Leer(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (this.ID > 0 && this.Materia.ID > 0)
            {
                sql = "SELECT * FROM Grupo WHERE ID = @ID AND MateriaID = @MateriaID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@MateriaID", this.Materia.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Grupo");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    this.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    this.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.HoraInicio = reader["HoraInicio"].ToString().Trim();
                    this.HoraFin = reader["HoraFin"].ToString().Trim();
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.Anio = Convert.ToInt32(reader["Anio"]);
                    this.Activo = Convert.ToBoolean(reader["Activo"]);

                    this.LeerDias(strCon);
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

        private void LeerDias(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlDataReader reader = null;
            string sql = "";
            try
            {
                this.LstDias.Clear();
                con.Open();
                sql = "SELECT * FROM GrupoDias WHERE GrupoID = @ID";
                List<SqlParameter> lstParametrosDias = new List<SqlParameter>();
                lstParametrosDias.Add(new SqlParameter("@ID", this.ID));
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametrosDias, CommandType.Text);
                while (reader.Read())
                {
                    GrupoDia grpDia = new GrupoDia();
                    grpDia.ID = Convert.ToInt32(reader["ID"]);
                    grpDia.Dia = reader["Dia"].ToString();
                    this.LstDias.Add(grpDia);
                }
                reader.Close();
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
        }

        public bool Guardar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            bool seGuardo = false;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                this.ID = 0;
                this.ID = (int)Herramientas.ObtenerNumerador("GRUPO", strCon);
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                string sqlGrupo = "INSERT INTO Grupo VALUES (@ID, @MateriaID, @SucursalID, @FuncionarioID, @HoraInicio, @HoraFin, @Precio, @Anio, 1);";
                int res = 0;
                res = Convert.ToInt32(Persistencia.EjecutarNoQuery(con, sqlGrupo, lstParametros, CommandType.Text, tran));
                if (res > 0)
                {
                    string sqlDias = "";
                    foreach (GrupoDia dia in this.LstDias)
                    {
                        List<SqlParameter> lstParametrosDia = new List<SqlParameter>();
                        lstParametrosDia.Add(new SqlParameter("@ID", this.ID));
                        lstParametrosDia.Add(new SqlParameter("@Dia", dia.Dia));
                        lstParametrosDia.Add(new SqlParameter("@GrupoDiaID", dia.ID));
                        sqlDias = "INSERT INTO GrupoDias VALUES (@ID, @GrupoDiaID, @Dia);";
                        Persistencia.EjecutarNoQuery(con, sqlDias, lstParametrosDia, CommandType.Text, tran);
                    }
                    MateriaHistorial materiaHistorial = new MateriaHistorial
                    {
                        ID = 0,
                        MateriaID = this.MateriaID,
                        SucursalID = this.SucursalID,
                        Anio = this.Anio
                    };
                    materiaHistorial.ModificarCantidadGrupos(con, tran);
                    tran.Commit();
                    seGuardo = true;
                }
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
            return seGuardo;
        }

        public bool Modificar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            bool SeModifico = false;
            List<SqlParameter> lstParametros = this.ObtenerParametros();
            string sql = "UPDATE Grupo SET MateriaID = @MateriaID, SucursalID = @SucursalID, FuncionarioID = @FuncionarioID, HoraInicio = @HoraInicio, HoraFin = @HoraFin, Precio = @Precio, Anio = @Anio WHERE ID = @ID;";
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                if (res > 0)
                {
                    string sqlEliminarDias = "DELETE FROM GrupoDias WHERE GrupoID = @ID";
                    List<SqlParameter> lstParamEliminarDias = new List<SqlParameter>();
                    lstParamEliminarDias.Add(new SqlParameter("@ID", this.ID));
                    Persistencia.EjecutarNoQuery(con, sqlEliminarDias, lstParamEliminarDias, CommandType.Text, tran);
                    foreach (GrupoDia grpDia in this.LstDias)
                    {
                        List<SqlParameter> lstParametrosDia = new List<SqlParameter>();
                        lstParametrosDia.Add(new SqlParameter("@Dia", grpDia.Dia));
                        lstParametrosDia.Add(new SqlParameter("@ID", this.ID));
                        lstParametrosDia.Add(new SqlParameter("@GrupoDiaID", grpDia.ID));
                        string sqlDias = "";
                        sqlDias = "INSERT INTO GrupoDias VALUES (@ID, @GrupoDiaID, @Dia);";
                        Persistencia.EjecutarNoQuery(con, sqlDias, lstParametrosDia, CommandType.Text, tran);
                    }
                    tran.Commit();
                    SeModifico = true;
                }
            }
            catch (SqlException ex)
            {
                tran.Rollback();
                tran.Dispose();
                if (ex.Message.Contains("FK_Estudiante_Grupo"))
                {
                    throw new ValidacionException("No se puede modificar la materia del grupo porque tiene estudiantes asignados");
                }
                else
                {
                    throw ex;
                }
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
            return SeModifico;
        }

        public bool Eliminar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            List<SqlParameter> lstParametrosDias = new List<SqlParameter>();
            lstParametrosDias.Add(new SqlParameter("@ID", this.ID));
            string sql = "DELETE FROM Grupo WHERE ID = @ID";
            string sqlDias = "DELETE FROM GrupoDias WHERE GrupoID = @ID";
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                int resultado = 0;
                Persistencia.EjecutarNoQuery(con, sqlDias, lstParametrosDias, CommandType.Text, tran);
                resultado = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                if (resultado > 0)
                {
                    tran.Commit();
                    seBorro = true;
                }
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
            return seBorro;
        }

        public List<Grupo> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Grupo> lstGrupo = new List<Grupo>();
            string sql = "SELECT * FROM Grupo WHERE ID <> 0;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Grupo grupo = new Grupo();
                    grupo.ID = Convert.ToInt32(reader["ID"]);
                    grupo.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    grupo.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    grupo.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    grupo.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    grupo.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    grupo.HoraInicio = reader["HoraInicio"].ToString().Trim();
                    grupo.HoraFin = reader["HoraFin"].ToString().Trim();
                    grupo.Precio = Convert.ToDecimal(reader["Precio"]);
                    grupo.Anio = Convert.ToInt32(reader["Anio"]);
                    grupo.Activo = Convert.ToBoolean(reader["Activo"]);
                    grupo.LeerDias(strCon);
                    lstGrupo.Add(grupo);
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
            return lstGrupo;
        }

        public List<Grupo> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@FuncionarioID", this.FuncionarioID));
            lstParametros.Add(new SqlParameter("@HoraInicio", this.HoraInicio));
            lstParametros.Add(new SqlParameter("@HoraFin", this.HoraFin));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            lstParametros.Add(new SqlParameter("@Anio", this.Anio));
            lstParametros.Add(new SqlParameter("@Activo", this.Activo));
            return lstParametros;
        }

        public List<Grupo> GetAllByAnio(int anio, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Grupo> lstGrupo = new List<Grupo>();
            string sql = "SELECT * FROM Grupo WHERE Anio = @Anio;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Anio", anio));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Grupo grupo = new Grupo();
                    grupo.ID = Convert.ToInt32(reader["ID"]);
                    grupo.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    grupo.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    grupo.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    grupo.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    grupo.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    grupo.HoraInicio = reader["HoraInicio"].ToString().Trim();
                    grupo.HoraFin = reader["HoraFin"].ToString().Trim();
                    grupo.Precio = Convert.ToDecimal(reader["Precio"]);
                    grupo.Anio = Convert.ToInt32(reader["Anio"]);
                    grupo.Activo = Convert.ToBoolean(reader["Activo"]);
                    grupo.LeerDias(strCon);
                    lstGrupo.Add(grupo);
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
            return lstGrupo;
        }

        public List<Estudiante> GetEstudiantes(string strCon)
        {
            if (this.ID < 1)
            {
                throw new ValidacionException("ID del grupo no puede ser vacio");
            }
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiantes = new List<Estudiante>();
            string sql = "SELECT * FROM Estudiante WHERE GrupoID = @ID;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
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
                    estudiante.Grupo.ID = this.ID;
                    estudiante.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.GrupoID = this.ID;
                    estudiante.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    estudiante.Activo = Convert.ToBoolean(reader["Activo"]);
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

        private bool ExisteGrupoDia(GrupoDia dia, string strCon)
        {
            {
                SqlConnection con = new SqlConnection(strCon);
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";
                if (dia.ID > 0 && this.ID > 0)
                {
                    sql = "SELECT * FROM GrupoDias WHERE ID = @ID AND GrupoID = @GrupoID";
                    lstParametros.Add(new SqlParameter("@ID", dia.ID));
                    lstParametros.Add(new SqlParameter("@GrupoID", this.ID));
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
        }

        public static bool SetInactivo(int id, SqlConnection con, SqlTransaction tran)
        {
            bool SeModifico = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", id));
            string sql = "UPDATE Grupo SET Activo = 0 WHERE ID = @ID;";
            try
            {
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                if (res > 0)
                {
                    SeModifico = true;
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

            return SeModifico;
        }

        #endregion


    }
}
