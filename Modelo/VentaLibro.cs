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
    public class VentaLibro : Persistencia, IPersistencia<VentaLibro>
    {
        public int ID { get; set; }
        public Libro Libro { get; set; }
        public Estudiante Estudiante { get; set; } //Estudiante tiene la lista
        public DateTime FechaHora { get; set; }
        public decimal Precio { get; set; }
        public VentaLibroEstado Estado { get; set; }


        public VentaLibro()
        {
            this.Libro = new Libro();
            this.Estudiante = new Estudiante();
        }

        public static bool ValidarVentaLibroInsert(VentaLibro venta)
        {
            try
            {
                string errorMsg = String.Empty;
                if (venta.Libro.ID < 1 || venta.Libro.Materia.ID < 1)
                {
                    errorMsg = "La venta debe estar asociada a un libro \n";
                }
                if (venta.Estudiante.ID < 1)
                {
                    errorMsg += "La venta debe estar asociada a un estudiante \n";
                }
                if (venta.Precio < 1)
                {
                    errorMsg += "La venta debe tener un valor \n";
                }
                if (!errorMsg.Equals(String.Empty))
                {
                    throw new ValidacionException(errorMsg);
                }
                return true;
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

        public static bool ValidarVentaLibroModificar(VentaLibro venta, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (venta.Libro.ID < 1 || venta.Libro.Materia.ID < 1)
                {
                    errorMsg = "La venta debe estar asociada a un libro \n";
                }
                if (venta.Estudiante.ID < 1)
                {
                    errorMsg += "La venta debe estar asociada a un estudiante \n";
                }
                if (venta.Precio < 1)
                {
                    errorMsg += "La venta debe tener un valor \n";
                }
                if (errorMsg.Equals(String.Empty) && !VentaLibro.ExisteVentaLibro(venta, strCon))
                {
                    errorMsg = "No existe la venta que desea modificar";
                }
                if (!errorMsg.Equals(String.Empty))
                {
                    throw new ValidacionException(errorMsg);
                }
                return true;
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

        public static bool ExisteVentaLibro(VentaLibro venta, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";

            if (venta.ID > 0 && venta.Libro.ID > 0 && venta.Libro.Materia.ID > 0 && venta.Estudiante.ID > 0)
            {
                sql = "SELECT * FROM VentaLibro WHERE ID = @ID AND LibroID = @LibroID AND MateriaID = @MateriaID AND EstudianteID = @EstudianteID";
                lstParametros.Add(new SqlParameter("@ID", venta.ID));
                lstParametros.Add(new SqlParameter("@LibroID", venta.Libro.ID));
                lstParametros.Add(new SqlParameter("@MateriaID", venta.Libro.Materia.ID));
                lstParametros.Add(new SqlParameter("@EstudianteID", venta.Estudiante.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar la Venta del libro");
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

            if (this.ID > 0 && this.Libro.ID > 0 && this.Libro.Materia.ID > 0 && this.Estudiante.ID > 0)
            {
                sql = "SELECT * FROM VentaLibro WHERE ID = @ID AND LibroID = @LibroID AND MateriaID = @MateriaID AND EstudianteID = @EstudianteID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@LibroID", this.Libro.ID));
                lstParametros.Add(new SqlParameter("@MateriaID", this.Libro.Materia.ID));
                lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar la Venta del libro");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    //this.Libro.LeerLazy(strCon);
                    //this.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    //this.Estudiante.LeerLazy(strCon);
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.Estado = (VentaLibroEstado)Convert.ToInt32(reader["Estado"]);
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
                this.ID = 0;
                this.ID = (int)Herramientas.ObtenerNumerador("VENLIB", strCon);
                List<SqlParameter> lstParametros = this.ObtenerParametros();
                string sql = "INSERT INTO VentaLibro VALUES (@ID, @LibroID, @MateriaID, @EstudianteID, @FechaHora, @Precio, @Estado);";
                int res = 0;
                res = Convert.ToInt32(Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, null));
                if (res > 0) seGuardo = true;
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
            string sql = "UPDATE VentaLibro SET FechaHora = @FechaHora, Precio = @Precio, Estado = @Estado WHERE ID = @ID AND LibroID = @LibroID AND MateriaID = @MateriaID AND EstudianteID = @EstudianteID;";
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
            lstParametros.Add(new SqlParameter("@LibroID", this.Libro.ID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.Libro.Materia.ID));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            string sql = "DELETE FROM VentaLibro WHERE ID = @ID AND LibroID = @LibroID AND MateriaID = @MateriaID AND EstudianteID = @EstudianteID";
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

        public List<VentaLibro> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<VentaLibro> lstVentas = new List<VentaLibro>();
            string sql = "SELECT * FROM VentaLibro;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    VentaLibro venta = new VentaLibro();
                    venta.ID = Convert.ToInt32(reader["ID"]);
                    venta.Libro.ID = Convert.ToInt32(reader["LibroID"]);
                    venta.Libro.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    venta.Libro.LeerLazy(strCon);
                    venta.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    venta.Estudiante.LeerLazy(strCon);
                    venta.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    venta.Precio = Convert.ToDecimal(reader["Precio"]);
                    venta.Estado = (VentaLibroEstado)Convert.ToInt32(reader["Estado"]);
                    lstVentas.Add(venta);
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
            return lstVentas;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@LibroID", this.Libro.ID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.Libro.Materia.ID));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            lstParametros.Add(new SqlParameter("@Estado", this.Estado));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public List<VentaLibro> GetAllLazy(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<VentaLibro> lstVentas = new List<VentaLibro>();
            string sql = "SELECT * FROM VentaLibro;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    VentaLibro venta = new VentaLibro();
                    venta.ID = Convert.ToInt32(reader["ID"]);
                    venta.Libro.ID = Convert.ToInt32(reader["LibroID"]);
                    venta.Libro.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    venta.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    venta.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    venta.Precio = Convert.ToDecimal(reader["Precio"]);
                    venta.Estado = (VentaLibroEstado)Convert.ToInt32(reader["Estado"]);
                    lstVentas.Add(venta);
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
            return lstVentas;
        }

        public List<VentaLibro> GetByEstado(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<VentaLibro> lstVentas = new List<VentaLibro>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Estado", this.Estado));
            string sql = "SELECT * FROM VentaLibro WHERE Estado = @Estado;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    VentaLibro venta = new VentaLibro();
                    venta.ID = Convert.ToInt32(reader["ID"]);
                    venta.Libro.ID = Convert.ToInt32(reader["LibroID"]);
                    venta.Libro.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    venta.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    venta.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    venta.Precio = Convert.ToDecimal(reader["Precio"]);
                    venta.Estado = (VentaLibroEstado)Convert.ToInt32(reader["Estado"]);
                    lstVentas.Add(venta);
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
            return lstVentas;
        }


        #endregion

    }
}
