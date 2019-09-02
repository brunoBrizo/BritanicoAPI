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
    public class Materia : Persistencia, IPersistencia<Materia>
    {
        public int ID { get; set; }
        [JsonIgnore]
        public Sucursal Sucursal { get; set; }
        public int SucursalID { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public decimal NotaFinalOralMax { get; set; }
        public decimal NotaFinalWrittingMax { get; set; }
        public decimal NotaFinalListeningMax { get; set; }
        public decimal ExamenPrecio { get; set; } //se utiliza para dar de alta el historial, para matricular estudiantes por convenio


        public Materia()
        {
            this.Sucursal = new Sucursal();
        }

        public static bool ValidarMateriaInsert(Materia materia, string strCon)
        {
            string errorMsg = String.Empty;
            if (materia.Nombre.Equals(String.Empty))
            {
                errorMsg = "Debe ingresar el nombre de la materia \n";
            }
            if (materia.Precio < 1)
            {
                errorMsg += "Debe ingresar el precio de la materia \n";
            }
            if (materia.Sucursal.ID < 1)
            {
                errorMsg += "Debe ingresar la sucursal de la materia \n";
            }
            Materia materiaAux = new Materia
            {
                ID = 0,
                Nombre = materia.Nombre,
                SucursalID = materia.SucursalID
            };
            if (errorMsg.Equals(String.Empty) && Materia.ExisteMateria(materiaAux, strCon))
            {
                errorMsg += "Ya existe la materia: " + materiaAux.Nombre.ToUpper().Trim();
            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ValidarMateriaModificar(Materia materia, string strCon)
        {
            string errorMsg = String.Empty;
            if (materia.Nombre.Equals(String.Empty))
            {
                errorMsg = "Debe ingresar el nombre de la materia \n";
            }
            if (materia.Precio < 1)
            {
                errorMsg += "Debe ingresar el precio de la materia \n";
            }
            if (materia.Sucursal.ID < 1)
            {
                errorMsg += "Debe ingresar la sucursal de la materia \n";
            }
            Materia materiaAux = new Materia
            {
                ID = materia.ID
            };
            if (errorMsg.Equals(String.Empty))
            {
                if (materiaAux.LeerLazy(strCon))
                {
                    if (materiaAux.Nombre != materia.Nombre)
                    {
                        materiaAux.ID = 0;
                        materiaAux.Nombre = materia.Nombre;
                        if (materiaAux.LeerLazy(strCon))
                        {
                            errorMsg += "Ya existe la materia: " + materia.Nombre.ToUpper().Trim();
                        }
                    }
                }
                else
                {
                    errorMsg += "No existe la materia que desea modificar \n";
                }
            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ExisteMateria(Materia materia, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";

            if (materia.ID > 0)
            {
                sql = "SELECT * FROM Materia WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", materia.ID));
            }
            else if (!materia.Nombre.Equals(String.Empty))
            {
                sql = "SELECT * FROM Materia WHERE Nombre = @Nombre AND SucursalID = @SucursalID";
                lstParametros.Add(new SqlParameter("@Nombre", materia.Nombre));
                lstParametros.Add(new SqlParameter("@SucursalID", materia.SucursalID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar a la Materia");
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
            {
                SqlConnection con = new SqlConnection(strCon);
                bool ok = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";

                if (this.ID > 0)
                {
                    sql = "SELECT * FROM Materia WHERE ID = @ID";
                    lstParametros.Add(new SqlParameter("@ID", this.ID));
                }
                else if (!this.Nombre.Equals(String.Empty))
                {
                    sql = "SELECT * FROM Materia WHERE Nombre = @Nombre";
                    lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
                }
                else
                {
                    throw new ValidacionException("Datos insuficientes para buscar a la Materia");
                }
                try
                {
                    con.Open();
                    reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                    while (reader.Read())
                    {
                        this.ID = Convert.ToInt32(reader["ID"]);
                        this.Nombre = reader["Nombre"].ToString().Trim();
                        this.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                        this.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                        this.NotaFinalListeningMax = Convert.ToDecimal(reader["NotaFinalListeningMax"]);
                        this.NotaFinalOralMax = Convert.ToDecimal(reader["NotaFinalOralMax"]);
                        this.NotaFinalWrittingMax = Convert.ToDecimal(reader["NotaFinalWrittingMax"]);
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
        }

        public bool Guardar(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            SqlTransaction tran = null;
            bool seGuardo = false;
            try
            {
                this.ID = 0;
                this.ID = (int)Herramientas.ObtenerNumerador("MATER", strCon);
                if (this.ID > 0)
                {
                    con.Open();
                    tran = con.BeginTransaction();
                    List<SqlParameter> lstParametros = this.ObtenerParametros();
                    string sql = "INSERT INTO Materia VALUES (@ID, @SucursalID, @Nombre, @Precio, @NotaFinalOralMax, @NotaFinalWrittingMax, @NotaFinalListeningMax);";
                    int ret = 0;
                    ret = Convert.ToInt32(Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, tran));
                    if (ret > 0)
                    {
                        MateriaHistorial materiaHistorial = new MateriaHistorial
                        {
                            ID = 0,
                            MateriaID = this.ID,
                            ExamenPrecio = this.ExamenPrecio,
                            Anio = DateTime.Now.Year,
                            SucursalID = this.SucursalID
                        };
                        if (!MateriaHistorial.ExisteMateriaHistorial(materiaHistorial, con))
                            materiaHistorial.GuardarTransaccional(con, tran);
                        seGuardo = true;
                    }
                    tran.Commit();
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
            string sql = "UPDATE Materia SET SucursalID = @SucursalID, Nombre = @Nombre, Precio = @Precio, NotaFinalWrittingMax = @NotaFinalWrittingMax, NotaFinalOralMax = @NotaFinalOralMax, NotaFinalListeningMax = @NotaFinalListeningMax WHERE ID = @ID;";
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
            string sql = "DELETE FROM Materia WHERE ID = @ID";
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

        public List<Materia> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Materia> lstMaterias = new List<Materia>();
            string sql = "SELECT * FROM Materia;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Materia materia = new Materia();
                    materia.ID = Convert.ToInt32(reader["ID"]);
                    materia.Nombre = reader["Nombre"].ToString().Trim();
                    materia.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    materia.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    materia.Precio = Convert.ToDecimal(reader["Precio"]);
                    materia.NotaFinalListeningMax = Convert.ToDecimal(reader["NotaFinalListeningMax"]);
                    materia.NotaFinalOralMax = Convert.ToDecimal(reader["NotaFinalOralMax"]);
                    materia.NotaFinalWrittingMax = Convert.ToDecimal(reader["NotaFinalWrittingMax"]);
                    lstMaterias.Add(materia);
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
            return lstMaterias;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            lstParametros.Add(new SqlParameter("@NotaFinalWrittingMax", this.NotaFinalWrittingMax));
            lstParametros.Add(new SqlParameter("@NotaFinalOralMax", this.NotaFinalOralMax));
            lstParametros.Add(new SqlParameter("@NotaFinalListeningMax", this.NotaFinalListeningMax));
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
                sql = "SELECT * FROM Materia WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else if (!this.Nombre.Equals(String.Empty))
            {
                sql = "SELECT * FROM Materia WHERE Nombre = @Nombre";
                lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar a la Materia");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.Nombre = reader["Nombre"].ToString().Trim();
                    this.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    this.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.NotaFinalListeningMax = Convert.ToDecimal(reader["NotaFinalListeningMax"]);
                    this.NotaFinalOralMax = Convert.ToDecimal(reader["NotaFinalOralMax"]);
                    this.NotaFinalWrittingMax = Convert.ToDecimal(reader["NotaFinalWrittingMax"]);
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

        public List<Materia> GetAllLazy(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Materia> lstMaterias = new List<Materia>();
            string sql = "SELECT * FROM Materia;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Materia materia = new Materia();
                    materia.ID = Convert.ToInt32(reader["ID"]);
                    materia.Nombre = reader["Nombre"].ToString().Trim();
                    materia.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    materia.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    materia.Precio = Convert.ToDecimal(reader["Precio"]);
                    materia.NotaFinalListeningMax = Convert.ToDecimal(reader["NotaFinalListeningMax"]);
                    materia.NotaFinalOralMax = Convert.ToDecimal(reader["NotaFinalOralMax"]);
                    materia.NotaFinalWrittingMax = Convert.ToDecimal(reader["NotaFinalWrittingMax"]);
                    lstMaterias.Add(materia);
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
            return lstMaterias;
        }

        #endregion

    }
}
