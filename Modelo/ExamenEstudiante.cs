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
    public class ExamenEstudiante : Persistencia, IPersistencia<ExamenEstudiante>
    {
        public int ID { get; set; }
        public Examen Examen { get; set; } //Examen tiene la lista tambien
        public Estudiante Estudiante { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public decimal NotaFinal { get; set; }
        public string NotaFinalLetra { get; set; }
        public bool Aprobado { get; set; }
        public int CantCuotas { get; set; }
        public FormaPago FormaPago { get; set; }
        public bool Pago { get; set; }
        public decimal Precio { get; set; }
        [JsonIgnore]
        public Funcionario Funcionario { get; set; }
        public int FuncionarioID { get; set; }
        public List<ExamenEstudianteCuota> LstCuotas { get; set; } = new List<ExamenEstudianteCuota>();



        public ExamenEstudiante()
        {
            this.Examen = new Examen();
            this.Estudiante = new Estudiante();
            this.Funcionario = new Funcionario();
        }


        //ver metodo para agregar cuotas y manejar ID

        public static bool ValidarExamenEstudianteInsert(ExamenEstudiante examenEstudiante, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (examenEstudiante.Examen.ID < 1)
                    errorMsg = "Debe ingresar un examen \n";
                if (examenEstudiante.Estudiante.ID < 1)
                    errorMsg += "Debe asociar el examen a un estudiante \n";
                if (examenEstudiante.FechaInscripcion <= DateTime.MinValue)
                    errorMsg += "Fecha de inscripcion invalida \n";
                if (examenEstudiante.CantCuotas < 1)
                    errorMsg += "Debe ingresar la cantidad de cuotas \n";
                if (examenEstudiante.Precio < 1)
                    errorMsg += "Debe ingresar precio \n";
                if (examenEstudiante.Funcionario.ID < 1)
                    errorMsg += "Debe asociar el examen a un funcionario \n";
                if (errorMsg.Equals(String.Empty) && ExamenEstudiante.ExisteExamenEstudiante(examenEstudiante, strCon))
                    errorMsg = "El estudiante ya esta inscripto al examen";
                if (!errorMsg.Equals(String.Empty))
                    throw new ValidacionException(errorMsg);
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

        public static bool ValidarExamenEstudianteModificar(ExamenEstudiante examenEstudiante, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (examenEstudiante.Examen.ID < 1)
                    errorMsg = "Debe ingresar un examen \n";
                if (examenEstudiante.Estudiante.ID < 1)
                    errorMsg += "Debe asociar el examen a un estudiante \n";
                if (examenEstudiante.FechaInscripcion <= DateTime.MinValue)
                    errorMsg += "Fecha de inscripcion invalida \n";
                if (examenEstudiante.CantCuotas < 1)
                    errorMsg += "Debe ingresar la cantidad de cuotas \n";
                if (examenEstudiante.Precio < 1)
                    errorMsg += "Debe ingresar precio \n";
                if (examenEstudiante.Funcionario.ID < 1)
                    errorMsg += "Debe asociar el examen a un funcionario \n";
                if (errorMsg.Equals(String.Empty) && !ExamenEstudiante.ExisteExamenEstudiante(examenEstudiante, strCon))
                    errorMsg = "No existe el examen que desea modificar";
                if (!errorMsg.Equals(String.Empty))
                    throw new ValidacionException(errorMsg);
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

        public static bool ExisteExamenEstudiante(ExamenEstudiante examenEstudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (examenEstudiante.Examen.ID > 0 && examenEstudiante.Examen.Grupo.ID > 0 && examenEstudiante.Estudiante.ID > 0)
            {
                sql = "SELECT * FROM ExamenEstudiante WHERE ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID";
                lstParametros.Add(new SqlParameter("@ExamenID", examenEstudiante.Examen.ID));
                lstParametros.Add(new SqlParameter("@GrupoID", examenEstudiante.Examen.Grupo.ID));
                lstParametros.Add(new SqlParameter("@EstudianteID", examenEstudiante.Estudiante.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Examen del estudiante");
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
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (this.ID > 0 && this.Examen.ID > 0 && this.Examen.Grupo.ID > 0 && this.Estudiante.ID > 0)
            {
                sql = "SELECT * FROM ExamenEstudiante WHERE ID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@ExamenID", this.Examen.ID));
                lstParametros.Add(new SqlParameter("@GrupoID", this.Examen.Grupo.ID));
                lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Examen del estudiante");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.FechaInscripcion = Convert.ToDateTime(reader["FechaInscripcion"]);
                    this.NotaFinal = Convert.ToDecimal(reader["NotaFinal"]);
                    this.NotaFinalLetra = reader["NotaFinalLetra"].ToString();
                    this.Aprobado = Convert.ToBoolean(reader["Aprobado"]);
                    this.CantCuotas = Convert.ToInt32(reader["CantCuotas"]);
                    this.FormaPago = (FormaPago)Convert.ToInt32(reader["FormaPago"]);
                    this.Pago = Convert.ToBoolean(reader["Pago"]);
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);

                    this.LeerCuotas(strCon);
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

        private void LeerCuotas(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            try
            {
                string sql = "SELECT * FROM ExamenEstudianteCuota WHERE ExamenEstudianteID = @ExamenEstudianteID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID";
                lstParametros.Add(new SqlParameter("@ExamenEstudianteID", this.ID));
                lstParametros.Add(new SqlParameter("@ExamenID", this.Examen.ID));
                lstParametros.Add(new SqlParameter("@GrupoID", this.Examen.Grupo.ID));
                lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    ExamenEstudianteCuota examenCuota = new ExamenEstudianteCuota();
                    examenCuota.ID = Convert.ToInt32(reader["ID"]);
                    examenCuota.NroCuota = Convert.ToInt32(reader["NroCuota"]);
                    examenCuota.Precio = Convert.ToDecimal(reader["Precio"]);
                    examenCuota.FechaPago = Convert.ToDateTime(reader["FechaPago"]);
                    examenCuota.CuotaPaga = Convert.ToBoolean(reader["CuotaPaga"]);
                    this.LstCuotas.Add(examenCuota);
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
                this.ID = (int)Herramientas.ObtenerNumerador("EXAMES", strCon);
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                string sqlGrupo = "INSERT INTO ExamenEstudiante VALUES (@ID, @ExamenID, @GrupoID, @EstudianteID, @FechaInscripcion, @NotaFinal, @NotaFinalLetra, @Aprobado, @CantCuotas, @FormaPago, @Pago, @Precio, @FuncionarioID);";
                int res = 0;
                res = Convert.ToInt32(Persistencia.EjecutarNoQuery(con, sqlGrupo, lstParametros, CommandType.Text, tran));
                if (res > 0)
                {
                    string sqlCuotas = "";
                    foreach (ExamenEstudianteCuota examenCuota in this.LstCuotas)
                    {
                        List<SqlParameter> lstParametrosCuota = this.ObtenerParametrosExamenEstudianteCuota(examenCuota);
                        sqlCuotas = "INSERT INTO ExamenEstudianteCuota VALUES (@ID, @ExamenEstudianteID, @ExamenID, @GrupoID, @EstudianteID, @NroCuota, @Precio, @FechaPago, @CuotaPaga);";
                        Persistencia.EjecutarNoQuery(con, sqlCuotas, lstParametrosCuota, CommandType.Text, tran);
                    }
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
            string sql = "UPDATE ExamenEstudiante SET FechaInscripcion = @FechaInscripcion, NotaFinal = @NotaFinal, NotaFinalLetra = @NotaFinalLetra, Aprobado = @Aprobado, CantCuotas = @CantCuotas, FormaPago = @FormaPago, Pago = @Pago, Precio = @Precio WHERE ID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID;";
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                if (res > 0)
                {
                    foreach (ExamenEstudianteCuota examenCuota in this.LstCuotas)
                    {
                        List<SqlParameter> lstParametrosExamenEstudianteCuota = this.ObtenerParametrosExamenEstudianteCuota(examenCuota);
                        string sqlCuotas = "UPDATE ExamenEstudianteCuota SET NroCuota = @NroCuota, Precio = @Precio, FechaPago = @FechaPago, CuotaPaga = @CuotaPaga WHERE ID = @ID AND ExamenEstudianteID = @ExamenEstudianteID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID;";
                        Persistencia.EjecutarNoQuery(con, sqlCuotas, lstParametrosExamenEstudianteCuota, CommandType.Text, tran);
                    }
                    tran.Commit();
                    SeModifico = true;
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
            return SeModifico;
        }

        public bool Eliminar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@ExamenID", this.Examen.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.Examen.Grupo.ID));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            //se hace una lista nueva ya que no se permite utilizar la misma durante la ejecucion
            List<SqlParameter> lstParametrosCuota = new List<SqlParameter>();
            lstParametrosCuota.Add(new SqlParameter("@ID", this.ID));
            lstParametrosCuota.Add(new SqlParameter("@ExamenID", this.Examen.ID));
            lstParametrosCuota.Add(new SqlParameter("@GrupoID", this.Examen.Grupo.ID));
            lstParametrosCuota.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            string sql = "DELETE FROM ExamenEstudiante WHERE ID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID";
            string sqlCuota = "DELETE FROM ExamenEstudianteCuota WHERE ExamenEstudianteID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID";
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                int resultado = 0;
                Persistencia.EjecutarNoQuery(con, sqlCuota, lstParametrosCuota, CommandType.Text, tran);
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

        public List<ExamenEstudiante> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<ExamenEstudiante> lstExamenes = new List<ExamenEstudiante>();
            string sql = "SELECT * FROM ExamenEstudiante;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    ExamenEstudiante examenEstudiante = new ExamenEstudiante();
                    examenEstudiante.ID = Convert.ToInt32(reader["ID"]);
                    examenEstudiante.Examen.ID = Convert.ToInt32(reader["ExamenID"]);
                    examenEstudiante.Examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Examen.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Examen.LeerLazy(strCon);
                    examenEstudiante.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    examenEstudiante.Estudiante.LeerLazy(strCon);
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
                    lstExamenes.Add(examenEstudiante);
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
            return lstExamenes;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@ExamenID", this.Examen.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.Examen.Grupo.ID));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            lstParametros.Add(new SqlParameter("@FechaInscripcion", this.FechaInscripcion));
            lstParametros.Add(new SqlParameter("@NotaFinal", this.NotaFinal));
            lstParametros.Add(new SqlParameter("@NotaFinalLetra", this.NotaFinalLetra));
            lstParametros.Add(new SqlParameter("@Aprobado", this.Aprobado));
            lstParametros.Add(new SqlParameter("@CantCuotas", this.CantCuotas));
            lstParametros.Add(new SqlParameter("@FormaPago", this.FormaPago));
            lstParametros.Add(new SqlParameter("@Pago", this.Pago));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            lstParametros.Add(new SqlParameter("@FuncionarioID", this.FuncionarioID));
            return lstParametros;
        }

        private List<SqlParameter> ObtenerParametrosExamenEstudianteCuota(ExamenEstudianteCuota examenCuota)
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", examenCuota.ID));
            lstParametros.Add(new SqlParameter("@ExamenEstudianteID", this.ID));
            lstParametros.Add(new SqlParameter("@ExamenID", this.Examen.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.Examen.Grupo.ID));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            lstParametros.Add(new SqlParameter("@NroCuota", examenCuota.NroCuota));
            lstParametros.Add(new SqlParameter("@Precio", examenCuota.Precio));
            lstParametros.Add(new SqlParameter("@FechaPago", examenCuota.FechaPago));
            lstParametros.Add(new SqlParameter("@CuotaPaga", examenCuota.CuotaPaga));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (this.ID > 0 && this.Examen.ID > 0)
            {
                sql = "SELECT * FROM ExamenEstudiante WHERE ID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@ExamenID", this.Examen.ID));
                lstParametros.Add(new SqlParameter("@GrupoID", this.Examen.Grupo.ID));
                lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Examen del estudiante");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    this.FechaInscripcion = Convert.ToDateTime(reader["FechaInscripcion"]);
                    this.NotaFinal = Convert.ToDecimal(reader["NotaFinal"]);
                    this.NotaFinalLetra = reader["NotaFinalLetra"].ToString();
                    this.Aprobado = Convert.ToBoolean(reader["Aprobado"]);
                    this.CantCuotas = Convert.ToInt32(reader["CantCuotas"]);
                    this.FormaPago = (FormaPago)Convert.ToInt32(reader["FormaPago"]);
                    this.Pago = Convert.ToBoolean(reader["Pago"]);
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);

                    this.LeerCuotas(strCon);
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

        public List<ExamenEstudiante> GetAllLazy(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<ExamenEstudiante> lstExamenes = new List<ExamenEstudiante>();
            string sql = "SELECT * FROM ExamenEstudiante;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    ExamenEstudiante examenEstudiante = new ExamenEstudiante();
                    examenEstudiante.ID = Convert.ToInt32(reader["ID"]);
                    examenEstudiante.Examen.ID = Convert.ToInt32(reader["ExamenID"]);
                    examenEstudiante.Examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examenEstudiante.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
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
                    lstExamenes.Add(examenEstudiante);
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
            return lstExamenes;
        }

        #endregion


    }
}
