using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using BibliotecaBritanico.Utilidad;

namespace BibliotecaBritanico.Modelo
{
    public class Sucursal : Persistencia, IPersistencia<Sucursal>
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Tel { get; set; }
        public string Ciudad { get; set; }
        public string Encargado { get; set; }


        public Sucursal()
        {
        }

        public Sucursal(string Nombre, string Direccion, string Tel, string Email, string Ciudad, string Encargado)
        {
            this.Nombre = Nombre;
            this.Direccion = Direccion;
            this.Tel = Tel;
            this.Email = Email;
            this.Ciudad = Ciudad;
            this.Encargado = Encargado;
        }

        public static bool ValidarSucursal(Sucursal sucursal)
        {
            string errorMsg = "";
            if (sucursal.Nombre.Equals(String.Empty))
            {
                errorMsg = "Debe ingresar nombre de la sucursal \n";
            }
            if (!sucursal.Email.Equals(String.Empty) && !Herramientas.ValidarMail(sucursal.Email))
            {
                errorMsg += "Email inválido";
            }
            if (errorMsg != "")
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
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
                sql = "SELECT * FROM Sucursal WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar la Sucursal");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.Nombre = reader["Nombre"].ToString();
                    this.Direccion = reader["Direccion"].ToString();
                    this.Tel = reader["Tel"].ToString();
                    this.Email = reader["Email"].ToString();
                    this.Ciudad = reader["Ciudad"].ToString();
                    this.Encargado = reader["Encargado"].ToString();
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
                string sql = "INSERT INTO Sucursal VALUES (@Nombre, @Email, @Direccion, @Tel, @Ciudad, @Encargado); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE Sucursal SET Nombre = @Nombre, Direccion = @Direccion, Tel = @Tel, Email = @Email, Ciudad = @Ciudad, Encargado = @Encargado WHERE ID = @ID;";
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

        //Sucursal no se borra nunca
        public bool Eliminar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            string sql = "DELETE FROM Sucursal WHERE ID = @ID";
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

        public List<Sucursal> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Sucursal> lstSucursales = new List<Sucursal>();
            string sql = "SELECT * FROM Sucursal;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Sucursal suc = new Sucursal();
                    suc.ID = Convert.ToInt32(reader["ID"]);
                    suc.Nombre = reader["Nombre"].ToString();
                    suc.Direccion = reader["Direccion"].ToString();
                    suc.Tel = reader["Tel"].ToString();
                    suc.Email = reader["Email"].ToString();
                    suc.Ciudad = reader["Ciudad"].ToString();
                    suc.Encargado = reader["Encargado"].ToString();
                    lstSucursales.Add(suc);
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
            return lstSucursales;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            lstParametros.Add(new SqlParameter("@Direccion", this.Direccion));
            lstParametros.Add(new SqlParameter("@Tel", this.Tel));
            lstParametros.Add(new SqlParameter("@Email", this.Email));
            lstParametros.Add(new SqlParameter("@Ciudad", this.Ciudad));
            lstParametros.Add(new SqlParameter("@Encargado", this.Encargado));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            //no necesita lazy
            return this.Leer(strCon);
        }

        public List<Sucursal> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        #endregion
    }
}
