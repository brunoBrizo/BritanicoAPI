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
    public class Examen : Persistencia, IPersistencia<Examen>
    {
        public int ID { get; set; }
        [JsonIgnore]
        public Grupo Grupo { get; set; }
        public int GrupoID { get; set; }
        public int MateriaID { get; set; }
        public DateTime FechaHora { get; set; }
        public int AnioAsociado { get; set; }
        public int NotaMinima { get; set; }
        public decimal Precio { get; set; }
        public bool Calificado { get; set; }

        //debe haber un registro por grupo-año de esta clase

        public Examen()
        {
            this.Grupo = new Grupo();
        }

        public static bool ValidarExamenInsert(Examen examen, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (examen.Grupo.ID < 1 || examen.Grupo.Materia.ID < 1)
                {
                    errorMsg = "Examen debe estar asociado a un grupo \n";
                }
                if (examen.AnioAsociado < 2000)
                {
                    errorMsg += "Año invalido \n";
                }
                if (examen.NotaMinima < 1)
                {
                    errorMsg += "Nota minima invalida \n";
                }
                if (examen.Precio < 1)
                {
                    errorMsg += "Debe ingresar precio \n";
                }
                if (errorMsg.Equals(String.Empty) && Examen.ExisteByAnio(examen, strCon))
                {
                    errorMsg = "Ya existe un examen para grupo y año seleccionados";
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

        public static bool ValidarExamenModificar(Examen examen, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (examen.Grupo.ID < 1 || examen.Grupo.Materia.ID < 1)
                {
                    errorMsg = "Examen debe estar asociado a un grupo \n";
                }
                if (examen.AnioAsociado < 2000)
                {
                    errorMsg += "Año invalido \n";
                }
                if (examen.NotaMinima < 1)
                {
                    errorMsg += "Nota minima invalida \n";
                }
                if (examen.Precio < 1)
                {
                    errorMsg += "Debe ingresar precio \n";
                }
                if (errorMsg.Equals(String.Empty))
                {
                    Examen examenAux = new Examen
                    {
                        ID = examen.ID,
                        Grupo = examen.Grupo,
                        GrupoID = examen.GrupoID
                    };
                    examenAux.LeerLazy(strCon);
                    if (examenAux.AnioAsociado != examen.AnioAsociado)
                    {
                        if (Examen.ExisteByAnio(examen, strCon))
                        {
                            throw new ValidacionException("Ya existe un examen para grupo y año seleccionado");
                        }
                    }
                }
                else
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

        public static bool ExisteByAnio(Examen examen, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (examen.AnioAsociado > 2000 && examen.Grupo.ID > 0 && examen.Grupo.Materia.ID > 0)
            {
                sql = "SELECT * FROM Examen WHERE GrupoID = @GrupoID AND MateriaID = @MateriaID AND AnioAsociado = @AnioAsociado";
                lstParametros.Add(new SqlParameter("@GrupoID", examen.Grupo.ID));
                lstParametros.Add(new SqlParameter("@MateriaID", examen.Grupo.Materia.ID));
                lstParametros.Add(new SqlParameter("@AnioAsociado", examen.AnioAsociado));
            }
            else
            {
                throw new ValidacionException("Examen debe tener valores validos de Año y Grupo");
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

        public bool ExisteExamen(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";

            if (this.ID > 0 && this.Grupo.ID > 0)
            {
                sql = "SELECT * FROM Examen WHERE ID = @ID AND GrupoID = @GrupoID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@GrupoID", this.Grupo.ID));
            }
            else if (this.Grupo.ID > 0 && this.AnioAsociado > 0)
            {
                sql = "SELECT * FROM Examen WHERE GrupoID = @GrupoID AND MateriaID = @MateriaID AND AnioAsociado = @AnioAsociado";
                lstParametros.Add(new SqlParameter("@GrupoID", this.Grupo.ID));
                lstParametros.Add(new SqlParameter("@MateriaID", this.Grupo.Materia.ID));
                lstParametros.Add(new SqlParameter("@AnioAsociado", this.AnioAsociado));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al examen");
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

            if (this.ID > 0 && this.GrupoID > 0)
            {
                sql = "SELECT * FROM Examen WHERE ID = @ID AND GrupoID = @GrupoID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            }
            else if (this.GrupoID > 0 && this.AnioAsociado > 0)
            {
                sql = "SELECT * FROM Examen WHERE GrupoID = @GrupoID AND AnioAsociado = @AnioAsociado";
                lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
                lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
                lstParametros.Add(new SqlParameter("@AnioAsociado", this.AnioAsociado));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Examen");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    this.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    //this.Grupo.Leer(strCon);
                    this.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    this.AnioAsociado = Convert.ToInt32(reader["AnioAsociado"]);
                    this.NotaMinima = Convert.ToInt32(reader["NotaMinima"]);
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.Calificado = Convert.ToBoolean(reader["Calificado"]);
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
                this.ID = 0;
                this.ID = (int)Herramientas.ObtenerNumerador("EXAMEN", strCon);
                if (this.ID > 0)
                {
                    List<SqlParameter> lstParametros = this.ObtenerParametros();
                    string sql = "INSERT INTO Examen VALUES (@ID, @GrupoID, @MateriaID, @FechaHora, @AnioAsociado, @NotaMinima, @Precio, @Calificado);";
                    int ret = 0;
                    ret = Convert.ToInt32(Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, null));
                    if (ret > 0)
                    {
                        seGuardo = true;
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
            return seGuardo;
        }

        public bool Modificar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool SeModifico = false;
            List<SqlParameter> lstParametros = this.ObtenerParametros();
            string sql = "UPDATE Examen SET FechaHora = @FechaHora, AnioAsociado = @AnioAsociado, NotaMinima = @NotaMinima, Precio = @Precio, Calificado = @Calificado WHERE ID = @ID AND GrupoID = @GrupoID;";
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
            SqlConnection con = new SqlConnection(strCon);
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.Grupo.ID));
            string sql = "DELETE FROM Examen WHERE ID = @ID AND GrupoID = @GrupoID";
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

        public List<Examen> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Examen> lstExamenes = new List<Examen>();
            string sql = "SELECT * FROM Examen;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Examen examen = new Examen();
                    examen.ID = Convert.ToInt32(reader["ID"]);
                    examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examen.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    examen.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    examen.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    examen.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    examen.AnioAsociado = Convert.ToInt32(reader["AnioAsociado"]);
                    examen.NotaMinima = Convert.ToInt32(reader["NotaMinima"]);
                    examen.Precio = Convert.ToDecimal(reader["Precio"]);
                    examen.Calificado = Convert.ToBoolean(reader["Calificado"]);
                    lstExamenes.Add(examen);
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
            lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@AnioAsociado", this.AnioAsociado));
            lstParametros.Add(new SqlParameter("@NotaMinima", this.NotaMinima));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            lstParametros.Add(new SqlParameter("@Calificado", this.Calificado));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public List<Examen> GetAllLazy(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Examen> lstExamenes = new List<Examen>();
            string sql = "SELECT * FROM Examen;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Examen examen = new Examen();
                    examen.ID = Convert.ToInt32(reader["ID"]);
                    examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examen.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    examen.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    examen.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    examen.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    examen.AnioAsociado = Convert.ToInt32(reader["AnioAsociado"]);
                    examen.NotaMinima = Convert.ToInt32(reader["NotaMinima"]);
                    examen.Precio = Convert.ToDecimal(reader["Precio"]);
                    examen.Calificado = Convert.ToBoolean(reader["Calificado"]);
                    lstExamenes.Add(examen);
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

        public List<Examen> GetByGrupo(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Examen> lstExamenes = new List<Examen>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            string sql = "SELECT * FROM Examen WHERE GrupoID = @GrupoID AND MateriaID = @MateriaID;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Examen examen = new Examen();
                    examen.ID = Convert.ToInt32(reader["ID"]);
                    examen.Grupo = this.Grupo;
                    examen.GrupoID = this.Grupo.ID;
                    examen.MateriaID = this.Grupo.Materia.ID;
                    examen.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    examen.AnioAsociado = Convert.ToInt32(reader["AnioAsociado"]);
                    examen.NotaMinima = Convert.ToInt32(reader["NotaMinima"]);
                    examen.Precio = Convert.ToDecimal(reader["Precio"]);
                    examen.Calificado = Convert.ToBoolean(reader["Calificado"]);
                    lstExamenes.Add(examen);
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

        public List<Examen> GetByAnio(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Examen> lstExamenes = new List<Examen>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@AnioAsociado", this.AnioAsociado));
            string sql = "SELECT * FROM Examen WHERE AnioAsociado = @AnioAsociado;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Examen examen = new Examen();
                    examen.ID = Convert.ToInt32(reader["ID"]);
                    examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examen.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    examen.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    examen.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    examen.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    examen.AnioAsociado = Convert.ToInt32(reader["AnioAsociado"]);
                    examen.NotaMinima = Convert.ToInt32(reader["NotaMinima"]);
                    examen.Precio = Convert.ToDecimal(reader["Precio"]);
                    examen.Calificado = Convert.ToBoolean(reader["Calificado"]);
                    lstExamenes.Add(examen);
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

        public List<Estudiante> GetListaEstudiantes(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Estudiante> lstEstudiante = new List<Estudiante>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            string sql = "SELECT DISTINCT EstudianteID FROM ExamenEstudiante WHERE ExamenID = @ID;";
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
                    lstEstudiante.Add(estudiante);
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
            return lstEstudiante;
        }

        public static List<Examen> GetExamenPendientePorEstudiante(Estudiante estudiante, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            List<Examen> lstExamenes = new List<Examen>();
            SqlDataReader reader = null;
            try
            {
                //si debe un examen anterior, busco el de la misma materia pero actual
                ExamenEstudiante examenEstudiante = ExamenEstudiante.GetExamenPendientePorEstudiante(estudiante, strCon);
                if (examenEstudiante != null && examenEstudiante.ID > 0)
                {
                    Examen examen = new Examen
                    {
                        ID = examenEstudiante.Examen.ID,
                        GrupoID = examenEstudiante.Examen.GrupoID
                    };
                    examen.Leer(strCon);
                    lstParametros.Add(new SqlParameter("@MateriaID", examen.MateriaID));
                    lstParametros.Add(new SqlParameter("@GrupoID", examen.GrupoID));
                }
                else
                {
                    lstParametros.Add(new SqlParameter("@MateriaID", estudiante.MateriaID));
                    lstParametros.Add(new SqlParameter("@GrupoID", estudiante.GrupoID));
                }
                lstParametros.Add(new SqlParameter("@AnioAsociado", DateTime.Now.Year));
                lstParametros.Add(new SqlParameter("@FechaHora", DateTime.Now));
                string sqlExamenID = "SELECT E.ID, E.GrupoID FROM EXAMEN E, MATERIA M WHERE E.MATERIAID = M.ID AND E.ANIOASOCIADO = @AnioAsociado AND E.FechaHora >= @FechaHora AND M.ID = @MateriaID;";

                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sqlExamenID, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Examen examen = new Examen();
                    examen.ID = Convert.ToInt32(reader["ID"]);
                    examen.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    examen.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    examen.Leer(strCon);
                    lstExamenes.Add(examen);
                }
                if (lstExamenes.Count > 0)
                {
                    Examen examen = null;
                    bool encuentroExamen = false;
                    foreach (Examen examenAux in lstExamenes)
                    {
                        if (examenAux.GrupoID == estudiante.GrupoID)
                        {
                            examen = examenAux;
                            encuentroExamen = true;
                            break;
                        }
                    }
                    if (encuentroExamen)
                    {
                        lstExamenes.Clear();
                        ExamenEstudiante examenEstudianteAux = new ExamenEstudiante
                        {
                            ID = 0,
                            Estudiante = estudiante,
                            Examen = examen
                        };
                        if (!ExamenEstudiante.ExisteExamenEstudiante(examenEstudianteAux, strCon))
                        {
                            lstExamenes.Add(examen);
                        }                        
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
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                con.Close();
            }
            return lstExamenes;
        }


        #endregion


    }
}
