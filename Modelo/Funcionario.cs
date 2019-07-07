using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaBritanico.Utilidad;
using System.Data.SqlClient;
using System.Data;
using DBConnection;
using Newtonsoft.Json;

namespace BibliotecaBritanico.Modelo
{
    public class Funcionario : Persistencia, IPersistencia<Funcionario>
    {
        public int ID { get; set; }
        [JsonIgnore]
        public Sucursal Sucursal { get; set; } = new Sucursal();
        public int SucursalID { get; set; }
        public string CI { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string TelefonoAux { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaNac { get; set; }
        public string Clave { get; set; }
        public bool Activo { get; set; }
        public FuncionarioTipo TipoFuncionario { get; set; }


        public Funcionario() { }

        public Funcionario(Sucursal Sucursal, string CI, string Nombre, string Email, string Clave, string Telefono, string Direccion, string TelefonoAux, DateTime FechaNac, bool Activo)
        {
            this.Sucursal = Sucursal;
            this.SucursalID = Sucursal.ID;
            this.CI = CI;
            this.Nombre = Nombre;
            this.Email = Email;
            this.Clave = Clave;
            this.Telefono = Telefono;
            this.Direccion = Direccion;
            this.TelefonoAux = TelefonoAux;
            this.FechaNac = FechaNac;
            this.Activo = Activo;
        }

        public static bool ValidarFuncionarioInsert(Funcionario funcionario, string strCon)
        {
            string errorMsg = String.Empty;
            if (funcionario.CI.Equals(String.Empty) || funcionario.Nombre.Equals(String.Empty) || funcionario.Clave.Equals(String.Empty) || funcionario.Telefono.Equals(String.Empty))
            {
                errorMsg = "Nombre, cedula, clave y telefono son obligatorios \n";
            }
            if (funcionario.FechaNac >= DateTime.Today || funcionario.FechaNac <= DateTime.MinValue)
            {
                errorMsg += "Fecha de nacimiento invalida \n";
            }
            if (!funcionario.CI.Equals(String.Empty) && !Herramientas.ValidarCedula(funcionario.CI))
            {
                errorMsg += "Cedula invalida \n";
            }
            if (!funcionario.Email.Equals(String.Empty) && !Herramientas.ValidarMail(funcionario.Email))
            {
                errorMsg += "Email inválido \n";
            }
            if (!Herramientas.ValidarPassword(funcionario.Clave))
            {
                errorMsg += "La contraseña debe tener más de 5 caracteres  \n";
            }
            if (errorMsg.Equals(String.Empty) && Funcionario.ExisteFuncionarioByCedula(funcionario.CI, strCon))
            {
                errorMsg += "Ya existe un funcionairo con la cedula: " + funcionario.CI.ToString().Trim() + " \n";
            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg, "Funcionario");
            }
            return true;
        }

        public static bool ValidarFuncionarioModificar(Funcionario funcionario, string strCon)
        {
            string errorMsg = String.Empty;
            if (funcionario.CI.Equals(String.Empty) || funcionario.Nombre.Equals(String.Empty) || funcionario.Clave.Equals(String.Empty) || funcionario.Telefono.Equals(String.Empty))
            {
                errorMsg = "Nombre, cedula, clave y telefono son obligatorios \n";
            }
            if (funcionario.FechaNac >= DateTime.Today || funcionario.FechaNac <= DateTime.MinValue)
            {
                errorMsg += "Fecha de nacimiento invalida \n";
            }
            if (!funcionario.Email.Equals(String.Empty) && !Herramientas.ValidarMail(funcionario.Email))
            {
                errorMsg += "Email inválido \n";
            }
            if (!Herramientas.ValidarPassword(funcionario.Clave))
            {
                errorMsg += "La contraseña debe tener más de 5 caracteres  \n";
            }
            if (errorMsg.Equals(String.Empty))
            {
                if (!Funcionario.ExisteFuncionarioByID(funcionario.ID, strCon))
                    throw new ValidacionException("No existe el funcionario con la Cedula");

            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg, "Funcionario");
            }
            return true;
        }

        public static bool ExisteFuncionarioByCedula(string CI, string strCon)
        {
            if (!CI.Equals(String.Empty) && !strCon.Equals(String.Empty))
            {
                SqlConnection con = new SqlConnection(strCon);
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "SELECT * FROM Funcionario WHERE CI = @CI";
                lstParametros.Add(new SqlParameter("@CI", CI));
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
            return false;
        }

        public static bool ExisteFuncionarioByID(int id, string strCon)
        {
            if (id > 0 && !strCon.Equals(String.Empty))
            {
                SqlConnection con = new SqlConnection(strCon);
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "SELECT * FROM Funcionario WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", id));
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
            return false;
        }

        public static Funcionario Login(Funcionario funcionario, string strCon)
        {
            try
            {
                Funcionario funcionarioAux = new Funcionario
                {
                    CI = funcionario.CI,
                    Clave = funcionario.Clave
                };
                if (funcionarioAux.Leer(strCon))
                {
                    if (funcionarioAux.Clave.Equals(funcionario.Clave))
                    {
                        return funcionarioAux;
                    }
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

        private string EncriptarPassword(string Clave)
        {
            try
            {
                return Encriptacion.Encriptar(Clave);
            }
            catch (Exception ex)
            {
                throw new ValidacionException("Error encriptando password: " + ex.Message.Trim(), "Funcionario");
            }
        }

        private string DesencriptarPassword(string ClaveEncriptada)
        {
            try
            {
                return Encriptacion.Desencriptar(ClaveEncriptada);
            }
            catch (Exception ex)
            {
                throw new ValidacionException("Error desencriptando password: " + ex.Message.Trim(), "Funcionario");
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
            if (this.ID > 0)
            {
                sql = "SELECT * FROM Funcionario WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else if (!this.CI.Equals(String.Empty))
            {
                sql = "SELECT * FROM Funcionario WHERE CI = @CI";
                lstParametros.Add(new SqlParameter("@CI", this.CI));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Funcionario");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    if (this.Sucursal == null)
                    {
                        this.Sucursal = new Sucursal();
                    }
                    this.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    this.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.CI = reader["CI"].ToString();
                    this.Email = reader["Email"].ToString();
                    this.Nombre = reader["Nombre"].ToString();
                    this.Telefono = reader["Telefono"].ToString();
                    this.TelefonoAux = reader["TelefonoAux"].ToString();
                    this.Direccion = reader["Direccion"].ToString();
                    this.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    this.Clave = this.DesencriptarPassword(reader["Clave"].ToString());
                    this.Activo = Convert.ToBoolean(reader["Activo"]);
                    this.TipoFuncionario = (FuncionarioTipo)Convert.ToInt32(reader["TipoFuncionario"]);
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
                string claveDesencriptada = this.Clave;
                this.Clave = this.EncriptarPassword(this.Clave);
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                lstParametros.Add(new SqlParameter("@Clave", this.Clave));
                this.Clave = claveDesencriptada;
                string sql = "INSERT INTO Funcionario VALUES (@SucursalID, @CI, @Email, @Nombre, @Telefono, @TelefonoAux, @Direccion, @FechaNac, @Clave, @Activo, @TipoFuncionario); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            this.Clave = this.EncriptarPassword(this.Clave);
            List<SqlParameter> lstParametros = this.ObtenerParametros();
            lstParametros.Add(new SqlParameter("@Clave", this.Clave));
            string sql = "UPDATE Funcionario SET SucursalID = @SucursalID, Email = @Email, Nombre = @Nombre, Telefono = @Telefono, TelefonoAux = @TelefonoAux, Direccion = @Direccion, FechaNac = @FechaNac, Clave = @Clave, Activo = @Activo, TipoFuncionario = @TipoFuncionario WHERE ID = @ID;";
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
            if (this.ID > 0)
            {
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                string sql = "DELETE FROM Funcionario WHERE ID = @ID";
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
            }
            return seBorro;
        }

        public List<Funcionario> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Funcionario> lstFuncionarios = new List<Funcionario>();
            string sql = "SELECT * FROM Funcionario;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Funcionario func = new Funcionario();
                    func.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    func.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    func.ID = Convert.ToInt32(reader["ID"]);
                    func.CI = reader["CI"].ToString();
                    func.Email = reader["Email"].ToString();
                    func.Nombre = reader["Nombre"].ToString();
                    func.Telefono = reader["Telefono"].ToString();
                    func.TelefonoAux = reader["TelefonoAux"].ToString();
                    func.Direccion = reader["Direccion"].ToString();
                    func.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    func.Clave = func.DesencriptarPassword(reader["Clave"].ToString());
                    func.Activo = Convert.ToBoolean(reader["Activo"]);
                    func.TipoFuncionario = (FuncionarioTipo)Convert.ToInt32(reader["TipoFuncionario"]);
                    lstFuncionarios.Add(func);
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
            return lstFuncionarios;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@CI", this.CI));
            lstParametros.Add(new SqlParameter("@Email", this.Email));
            lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            lstParametros.Add(new SqlParameter("@Telefono", this.Telefono));
            lstParametros.Add(new SqlParameter("@TelefonoAux", this.TelefonoAux));
            lstParametros.Add(new SqlParameter("@Direccion", this.Direccion));
            lstParametros.Add(new SqlParameter("@FechaNac", this.FechaNac));
            lstParametros.Add(new SqlParameter("@Activo", this.Activo));
            lstParametros.Add(new SqlParameter("@TipoFuncionario", this.TipoFuncionario));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            //no necesita lazy
            return this.Leer(strCon);
        }

        public List<Funcionario> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        public List<Funcionario> GetBySucursal(Sucursal sucursal, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Funcionario> lstFuncionarios = new List<Funcionario>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@SucursalID", sucursal.ID));
            string sql = "SELECT * FROM Funcionario WHERE SucursalID = @SucursalID;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Funcionario func = new Funcionario();
                    func.Sucursal = sucursal;
                    func.SucursalID = sucursal.ID;
                    func.ID = Convert.ToInt32(reader["ID"]);
                    func.CI = reader["CI"].ToString();
                    func.Email = reader["Email"].ToString();
                    func.Nombre = reader["Nombre"].ToString();
                    func.Telefono = reader["Telefono"].ToString();
                    func.TelefonoAux = reader["TelefonoAux"].ToString();
                    func.Direccion = reader["Direccion"].ToString();
                    func.FechaNac = Convert.ToDateTime(reader["FechaNac"]);
                    func.Clave = func.DesencriptarPassword(reader["Clave"].ToString());
                    func.Activo = Convert.ToBoolean(reader["Activo"]);
                    func.TipoFuncionario = (FuncionarioTipo)Convert.ToInt32(reader["TipoFuncionario"]);
                    lstFuncionarios.Add(func);
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
            return lstFuncionarios;
        }

        #endregion
    }
}
