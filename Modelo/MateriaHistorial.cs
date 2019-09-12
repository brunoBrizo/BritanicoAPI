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
    public class MateriaHistorial : Persistencia, IPersistencia<MateriaHistorial>
    {
        public int ID { get; set; }
        public int MateriaID { get; set; }
        public int SucursalID { get; set; }
        public int Anio { get; set; }
        public decimal ExamenPrecio { get; set; }
        public decimal MensualidadPrecio { get; set; }
        public int CantidadGrupos { get; set; }
        public int CantidadAlumnos { get; set; }
        public DateTime FechaHora { get; set; }




        #region Persistencia

        public static bool ExisteMateriaHistorial(MateriaHistorial materiaHistorial, string strCon)
        {
            {
                SqlConnection con = new SqlConnection(strCon);
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";
                if (materiaHistorial.ID > 0)
                {
                    sql = "SELECT * FROM MateriaHistorial WHERE ID = @ID";
                    lstParametros.Add(new SqlParameter("@ID", materiaHistorial.ID));
                }
                else if (materiaHistorial.MateriaID > 0 && materiaHistorial.Anio > 2000)
                {
                    sql = "SELECT * FROM MateriaHistorial WHERE MateriaID = @MateriaID AND Anio = @Anio";
                    lstParametros.Add(new SqlParameter("@MateriaID", materiaHistorial.MateriaID));
                    lstParametros.Add(new SqlParameter("@Anio", materiaHistorial.Anio));
                    if (materiaHistorial.SucursalID > 0)
                    {
                        sql = "SELECT * FROM MateriaHistorial WHERE MateriaID = @MateriaID AND Anio = @Anio AND SucursalID = @SucursalID";
                        lstParametros.Add(new SqlParameter("@SucursalID", materiaHistorial.SucursalID));
                    }
                }
                else
                {
                    throw new ValidacionException("Datos insuficientes para buscar el historial de la Materia");
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

        public static bool ExisteMateriaHistorial(MateriaHistorial materiaHistorial, SqlConnection con, SqlTransaction tran)
        {
            {
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";
                if (materiaHistorial.ID > 0)
                {
                    sql = "SELECT * FROM MateriaHistorial WHERE ID = @ID";
                    lstParametros.Add(new SqlParameter("@ID", materiaHistorial.ID));
                }
                else if (materiaHistorial.MateriaID > 0 && materiaHistorial.Anio > 2000 && materiaHistorial.SucursalID > 0)
                {
                    sql = "SELECT * FROM MateriaHistorial WHERE MateriaID = @MateriaID AND Anio = @Anio AND SucursalID = @SucursalID";
                    lstParametros.Add(new SqlParameter("@MateriaID", materiaHistorial.MateriaID));
                    lstParametros.Add(new SqlParameter("@SucursalID", materiaHistorial.SucursalID));
                    lstParametros.Add(new SqlParameter("@Anio", materiaHistorial.Anio));
                }
                else
                {
                    throw new ValidacionException("Datos insuficientes para buscar el historial de la Materia");
                }
                try
                {
                    reader = MateriaHistorial.EjecutarConsulta(con, sql, lstParametros, CommandType.Text, tran);
                    while (reader.Read())
                    {
                        ok = true;
                        break;
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
                return ok;
            }
        }

        public bool Leer(string strCon)
        {
            {
                SqlConnection con = new SqlConnection(strCon);
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";

                if (this.ID > 0)
                {
                    sql = "SELECT * FROM MateriaHistorial WHERE ID = @ID";
                    lstParametros.Add(new SqlParameter("@ID", this.ID));
                }
                else if (this.MateriaID > 0 && this.Anio > 2000 && this.SucursalID > 0)
                {
                    sql = "SELECT * FROM MateriaHistorial WHERE MateriaID = @MateriaID AND Anio = @Anio AND SucursalID = @SucursalID";
                    lstParametros.Add(new SqlParameter("@Nombre", this.MateriaID));
                    lstParametros.Add(new SqlParameter("@Nombre", this.SucursalID));
                    lstParametros.Add(new SqlParameter("@Nombre", this.Anio));
                }
                else
                {
                    throw new ValidacionException("Datos insuficientes para buscar el historial de la Materia");
                }
                try
                {
                    con.Open();
                    reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                    while (reader.Read())
                    {
                        this.ID = Convert.ToInt32(reader["ID"]);
                        this.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                        this.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                        this.Anio = Convert.ToInt32(reader["Anio"]);
                        this.ExamenPrecio = Convert.ToDecimal(reader["ExamenPrecio"]);
                        this.MensualidadPrecio = Convert.ToDecimal(reader["MensualidadPrecio"]);
                        this.CantidadAlumnos = Convert.ToInt32(reader["CantidadAlumnos"]);
                        this.CantidadGrupos = Convert.ToInt32(reader["CantidadGrupos"]);
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
        }

        public bool Guardar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seGuardo = false;
            try
            {
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                string sql = "INSERT INTO MateriaHistorial VALUES (@MateriaID, @SucursalID, @Anio, @ExamenPrecio, @MensualidadPrecio, @CantidadGrupos, @CantidadAlumnos); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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

        public bool GuardarTransaccional(SqlConnection con, SqlTransaction tran)
        {
            bool seGuardo = false;
            try
            {
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                string sql = "INSERT INTO MateriaHistorial VALUES (@MateriaID, @SucursalID, @Anio, @ExamenPrecio, @MensualidadPrecio, @CantidadGrupos, @CantidadAlumnos); SELECT CAST (SCOPE_IDENTITY() AS INT);";
                this.ID = 0;
                this.ID = Convert.ToInt32(Persistencia.EjecutarScalar(con, sql, CommandType.Text, lstParametros, tran));
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
            string sql = "UPDATE MateriaHistorial SET ExamenPrecio = @ExamenPrecio, MensualidadPrecio = @MensualidadPrecio, CantidadAlumnos = @CantidadAlumnos, CantidadGrupos = @CantidadGrupos  WHERE ID = @ID;";
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

        public bool ModificarExamenPrecio(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool SeModifico = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ExamenPrecio", this.ExamenPrecio));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@Anio", this.Anio));
            string sql = "UPDATE MateriaHistorial SET ExamenPrecio = @ExamenPrecio  WHERE MateriaID = @MateriaID AND Anio = @Anio;";
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

        public bool ModificarCantidadGrupos(SqlConnection con, SqlTransaction tran)
        {
            bool SeModifico = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@Anio", this.Anio));
            string sql = "UPDATE MateriaHistorial SET CantidadGrupos = 1 + (SELECT CantidadGrupos FROM MateriaHistorial WHERE MateriaID = @MateriaID AND Anio = @Anio AND SucursalID = @SucursalID) WHERE MateriaID = @MateriaID AND Anio = @Anio AND SucursalID = @SucursalID;";
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

        public bool ModificarCantidadAlumnos(SqlConnection con, SqlTransaction tran, int grupoID)
        {
            bool SeModifico = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@GrupoID", grupoID));
            string sql = "UPDATE MateriaHistorial SET CantidadAlumnos = 1 + (SELECT CantidadAlumnos FROM MateriaHistorial WHERE MateriaID = @MateriaID AND Anio = (SELECT Anio FROM Grupo WHERE ID = @GrupoID and MateriaID = @MateriaID) AND SucursalID = @SucursalID)  WHERE MateriaID = @MateriaID AND Anio = (SELECT Anio FROM Grupo WHERE ID = @GrupoID and MateriaID = @MateriaID) AND SucursalID = @SucursalID;";
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

        public bool Eliminar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            string sql = "DELETE FROM MateriaHistorial WHERE ID = @ID";
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

        public List<MateriaHistorial> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<MateriaHistorial> lstMateriaHistorial = new List<MateriaHistorial>();
            string sql = "SELECT * FROM MateriaHistorial;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    MateriaHistorial materiaHistorial = new MateriaHistorial();
                    materiaHistorial.ID = Convert.ToInt32(reader["ID"]);
                    materiaHistorial.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    materiaHistorial.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    materiaHistorial.Anio = Convert.ToInt32(reader["Anio"]);
                    materiaHistorial.ExamenPrecio = Convert.ToDecimal(reader["ExamenPrecio"]);
                    materiaHistorial.MensualidadPrecio = Convert.ToDecimal(reader["MensualidadPrecio"]);
                    materiaHistorial.CantidadAlumnos = Convert.ToInt32(reader["CantidadAlumnos"]);
                    materiaHistorial.CantidadGrupos = Convert.ToInt32(reader["CantidadGrupos"]);
                    lstMateriaHistorial.Add(materiaHistorial);
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
            return lstMateriaHistorial;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@Anio", this.Anio));
            lstParametros.Add(new SqlParameter("@ExamenPrecio", this.ExamenPrecio));
            lstParametros.Add(new SqlParameter("@MensualidadPrecio", this.MensualidadPrecio));
            lstParametros.Add(new SqlParameter("@CantidadAlumnos", this.CantidadAlumnos));
            lstParametros.Add(new SqlParameter("@CantidadGrupos", this.CantidadGrupos));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public List<MateriaHistorial> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        public static List<MateriaHistorial> GetAllByMateria(Materia materia, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<MateriaHistorial> lstMateriaHistorial = new List<MateriaHistorial>();
            string sql = "SELECT * FROM MateriaHistorial WHERE MateriaID = @MateriaID;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@MateriaID", materia.ID));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    MateriaHistorial materiaHistorial = new MateriaHistorial();
                    materiaHistorial.ID = Convert.ToInt32(reader["ID"]);
                    materiaHistorial.MateriaID = materia.ID;
                    materiaHistorial.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    materiaHistorial.Anio = Convert.ToInt32(reader["Anio"]);
                    materiaHistorial.ExamenPrecio = Convert.ToDecimal(reader["ExamenPrecio"]);
                    materiaHistorial.MensualidadPrecio = Convert.ToDecimal(reader["MensualidadPrecio"]);
                    materiaHistorial.CantidadAlumnos = Convert.ToInt32(reader["CantidadAlumnos"]);
                    materiaHistorial.CantidadGrupos = Convert.ToInt32(reader["CantidadGrupos"]);
                    lstMateriaHistorial.Add(materiaHistorial);
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
            return lstMateriaHistorial;
        }

        public static List<MateriaHistorial> GetAllByMateriaAnio(Materia materia, int anio, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<MateriaHistorial> lstMateriaHistorial = new List<MateriaHistorial>();
            string sql = "SELECT * FROM MateriaHistorial WHERE MateriaID = @MateriaID AND Anio = @Anio;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@MateriaID", materia.ID));
            lstParametros.Add(new SqlParameter("@Anio", anio));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    MateriaHistorial materiaHistorial = new MateriaHistorial();
                    materiaHistorial.ID = Convert.ToInt32(reader["ID"]);
                    materiaHistorial.MateriaID = materia.ID;
                    materiaHistorial.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    materiaHistorial.Anio = anio;
                    materiaHistorial.ExamenPrecio = Convert.ToDecimal(reader["ExamenPrecio"]);
                    materiaHistorial.MensualidadPrecio = Convert.ToDecimal(reader["MensualidadPrecio"]);
                    materiaHistorial.CantidadAlumnos = Convert.ToInt32(reader["CantidadAlumnos"]);
                    materiaHistorial.CantidadGrupos = Convert.ToInt32(reader["CantidadGrupos"]);
                    lstMateriaHistorial.Add(materiaHistorial);
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
            return lstMateriaHistorial;
        }

        #endregion

        private static SqlDataReader EjecutarConsulta(SqlConnection con, string sql, List<SqlParameter> listaParametros, CommandType tipo, SqlTransaction tran)
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
