using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace BibliotecaBritanico.Utilidad
{
    public abstract class Persistencia
    {

        #region Metodos

        public virtual List<SqlParameter> ObtenerParametros()
        {
            return new List<SqlParameter>();
        }

        protected static Object EjecutarScalar(SqlConnection con, string sql, CommandType tipo, List<SqlParameter> listaParametros, SqlTransaction tran)
        {
            object resultado = "";
            bool cerrarCon = false;
            try
            {
                SqlCommand comando = new SqlCommand(sql, con);
                comando.CommandType = tipo;
                comando.Parameters.AddRange(listaParametros.ToArray());
                if (con.State != ConnectionState.Open)
                {
                    cerrarCon = true;
                    con.Open();
                }
                if (tran != null)
                {
                    comando.Transaction = tran;
                }
                resultado = comando.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (cerrarCon) con.Close();
            }
            return resultado;
        }

        protected static int EjecutarNoQuery(SqlConnection con, string sql, List<SqlParameter> listaParametros, CommandType tipo, SqlTransaction tran)
        {
            bool cerrarCon = false;
            int resultado = 0;
            SqlCommand comando = new SqlCommand();
            try
            {
                comando.CommandText = sql;
                comando.Connection = con;
                comando.CommandType = tipo;
                comando.Parameters.Clear();
                comando.Parameters.AddRange(listaParametros.ToArray());
                if (con.State != ConnectionState.Open)
                {
                    cerrarCon = true;
                    con.Open();
                }
                if (tran != null)
                {
                    comando.Transaction = tran;
                }
                resultado = comando.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (cerrarCon) con.Close();
            }
            return resultado;
        }

        protected static SqlDataReader EjecutarConsulta(SqlConnection con, string sql, List<SqlParameter> listaParametros, CommandType tipo)
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
