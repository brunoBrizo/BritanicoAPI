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
    public class MatriculaEstudiante : Persistencia, IPersistencia<MatriculaEstudiante>
    {
        public int ID { get; set; }
        public Matricula Matricula { get; set; }
        [JsonIgnore]
        public Sucursal Sucursal { get; set; } //es la sucursal donde se le tomo la matricula, independiente del curso donde lo va a hacer  
        public int SucursalID { get; set; }
        public Estudiante Estudiante { get; set; } //el estudiante tiene la lista de matriculas, esto se deja para saber cuantos estudiantes se matricularon
        [JsonIgnore]
        public Grupo Grupo { get; set; }
        public int GrupoID { get; set; }
        public int MateriaID { get; set; }
        public DateTime FechaHora { get; set; }
        [JsonIgnore]
        public Funcionario Funcionario { get; set; }
        public int FuncionarioID { get; set; }
        public decimal Descuento { get; set; }
        public decimal Precio { get; set; }


        public MatriculaEstudiante()
        {
            this.Matricula = new Matricula();
            this.Sucursal = new Sucursal();
            this.Estudiante = new Estudiante();
            this.Grupo = new Grupo();
            this.Funcionario = new Funcionario();
        }

        public static bool ValidarMatriculaEstudianteInsert(MatriculaEstudiante matriculaEstudiante, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (matriculaEstudiante.Matricula.ID < 1)
                {
                    errorMsg = "Debe asociar a una matricula \n";
                }
                if (matriculaEstudiante.Estudiante.ID < 1)
                {
                    errorMsg += "Debe asociar la matricula a un estudiante \n";
                }
                if (matriculaEstudiante.Grupo.ID < 1 || matriculaEstudiante.Grupo.Materia.ID < 1)
                {
                    errorMsg += "Debe asociar la matricula a un grupo \n";
                }
                if (matriculaEstudiante.Funcionario.ID < 1)
                {
                    errorMsg += "Debe asociar la matricula a un funcionario \n";
                }
                if (matriculaEstudiante.Precio < 1 && (matriculaEstudiante.Estudiante.Convenio == null || matriculaEstudiante.Estudiante.Convenio.ID < 1))
                {
                    errorMsg += "Debe ingresar precio \n";
                }
                if (errorMsg.Equals(String.Empty))
                {
                    if (matriculaEstudiante.Estudiante.GrupoID > 0)
                    {
                        errorMsg = "El estudiante ya esta inscripto a un grupo \n";
                    }
                }
                if (!errorMsg.Equals(String.Empty))
                {
                    throw new ValidacionException(errorMsg);
                }
                Estudiante estudianteAux = matriculaEstudiante.Estudiante;
                estudianteAux.GrupoID = matriculaEstudiante.GrupoID;
                estudianteAux.MateriaID = matriculaEstudiante.MateriaID;
                if (estudianteAux.ValidarDebeMensualidadEnMatricula(strCon))
                {
                    throw new ValidacionException("El estudiante tiene mensualidades anteriores IMPAGAS");
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

        public static bool ValidarMatriculaEstudianteModificar(MatriculaEstudiante matriculaEstudiante, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (matriculaEstudiante.Matricula.ID < 1)
                {
                    errorMsg = "Debe asociar a una matricula \n";
                }
                if (matriculaEstudiante.Estudiante.ID < 1)
                {
                    errorMsg += "Debe asociar la matricula a un estudiante \n";
                }
                if (matriculaEstudiante.Grupo.ID < 1 || matriculaEstudiante.Grupo.Materia.ID < 1)
                {
                    errorMsg += "Debe asociar la matricula a un grupo \n";
                }
                if (matriculaEstudiante.Funcionario.ID < 1)
                {
                    errorMsg += "Debe asociar la matricula a un funcionario \n";
                }
                if (matriculaEstudiante.Precio < 1)
                {
                    errorMsg += "Debe ingresar precio \n";
                }
                if (errorMsg.Equals(String.Empty) && !MatriculaEstudiante.ExisteMatriculaEstudiante(matriculaEstudiante, strCon))
                {
                    errorMsg = "No existe la matricula que desea modificar \n";
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

        public static bool ExisteMatriculaEstudiante(MatriculaEstudiante matriculaEstudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            SqlDataReader reader = null;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            string sql = "";
            if (matriculaEstudiante.Matricula.ID > 0 && matriculaEstudiante.Estudiante.ID > 0)
            {
                sql = "SELECT * FROM MatriculaEstudiante WHERE MatriculaID = @MatriculaID AND EstudianteID = @EstudianteID";
                lstParametros.Add(new SqlParameter("@MatriculaID", matriculaEstudiante.Matricula.ID));
                lstParametros.Add(new SqlParameter("@EstudianteID", matriculaEstudiante.Estudiante.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscarla matricula");
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

        #region Persistencia

        public bool Leer(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            SqlDataReader reader = null;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            string sql = "";
            if (this.ID > 0 && this.Matricula.ID > 0 && this.Estudiante.ID > 0 && this.Grupo.ID > 0)
            {
                sql = "SELECT * FROM MatriculaEstudiante WHERE ID = @ID AND MatriculaID = @MatriculaID AND EstudianteID = @EstudianteID AND GrupoID = @GrupoID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@MatriculaID", this.Matricula.ID));
                lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
                lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar la Matricula del estudiante");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    this.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    this.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    this.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    this.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    this.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.Descuento = Convert.ToDecimal(reader["Descuento"]);
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
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
            SqlTransaction tran = null;
            bool seGuardo = false;            
            try
            {
                this.ID = 0;
                this.ID = (int)Herramientas.ObtenerNumerador("MATRES", strCon);
                con.Open();
                tran = con.BeginTransaction();
                if (this.ID > 0)
                {
                    List<SqlParameter> lstParametros = this.ObtenerParametros();
                    string sql = "INSERT INTO MatriculaEstudiante VALUES (@ID, @MatriculaID, @SucursalID, @EstudianteID, @GrupoID, @MateriaID, @FechaHora, @FuncionarioID, @Descuento, @Precio);";
                    int res = 0;
                    res = Convert.ToInt32(Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran));
                    if (res > 0 && this.Estudiante.SetActivo(con, tran))
                    {
                        MateriaHistorial materiaHistorial = new MateriaHistorial
                        {
                            ID = 0,
                            MateriaID = this.MateriaID,
                            SucursalID = this.SucursalID
                        };
                        materiaHistorial.ModificarCantidadAlumnos(con, tran, this.GrupoID);
                        seGuardo = true;
                    }
                    else
                    {
                        tran.Rollback();
                        tran.Dispose();
                        return false;
                    }
                }
                tran.Commit();
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
            return seGuardo;
        }

        public bool Modificar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool SeModifico = false;
            List<SqlParameter> lstParametros = this.ObtenerParametros();
            string sql = "UPDATE MatriculaEstudiante SET SucursalID = @SucursalID, FechaHora = @FechaHora, Descuento = @Descuento, Precio = @Precio WHERE ID = @ID AND MatriculaID = @MatriculaID AND EstudianteID = @EstudianteID AND GrupoID = @GrupoID;";
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

        public bool Eliminar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@MatriculaID", this.Matricula.ID));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            string sql = "DELETE FROM MatriculaEstudiante WHERE ID = @ID AND MatriculaID = @MatriculaID AND EstudianteID = @EstudianteID AND GrupoID = @GrupoID";
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

        public List<MatriculaEstudiante> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<MatriculaEstudiante> lstMatriculas = new List<MatriculaEstudiante>();
            string sql = "SELECT * FROM MatriculaEstudiante;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    MatriculaEstudiante matricula = new MatriculaEstudiante();
                    matricula.ID = Convert.ToInt32(reader["ID"]);
                    matricula.Matricula.ID = Convert.ToInt32(reader["MatriculaID"]);
                    //matricula.Matricula.LeerLazy(strCon);
                    matricula.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    matricula.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    //matricula.Sucursal.LeerLazy(strCon);
                    matricula.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    matricula.Estudiante.LeerLazy(strCon);
                    matricula.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    matricula.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    matricula.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    matricula.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    //matricula.Grupo.LeerLazy(strCon);
                    matricula.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    matricula.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    matricula.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    //matricula.Funcionario.LeerLazy(strCon);
                    matricula.Descuento = Convert.ToDecimal(reader["Descuento"]);
                    matricula.Precio = Convert.ToDecimal(reader["Precio"]);
                    lstMatriculas.Add(matricula);
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
            return lstMatriculas;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@MatriculaID", this.Matricula.ID));
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@FuncionarioID", this.FuncionarioID));
            lstParametros.Add(new SqlParameter("@Descuento", this.Descuento));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public List<MatriculaEstudiante> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        #endregion

    }
}
