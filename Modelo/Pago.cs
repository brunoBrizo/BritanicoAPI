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
    public class Pago : Persistencia, IPersistencia<Pago>
    {
        public int ID { get; set; }
        [JsonIgnore]
        public Sucursal Sucursal { get; set; }
        public int SucursalID { get; set; }
        public DateTime FechaHora { get; set; }
        public TipoPago Concepto { get; set; }
        public decimal Monto { get; set; }
        [JsonIgnore]
        public Funcionario Funcionario { get; set; }
        public int FuncionarioID { get; set; }
        public string Observacion { get; set; }


        public Pago()
        {
            this.Sucursal = new Sucursal();
            this.Funcionario = new Funcionario();
        }

        public static bool ValidarPago(Pago pago)
        {
            string errorMsg = String.Empty;
            if (pago.Monto < 1)
            {
                errorMsg += "Debe ingresar el Monto del pago";
            }
            if (pago.Funcionario.ID < 1)
            {
                errorMsg += "Debe asociar el pago a un funcionario";
            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ValidarPagoModificar(Pago pago, string strCon)
        {
            string errorMsg = String.Empty;
            if (pago.Concepto.Equals(String.Empty))
            {
                errorMsg = "Debe ingresar el Concepto del pago \n";
            }
            if (pago.Monto < 1)
            {
                errorMsg += "Debe ingresar el Monto del pago \n";
            }
            if (errorMsg.Equals(String.Empty) && !Pago.ExistePagoByID(pago.ID, strCon))
            {
                errorMsg = "No existe el Pago";
            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ExistePagoByID(int id, string strCon)
        {
            {
                SqlConnection con = new SqlConnection(strCon);
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";

                if (id > 0)
                {
                    sql = "SELECT * FROM Pago WHERE ID = @ID";
                    lstParametros.Add(new SqlParameter("@ID", id));
                }
                else
                {
                    throw new ValidacionException("Datos insuficientes para buscar el Pago");
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
                    sql = "SELECT * FROM Pago WHERE ID = @ID";
                    lstParametros.Add(new SqlParameter("@ID", this.ID));
                }
                else
                {
                    throw new ValidacionException("Datos insuficientes para buscar el Pago");
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
                        this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                        this.Concepto = (TipoPago)Convert.ToInt32(reader["Concepto"]);
                        this.Monto = Convert.ToDecimal(reader["Monto"]);
                        this.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                        this.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                        this.Observacion = reader["Observacion"].ToString().Trim();
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
                string sql = "INSERT INTO Pago VALUES (@SucursalID, @FechaHora, @Concepto, @Monto, @FuncionarioID, @Observacion); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE Pago SET FechaHora = @FechaHora, Concepto = @Concepto, Monto = @Monto, Observacion = @Observacion WHERE ID = @ID;";
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
            string sql = "DELETE FROM Pago WHERE ID = @ID";
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

        public List<Pago> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Pago> lstPagos = new List<Pago>();
            string sql = "SELECT * FROM Pago;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Pago pago = new Pago();
                    pago.ID = Convert.ToInt32(reader["ID"]);
                    pago.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    pago.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    pago.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    pago.Concepto = (TipoPago)Convert.ToInt32(reader["Concepto"]);
                    pago.Monto = Convert.ToDecimal(reader["Monto"]);
                    pago.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    pago.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    pago.Observacion = reader["Observacion"].ToString().Trim();
                    lstPagos.Add(pago);
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
            return lstPagos;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@Concepto", this.Concepto));
            lstParametros.Add(new SqlParameter("@Monto", this.Monto));
            lstParametros.Add(new SqlParameter("@FuncionarioID", this.FuncionarioID));
            lstParametros.Add(new SqlParameter("@Observacion", this.Observacion));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";

            if (this.ID > 0)
            {
                sql = "SELECT * FROM Pago WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar el Pago");
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
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    this.Concepto = (TipoPago)Convert.ToInt32(reader["Concepto"]);
                    this.Monto = Convert.ToDecimal(reader["Monto"]);
                    this.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.Observacion = reader["Observacion"].ToString().Trim();
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

        public List<Pago> GetAllLazy(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Pago> lstPagos = new List<Pago>();
            string sql = "SELECT * FROM Pago;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Pago pago = new Pago();
                    pago.ID = Convert.ToInt32(reader["ID"]);
                    pago.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    pago.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    pago.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    pago.Concepto = (TipoPago)Convert.ToInt32(reader["Concepto"]);
                    pago.Monto = Convert.ToDecimal(reader["Monto"]);
                    pago.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    pago.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    pago.Observacion = reader["Observacion"].ToString().Trim();
                    lstPagos.Add(pago);
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
            return lstPagos;
        }

        #endregion

    }
}
