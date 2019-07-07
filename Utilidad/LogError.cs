using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace BibliotecaBritanico.Utilidad
{
    public class LogError : Persistencia, IPersistencia<LogError>
    {
        public int ID { get; set; }
        public DateTime FechaHora { get; set; }
        public string Location { get; set; }
        public string Msg { get; set; }
        public LogErrorTipo Tipo { get; set; }


        public LogError() { }

        public LogError(string Location, string Msg, LogErrorTipo Tipo)
        {
            this.FechaHora = DateTime.Now;
            this.Location = Location;
            this.Msg = Msg;
            this.Tipo = Tipo;
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
                sql = "SELECT * FROM LogError WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar el Log");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"].ToString());
                    this.Location = reader["Location"].ToString();
                    this.Msg = reader["Msg"].ToString();
                    this.Tipo = (LogErrorTipo)Convert.ToInt32(reader["Tipo"]);
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
                string sql = "INSERT INTO LogError VALUES (@FechaHora, @Location, @Msg, @Tipo); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE LogError SET FechaHora = @FechaHora, Location = @Location, Msg = @Msg, Tipo = @Tipo WHERE ID = @ID;";
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
            string sql = "DELETE FROM LogError WHERE ID = @ID";
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

        public bool EliminarEntreFechas(string strCon, DateTime desde, DateTime hasta)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Desde", desde));
            lstParametros.Add(new SqlParameter("@Hasta", hasta));
            string sql = "DELETE FROM LogError WHERE FechaHora >= @Desde AND FechaHora <= @Hasta";
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

        public List<LogError> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<LogError> lstLogError = new List<LogError>();
            string sql = "SELECT * FROM LogError;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    LogError log = new LogError();
                    log.ID = Convert.ToInt32(reader["ID"]);
                    log.FechaHora = Convert.ToDateTime(reader["FechaHora"].ToString());
                    log.Location = reader["Location"].ToString();
                    log.Msg = reader["Msg"].ToString();
                    log.Tipo = (LogErrorTipo)Convert.ToInt32(reader["Tipo"]);
                    lstLogError.Add(log);
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
            return lstLogError;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@Location", this.Location));
            lstParametros.Add(new SqlParameter("@Msg", this.Msg));
            lstParametros.Add(new SqlParameter("@Tipo", this.Tipo));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            //no necesita lazy
            return this.Leer(strCon);
        }

        public List<LogError> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        #endregion

    }
}
