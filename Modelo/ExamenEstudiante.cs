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
        public Examen Examen { get; set; }
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
        public bool Anulado { get; set; }
        public decimal FaltasEnClase { get; set; }
        public decimal NotaFinalOral { get; set; }
        public decimal NotaFinalWritting { get; set; }
        public decimal NotaFinalListening { get; set; }
        public decimal InternalAssessment { get; set; }

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
                if (examenEstudiante.Precio < 1 && (examenEstudiante.Estudiante.Convenio == null || examenEstudiante.Estudiante.Convenio.ID < 1))
                    errorMsg += "Debe ingresar precio \n";
                if (examenEstudiante.Funcionario.ID < 1)
                    errorMsg += "Debe asociar el examen a un funcionario \n";
                if (errorMsg.Equals(String.Empty) && ExamenEstudiante.ExisteExamenEstudiante(examenEstudiante, strCon))
                {
                    errorMsg = "El estudiante ya esta inscripto al examen";
                }
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
                if (examenEstudiante.Precio < 1 && (examenEstudiante.Estudiante.Convenio == null || examenEstudiante.Estudiante.Convenio.ID < 1))
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

        public static ExamenEstudiante GetExamenPendientePorEstudiante(List<ExamenEstudiante> lstExamenEstudiante)
        {
            try
            {
                ExamenEstudiante ultExamen = new ExamenEstudiante();
                lstExamenEstudiante.OrderByDescending(e => e.Examen.AnioAsociado);
                foreach (ExamenEstudiante examenEstudiante in lstExamenEstudiante)
                {
                    ultExamen = examenEstudiante;
                    break;
                }
                if (!ultExamen.Aprobado)
                {
                    return ultExamen;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ExamenEstudiante GetExamenPendientePorEstudiante(Estudiante estudiante, string strCon)
        {
            try
            {
                List<ExamenEstudiante> lstExamenEstudiante = ExamenEstudiante.GetByEstudiante(estudiante, strCon);
                if (lstExamenEstudiante == null)
                {
                    return null;
                }
                lstExamenEstudiante.OrderByDescending(e => e.Examen.AnioAsociado);
                ExamenEstudiante ultExamen = new ExamenEstudiante();
                foreach (ExamenEstudiante examenEstudiante in lstExamenEstudiante)
                {
                    ultExamen = examenEstudiante;
                    break;
                }
                if (!ultExamen.Aprobado)
                {
                    return ultExamen;
                }
                return null;
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

        #region Persistencia

        public bool Leer(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (this.ID > 0 && this.Examen.ID > 0 && this.Examen.GrupoID > 0 && this.Estudiante.ID > 0)
            {
                sql = "SELECT * FROM ExamenEstudiante WHERE ID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@ExamenID", this.Examen.ID));
                lstParametros.Add(new SqlParameter("@GrupoID", this.Examen.GrupoID));
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
                    this.FaltasEnClase = Convert.ToDecimal(reader["FaltasEnClase"]);
                    this.NotaFinalListening = Convert.ToDecimal(reader["NotaFinalListening"]);
                    this.NotaFinalOral = Convert.ToDecimal(reader["NotaFinalOral"]);
                    this.NotaFinalWritting = Convert.ToDecimal(reader["NotaFinalWritting"]);
                    this.Anulado = Convert.ToBoolean(reader["Anulado"]);
                    this.InternalAssessment = Convert.ToDecimal(reader["InternalAssessment"]);

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

        public void LeerCuotas(string strCon)
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
                string sqlGrupo = "INSERT INTO ExamenEstudiante VALUES (@ID, @ExamenID, @GrupoID, @EstudianteID, @FechaInscripcion, @NotaFinal, @NotaFinalLetra, @Aprobado, @CantCuotas, @FormaPago, @Pago, @Precio, @FuncionarioID, 0, 0, 0, 0, 0, 0);";
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
            string sql = "UPDATE ExamenEstudiante SET FechaInscripcion = @FechaInscripcion, NotaFinal = @NotaFinal, NotaFinalLetra = @NotaFinalLetra, Aprobado = @Aprobado, CantCuotas = @CantCuotas, FormaPago = @FormaPago, Pago = @Pago, Precio = @Precio, FaltasEnClase = @FaltasEnClase, NotaFinalListening = @NotaFinalListening, NotaFinalOral = @NotaFinalOral, NotaFinalWritting = @NotaFinalWritting WHERE ID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID;";
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
                        string sqlCuotas = "UPDATE ExamenEstudianteCuota SET Precio = @Precio, FechaPago = @FechaPago, CuotaPaga = @CuotaPaga WHERE ID = @ID AND ExamenEstudianteID = @ExamenEstudianteID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID;";
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
                    examenEstudiante.FaltasEnClase = Convert.ToDecimal(reader["FaltasEnClase"]);
                    examenEstudiante.NotaFinalListening = Convert.ToDecimal(reader["NotaFinalListening"]);
                    examenEstudiante.NotaFinalOral = Convert.ToDecimal(reader["NotaFinalOral"]);
                    examenEstudiante.NotaFinalWritting = Convert.ToDecimal(reader["NotaFinalWritting"]);
                    examenEstudiante.Anulado = Convert.ToBoolean(reader["Anulado"]);
                    examenEstudiante.InternalAssessment = Convert.ToDecimal(reader["InternalAssessment"]);

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
            lstParametros.Add(new SqlParameter("@FaltasEnClase", this.FaltasEnClase));
            lstParametros.Add(new SqlParameter("@NotaFinalListening", this.NotaFinalListening));
            lstParametros.Add(new SqlParameter("@NotaFinalOral", this.NotaFinalOral));
            lstParametros.Add(new SqlParameter("@NotaFinalWritting", this.NotaFinalWritting));
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
                    this.FaltasEnClase = Convert.ToDecimal(reader["FaltasEnClase"]);
                    this.NotaFinalListening = Convert.ToDecimal(reader["NotaFinalListening"]);
                    this.NotaFinalOral = Convert.ToDecimal(reader["NotaFinalOral"]);
                    this.NotaFinalWritting = Convert.ToDecimal(reader["NotaFinalWritting"]);
                    this.Anulado = Convert.ToBoolean(reader["Anulado"]);
                    this.InternalAssessment = Convert.ToDecimal(reader["InternalAssessment"]);

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
                    examenEstudiante.FaltasEnClase = Convert.ToDecimal(reader["FaltasEnClase"]);
                    examenEstudiante.NotaFinalListening = Convert.ToDecimal(reader["NotaFinalListening"]);
                    examenEstudiante.NotaFinalOral = Convert.ToDecimal(reader["NotaFinalOral"]);
                    examenEstudiante.NotaFinalWritting = Convert.ToDecimal(reader["NotaFinalWritting"]);
                    examenEstudiante.Anulado = Convert.ToBoolean(reader["Anulado"]);
                    examenEstudiante.InternalAssessment = Convert.ToDecimal(reader["InternalAssessment"]);

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

        public static List<ExamenEstudiante> GetByEstudiante(Estudiante estudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<ExamenEstudiante> lstExamenes = new List<ExamenEstudiante>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@EstudianteID", estudiante.ID));
            string sql = "SELECT * FROM ExamenEstudiante WHERE EstudianteID = @EstudianteID;";
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
                    examenEstudiante.Anulado = Convert.ToBoolean(reader["Anulado"]);
                    examenEstudiante.InternalAssessment = Convert.ToDecimal(reader["InternalAssessment"]);

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

        public static List<ExamenEstudiante> GetNoPagosByEstudiante(Estudiante estudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<ExamenEstudiante> lstExamenes = new List<ExamenEstudiante>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@EstudianteID", estudiante.ID));
            string sql = "SELECT * FROM ExamenEstudiante WHERE EstudianteID = @EstudianteID AND Pago = 0;";
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
                    examenEstudiante.Examen.Leer(strCon);
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
                    examenEstudiante.Anulado = Convert.ToBoolean(reader["Anulado"]);
                    examenEstudiante.InternalAssessment = Convert.ToDecimal(reader["InternalAssessment"]);

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

        public static List<ExamenEstudiante> GetActualesByEstudiante(Estudiante estudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<ExamenEstudiante> lstExamenes = new List<ExamenEstudiante>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@EstudianteID", estudiante.ID));
            lstParametros.Add(new SqlParameter("@FechaHora", DateTime.Now));
            string sql = "SELECT EE.ID, EE.ExamenID, EE.GrupoID FROM Examen E, ExamenEstudiante EE WHERE E.ID = EE.ExamenID AND E.GrupoID = EE.GrupoID AND EE.EstudianteID = @EstudianteID AND E.FechaHora >= @FechaHora";
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
                    examenEstudiante.Examen.Leer(strCon);
                    examenEstudiante.Estudiante = estudiante;
                    examenEstudiante.Leer(strCon);
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

        public static List<ExamenEstudiante> GetByExamen(Examen examen, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<ExamenEstudiante> lstExamenes = new List<ExamenEstudiante>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ExamenID", examen.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", examen.GrupoID));
            string sql = "SELECT * FROM ExamenEstudiante WHERE ExamenID = @ExamenID AND GrupoID = @GrupoID;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    ExamenEstudiante examenEstudiante = new ExamenEstudiante();
                    examenEstudiante.ID = Convert.ToInt32(reader["ID"]);
                    examenEstudiante.Examen = examen;
                    examenEstudiante.Examen.ID = Convert.ToInt32(reader["ExamenID"]);
                    examenEstudiante.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    examenEstudiante.Estudiante.Leer(strCon);
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
                    examenEstudiante.Anulado = Convert.ToBoolean(reader["Anulado"]);
                    examenEstudiante.InternalAssessment = Convert.ToDecimal(reader["InternalAssessment"]);

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

        public static bool AsignarResultadoByLista(List<ExamenEstudiante> lstExamenEstudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            bool SeModifico = false;
            string sql = "";
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                foreach (ExamenEstudiante examenEstudiante in lstExamenEstudiante)
                {
                    List<SqlParameter> lstParametros = new List<SqlParameter>();
                    lstParametros.Add(new SqlParameter("@ID", examenEstudiante.ID));
                    lstParametros.Add(new SqlParameter("@ExamenID", examenEstudiante.Examen.ID));
                    lstParametros.Add(new SqlParameter("@GrupoID", examenEstudiante.Examen.GrupoID));
                    lstParametros.Add(new SqlParameter("@EstudianteID", examenEstudiante.Estudiante.ID));
                    lstParametros.Add(new SqlParameter("@NotaFinal", examenEstudiante.NotaFinal));
                    lstParametros.Add(new SqlParameter("@NotaFinalLetra", examenEstudiante.NotaFinalLetra));
                    lstParametros.Add(new SqlParameter("@Aprobado", examenEstudiante.Aprobado));
                    lstParametros.Add(new SqlParameter("@FaltasEnClase", examenEstudiante.FaltasEnClase));
                    lstParametros.Add(new SqlParameter("@NotaFinalListening", examenEstudiante.NotaFinalListening));
                    lstParametros.Add(new SqlParameter("@NotaFinalOral", examenEstudiante.NotaFinalOral));
                    lstParametros.Add(new SqlParameter("@NotaFinalWritting", examenEstudiante.NotaFinalWritting));
                    lstParametros.Add(new SqlParameter("@InternalAssessment", examenEstudiante.InternalAssessment));

                    sql = "UPDATE ExamenEstudiante SET NotaFinal = @NotaFinal, NotaFinalLetra = @NotaFinalLetra, Aprobado = @Aprobado, FaltasEnClase = @FaltasEnClase, NotaFinalListening = @NotaFinalListening, NotaFinalOral = @NotaFinalOral, NotaFinalWritting = @NotaFinalWritting, InternalAssessment = @InternalAssessment WHERE ID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID;";
                    Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                    Grupo.SetInactivo(examenEstudiante.Examen.GrupoID, con, tran);
                }

                //marco el examen como calificado
                //todos los examen estudiante tienen el mismo examen
                Examen examen = lstExamenEstudiante[0].Examen;
                string sqlExamen = "UPDATE Examen SET Calificado = 1 WHERE ID = @ExamenID AND GrupoID = @GrupoID;";
                List<SqlParameter> lstParametrosExamen = new List<SqlParameter>();
                lstParametrosExamen.Add(new SqlParameter("@ExamenID", examen.ID));
                lstParametrosExamen.Add(new SqlParameter("@GrupoID", examen.GrupoID));
                Persistencia.EjecutarNoQuery(con, sqlExamen, lstParametrosExamen, CommandType.Text, tran);

                foreach (ExamenEstudiante examenEstudianteAux in lstExamenEstudiante)
                {
                    examenEstudianteAux.Estudiante.BorrarGrupo(con, tran);
                }

                tran.Commit();
                SeModifico = true;
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

        public bool PagarCuota(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            SqlDataReader reader = null;
            bool SeModifico = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@ExamenID", this.Examen.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.Examen.GrupoID));
            lstParametros.Add(new SqlParameter("@CuotaID", this.LstCuotas[0].ID));
            lstParametros.Add(new SqlParameter("@Precio", this.LstCuotas[0].Precio));
            lstParametros.Add(new SqlParameter("@FechaPago", DateTime.Now));
            string sql = "UPDATE ExamenEstudianteCuota SET Precio = @Precio, FechaPago = @FechaPago, CuotaPaga = 1 WHERE ID = @CuotaID AND ExamenEstudianteID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID;";
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                int res = 0;
                res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                if (res > 0)
                {
                    List<SqlParameter> lstParametrosAux = new List<SqlParameter>();
                    lstParametrosAux.Add(new SqlParameter("@ID", this.ID));
                    lstParametrosAux.Add(new SqlParameter("@ExamenID", this.Examen.ID));
                    lstParametrosAux.Add(new SqlParameter("@GrupoID", this.Examen.GrupoID));
                    string sqlMaxCuota = "SELECT MAX(ID) AS IDCuota FROM ExamenEstudianteCuota WHERE ExamenEstudianteID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID";
                    reader = this.EjecutarConsulta(con, sqlMaxCuota, lstParametrosAux, CommandType.Text, tran);
                    int maxCuota = 0;
                    while (reader.Read())
                    {
                        maxCuota = Convert.ToInt32(reader["IDCuota"]);
                    }
                    reader.Close();
                    if (maxCuota == this.LstCuotas[0].ID)
                    {
                        //es la ultima cuota, por lo tanto marco al examen estudiante como pago
                        res = 0;
                        string sqlExamenEstudiante = "UPDATE ExamenEstudiante SET Pago = 1 WHERE ID = @ID AND ExamenID = @ExamenID AND GrupoID = @GrupoID AND EstudianteID = @EstudianteID";
                        List<SqlParameter> lstParametrosExamenEstudiante = new List<SqlParameter>();
                        lstParametrosExamenEstudiante.Add(new SqlParameter("@ID", this.ID));
                        lstParametrosExamenEstudiante.Add(new SqlParameter("@ExamenID", this.Examen.ID));
                        lstParametrosExamenEstudiante.Add(new SqlParameter("@GrupoID", this.Examen.GrupoID));
                        lstParametrosExamenEstudiante.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
                        res = Persistencia.EjecutarNoQuery(con, sqlExamenEstudiante, lstParametrosExamenEstudiante, CommandType.Text, tran);
                    }
                    SeModifico = true;
                    tran.Commit();
                }
                else
                {
                    tran.Rollback();
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

        public static bool Anular(List<ExamenEstudiante> lstExamenEstudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            bool SeModifico = false;
            try
            {
                con.Open();
                tran = con.BeginTransaction();
                foreach (ExamenEstudiante examenEstudiante in lstExamenEstudiante)
                {
                    List<SqlParameter> lstParametros = new List<SqlParameter>();
                    lstParametros.Add(new SqlParameter("@ID", examenEstudiante.ID));
                    string sql = "UPDATE ExamenEstudiante SET Anulado = 1 WHERE ID = @ID;";
                    int res = 0;
                    res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                    if (res > 0)
                    {
                        List<SqlParameter> lstParametrosExamenEstudianteCuota = new List<SqlParameter>();
                        lstParametrosExamenEstudianteCuota.Add(new SqlParameter("ID", examenEstudiante.ID));
                        string sqlCuotas = "DELETE * FROM ExamenEstudianteCuota WHERE CuotaPaga = 0 AND ExamenEstudianteID = @ID;";
                        Persistencia.EjecutarNoQuery(con, sqlCuotas, lstParametrosExamenEstudianteCuota, CommandType.Text, tran);

                    }
                }
                tran.Commit();
                SeModifico = true;
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

        public static bool Anular(List<ExamenEstudiante> lstExamenEstudiante, SqlConnection con, SqlTransaction tran)
        {
            bool SeModifico = false;
            try
            {
                foreach (ExamenEstudiante examenEstudiante in lstExamenEstudiante)
                {
                    List<SqlParameter> lstParametros = new List<SqlParameter>();
                    lstParametros.Add(new SqlParameter("@ID", examenEstudiante.ID));
                    string sql = "UPDATE ExamenEstudiante SET Anulado = 1 WHERE ID = @ID;";
                    int res = 0;
                    res = Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran);
                    if (res > 0)
                    {
                        List<SqlParameter> lstParametrosExamenEstudianteCuota = new List<SqlParameter>();
                        lstParametrosExamenEstudianteCuota.Add(new SqlParameter("ID", examenEstudiante.ID));
                        string sqlCuotas = "DELETE FROM ExamenEstudianteCuota WHERE CuotaPaga = 0 AND ExamenEstudianteID = @ID;";
                        Persistencia.EjecutarNoQuery(con, sqlCuotas, lstParametrosExamenEstudianteCuota, CommandType.Text, tran);

                    }
                }
                SeModifico = true;
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

        #endregion
        
    }
}
