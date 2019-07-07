using BibliotecaBritanico.Utilidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaBritanico.Modelo
{
    public class Empresa : Persistencia, IPersistencia<Empresa>
    {
        public int ID { get; set; }
        public string Rut { get; set; }
        public string RazonSoc { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Tel { get; set; }
        public string Logo { get; set; }
        public string LogoImagen { get; set; }


        public Empresa() { }

        public Empresa(string Rut, string RazonSoc, string Nombre, string Direccion, string Tel, string Email, string Logo, string LogoImagen)
        {
            this.Rut = Rut;
            this.RazonSoc = RazonSoc;
            this.Nombre = Nombre;
            this.Direccion = Direccion;
            this.Tel = Tel;
            this.Email = Email;
            this.Logo = Logo;
            this.LogoImagen = LogoImagen;
        }

        public static bool ValidarEmpresaInsert(Empresa empresa, string strCon)
        {
            string errorMsg = String.Empty;
            if (empresa.Rut.Equals(String.Empty) || empresa.RazonSoc.Equals(String.Empty) || empresa.Nombre.Equals(String.Empty))
            {
                errorMsg = "Rut, Razon Social y Nombre son obligatorios \n";
            }
            if (!empresa.Rut.Equals(String.Empty) && !Herramientas.ValidarRUT(empresa.Rut))
            {
                errorMsg += "RUT inválido \n";
            }
            if (!empresa.Email.Equals(String.Empty) && !Herramientas.ValidarMail(empresa.Email))
            {
                errorMsg += "Email inválido \n";
            }
            if (errorMsg.Equals(String.Empty) && !empresa.Rut.Equals(String.Empty) && Empresa.ExisteRUT(empresa.Rut, strCon))
            {
                errorMsg += "Ya existe una empresa con el RUT: " + empresa.Rut + " \n";
            }
            if (errorMsg != String.Empty)
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ValidarEmpresaModificar(Empresa empresa, string strCon)
        {
            string ErrorMsg = String.Empty;
            if (empresa.RazonSoc.Equals(String.Empty) || empresa.Nombre.Equals(String.Empty))
            {
                ErrorMsg = "Razon Social y Nombre son obligatorios \n";
            }
            if (!empresa.Email.Equals(String.Empty) && !Herramientas.ValidarMail(empresa.Email))
            {
                ErrorMsg += "Email inválido \n";
            }
            if (ErrorMsg != String.Empty)
            {
                throw new ValidacionException(ErrorMsg);
            }
            return true;
        }

        public static bool ExisteRUT(string Rut, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            lstParametros.Add(new SqlParameter("@Rut", Rut));
            string sql = "SELECT * FROM Empresa WHERE Rut = @Rut";
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
                sql = "SELECT * FROM Empresa WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else if (!this.Rut.Equals(String.Empty))
            {
                sql = "SELECT * FROM Empresa WHERE Rut = @Rut";
                lstParametros.Add(new SqlParameter("@Rut", this.Rut));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar a la Empresa");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.Rut = reader["Rut"].ToString();
                    this.RazonSoc = reader["RazonSoc"].ToString();
                    this.Nombre = reader["Nombre"].ToString();
                    this.Direccion = reader["Direccion"].ToString();
                    this.Tel = reader["Tel"].ToString();
                    this.Email = reader["Email"].ToString();
                    this.Logo = reader["Logo"].ToString();
                    this.LogoImagen = reader["LogoImagen"].ToString();
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

        //usar ValidarEmpresaInsert antes de Guardar
        public bool Guardar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seGuardo = false;
            try
            {
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                string sql = "INSERT INTO Empresa VALUES (@Rut, @RazonSoc, @Nombre, @Email, @Direccion, @Tel, @Logo, @LogoImagen); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE Empresa SET RazonSoc = @RazonSoc, Nombre = @Nombre, Direccion = @Direccion, Tel = @Tel, Email = @Email, Logo = @Logo, LogoImagen = @LogoImagen WHERE ID = @ID;";
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

        //Empresa no se borra nunca
        public bool Eliminar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            string sql = "DELETE FROM Empresa WHERE ID = @ID";
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

        public List<Empresa> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Empresa> lstEmpresas = new List<Empresa>();
            string sql = "SELECT * FROM Empresa;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Empresa emp = new Empresa();
                    emp.ID = Convert.ToInt32(reader["ID"]);
                    emp.Rut = reader["Rut"].ToString();
                    emp.RazonSoc = reader["RazonSoc"].ToString();
                    emp.Nombre = reader["Nombre"].ToString();
                    emp.Direccion = reader["Direccion"].ToString();
                    emp.Tel = reader["Tel"].ToString();
                    emp.Email = reader["Email"].ToString();
                    emp.Logo = reader["Logo"].ToString();
                    emp.LogoImagen = reader["LogoImagen"].ToString();
                    lstEmpresas.Add(emp);
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
            return lstEmpresas;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@Rut", this.Rut));
            lstParametros.Add(new SqlParameter("@RazonSoc", this.RazonSoc));
            lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            lstParametros.Add(new SqlParameter("@Direccion", this.Direccion));
            lstParametros.Add(new SqlParameter("@Tel", this.Tel));
            lstParametros.Add(new SqlParameter("@Email", this.Email));
            lstParametros.Add(new SqlParameter("@Logo", this.Logo));
            lstParametros.Add(new SqlParameter("@LogoImagen", this.LogoImagen));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            //no necesita lazy
            return this.Leer(strCon);
        }

        public List<Empresa> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        #endregion
    }
}
