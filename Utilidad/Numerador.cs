using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace BibliotecaBritanico.Utilidad
{
    public class Numerador : Persistencia, IPersistencia<Numerador>
    {
        public string Tipo { get; set; }
        public decimal Valor { get; set; }


        public Numerador()
        {
        }

        public Numerador(string Tipo, decimal Valor)
        {
            this.Tipo = Tipo;
            this.Valor = Valor;
        }



        #region Persistencia

        public bool Leer(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (this.Tipo != String.Empty)
            {
                sql = "SELECT * FROM Numerador WHERE Tipo = @Tipo";
                lstParametros.Add(new SqlParameter("@Tipo", this.Tipo));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar el Numerador");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.Valor = Convert.ToDecimal(reader["Valor"]);
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
                if (!this.Leer(strCon))
                {
                    List<SqlParameter> lstParametros = new List<SqlParameter>();
                    lstParametros.Add(new SqlParameter("@Tipo", this.Tipo));
                    lstParametros.Add(new SqlParameter("@Valor", this.Valor));
                    string sql = "INSERT INTO Numerador VALUES (@Tipo, @Valor);";
                    int res = 0;
                    res = Convert.ToInt32(Persistencia.EjecutarScalar(con, sql, CommandType.Text, lstParametros, null));
                    if (res > 0) seGuardo = true;
                }
                else
                {
                    throw new ValidacionException("Ya existe el Numerador: " + this.Tipo, "Numerador");
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
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Tipo", this.Tipo));
            lstParametros.Add(new SqlParameter("@Valor", this.Valor));
            string sql = "UPDATE Numerador SET Valor = @Valor WHERE Tipo = @Tipo;";
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
            lstParametros.Add(new SqlParameter("@Tipo", this.Tipo));
            string sql = "DELETE FROM Numerador WHERE Tipo = @Tipo";
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

        public List<Numerador> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Numerador> lstNumeradores = new List<Numerador>();
            string sql = "SELECT * FROM Numerador;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Numerador num = new Numerador();
                    num.Tipo = reader["Tipo"].ToString();
                    num.Valor = Convert.ToDecimal(reader["Valor"]);
                    lstNumeradores.Add(num);
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
            return lstNumeradores;
        }

        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public List<Numerador> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        #endregion
    }
}
