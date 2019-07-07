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
    public class Matricula : Persistencia, IPersistencia<Matricula>
    {
        public int ID { get; set; }
        [JsonIgnore]
        public Sucursal Sucursal { get; set; } = new Sucursal();
        public int SucursalID { get; set; }
        public int Anio { get; set; }
        public DateTime FechaHora { get; set; }
        public decimal Precio { get; set; }

        //debe haber un registro por anio/sucursal de esta clase

        public static bool ValidarMatriculaInsert(Matricula matricula, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (matricula.Anio < 2000)
                {
                    errorMsg = "Año invalido \n";
                }
                if (matricula.Precio < 1)
                {
                    errorMsg += "Precio invalido \n";
                }
                if (matricula.Sucursal.ID < 1)
                {
                    errorMsg += "Debe asociar la matricula a una Sucursal \n";
                }
                if (errorMsg.Equals(String.Empty) && Matricula.ExisteByAnio(matricula.Anio, matricula.Sucursal.ID, strCon))
                {
                    errorMsg = "Ya existe matricula para el año " + matricula.Anio.ToString().Trim();
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

        public static bool ValidarMatriculaModificar(Matricula matricula, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (matricula.Anio < 2000)
                {
                    errorMsg = "Año invalido \n";
                }
                if (matricula.Precio < 1)
                {
                    errorMsg += "Precio invalido \n";
                }
                if (errorMsg.Equals(String.Empty))
                {
                    Matricula matriculaAux = new Matricula
                    {
                        ID = matricula.ID
                    };
                    matriculaAux.Leer(strCon);
                    if (matriculaAux.Anio != matricula.Anio)
                    {
                        if (Matricula.ExisteByAnio(matricula.Anio, matricula.Sucursal.ID, strCon))
                        {
                            errorMsg = "Ya existe una matricula para el año " + matricula.Anio.ToString().Trim();
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

        public static bool ExisteByAnio(int anio, int sucursalID, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (anio > 0 && sucursalID > 0)
            {
                sql = "SELECT * FROM Matricula WHERE SucursalID = @SucursalID AND Anio = @Anio";
                lstParametros.Add(new SqlParameter("@Anio", anio));
                lstParametros.Add(new SqlParameter("@SucursalID", sucursalID));
            }
            else
            {
                throw new ValidacionException("Datos insuficiente para buscar matricula");
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

        public static bool ExisteByID(int id, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (id > 0)
            {
                sql = "SELECT * FROM Matricula WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", id));
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
                sql = "SELECT * FROM Matricula WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else if (this.Anio > 0 && this.SucursalID > 0)
            {
                sql = "SELECT * FROM Matricula WHERE SucursalID = @SucursalID AND Anio = @Anio";
                lstParametros.Add(new SqlParameter("@Anio", this.Anio));
                lstParametros.Add(new SqlParameter("@SucursalID", this.Sucursal.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar la Matricula");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    this.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    this.Anio = Convert.ToInt32(reader["Anio"]);
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
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
            bool seGuardo = false;
            try
            {
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                string sql = "INSERT INTO Matricula VALUES (@SucursalID, @Anio, @FechaHora, @Precio); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE Matricula SET Anio = @Anio, FechaHora = @FechaHora, Precio = @Precio WHERE ID = @ID;";
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
            string sql = "DELETE FROM Matricula WHERE ID = @ID";
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

        public List<Matricula> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Matricula> lstMatriculas = new List<Matricula>();
            string sql = "SELECT * FROM Matricula;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Matricula matricula = new Matricula();
                    matricula.ID = Convert.ToInt32(reader["ID"]);
                    matricula.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    matricula.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    matricula.Anio = Convert.ToInt32(reader["Anio"]);
                    matricula.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
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
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@Anio", this.Anio));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            //no necesita lazy
            return this.Leer(strCon);
        }

        public List<Matricula> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        public static List<Matricula> GetAllByAnio(int anio, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Matricula> lstMatriculas = new List<Matricula>();
            string sql = "SELECT * FROM Matricula WHERE Anio = @Anio;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Anio", anio));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Matricula matricula = new Matricula();
                    matricula.ID = Convert.ToInt32(reader["ID"]);
                    matricula.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    matricula.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    matricula.Anio = Convert.ToInt32(reader["Anio"]);
                    matricula.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
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

        #endregion

    }
}
