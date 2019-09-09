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
    public class Libro : Persistencia, IPersistencia<Libro>
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public Materia Materia { get; set; } //la materia tiene una lista de libros
        public decimal Precio { get; set; }
        public string Autor { get; set; }
        public string Editorial { get; set; }


        #region Metodos

        public Libro()
        {
            this.Materia = new Materia();
        }

        public static bool ValidarInsertLibro(string strCon, Libro libro)
        {
            string errorMsg = String.Empty;
            if (libro.Nombre.Equals(String.Empty))
            {
                errorMsg += "Debe ingresar el nombre del libro \n";
            }
            if (libro.Precio <= 0)
            {
                errorMsg += "Debe ingresar el precio del libro \n";
            }
            if (libro.Materia.ID < 1)
            {
                errorMsg += "Debe asociar una materia al libro \n";
            }
            if (errorMsg.Equals(String.Empty) && Libro.ExisteLibroByNombre(strCon, libro))
            {
                errorMsg += "Ya existe el libro \n";
            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ValidarModificarLibro(string strCon, Libro libro)
        {
            string errorMsg = String.Empty;
            if (libro.Nombre == String.Empty)
            {
                errorMsg += "Debe ingresar el nombre del libro \n";
            }
            if (libro.Precio <= 0)
            {
                errorMsg += "Debe ingresar el precio del libro \n";
            }
            if (libro.Materia.ID < 1)
            {
                errorMsg += "Debe asociar una materia al libro \n";
            }
            if (errorMsg.Equals(String.Empty))
            {
                Libro libroAux = new Libro();
                libroAux.Materia = libro.Materia;
                libroAux.ID = libro.ID;
                if (libroAux.Leer(strCon))
                {
                    if (libroAux.Nombre != libro.Nombre)
                    {
                        if (Libro.ExisteLibroByNombre(strCon, libro))
                        {
                            errorMsg = "Ya existe un libro con el nombre seleccionado";
                        }
                    }
                }
                else
                {
                    errorMsg += "No existe el libro que desea modificar \n";
                }
            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ExisteLibro(string strCon, Libro libro)
        {
            {
                SqlConnection con = new SqlConnection(strCon);
                bool existe = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";
                if (libro.Materia != null && libro.Materia.ID > 0)
                {
                    sql = "SELECT * FROM Libro WHERE ID = @ID AND MateriaID = @MateriaID";
                    lstParametros.Add(new SqlParameter("@ID", libro.ID));
                    lstParametros.Add(new SqlParameter("@MateriaID", libro.Materia.ID));
                }
                else
                {
                    throw new ValidacionException("Datos insuficientes para buscar al libro");
                }
                try
                {
                    con.Open();
                    reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                    while (reader.Read())
                    {
                        existe = true;
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
                return existe;
            }
        }

        public static bool ExisteLibroByNombre(string strCon, Libro libro)
        {
            {
                SqlConnection con = new SqlConnection(strCon);
                bool existe = false;
                List<SqlParameter> lstParametros = new List<SqlParameter>();
                SqlDataReader reader = null;
                string sql = "";
                if (libro.Materia != null && libro.Materia.ID > 0)
                {
                    sql = "SELECT * FROM Libro WHERE MateriaID = @MateriaID AND Nombre = @Nombre";
                    lstParametros.Add(new SqlParameter("@Nombre", libro.Nombre));
                    lstParametros.Add(new SqlParameter("@MateriaID", libro.Materia.ID));
                }
                else
                {
                    throw new ValidacionException("Datos insuficientes para buscar al libro");
                }
                try
                {
                    con.Open();
                    reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                    while (reader.Read())
                    {
                        existe = true;
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
                return existe;
            }
        }


        #endregion

        #region Persistencia

        public bool Leer(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (this.Materia != null && this.Materia.ID > 0 && this.ID > 0)
            {
                sql = "SELECT * FROM Libro WHERE ID = @ID AND MateriaID = @MateriaID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
                lstParametros.Add(new SqlParameter("@MateriaID", this.Materia.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Libro");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.Nombre = reader["Nombre"].ToString().Trim();
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.Autor = reader["Autor"].ToString();
                    this.Editorial = reader["Editorial"].ToString();
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
                this.ID = (int)Herramientas.ObtenerNumerador("LIBRO", strCon);
                if (this.ID > 0)
                {
                    List<SqlParameter> lstParametros = this.ObtenerParametros();
                    string sql = "INSERT INTO Libro VALUES (@ID, @MateriaID, @Nombre, @Precio, @Autor, @Editorial);";
                    int ret = 0;
                    ret = Convert.ToInt32(Persistencia.EjecutarNoQuery(con, sql, lstParametros, CommandType.Text, null));
                    if (ret > 0) seGuardo = true;
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
            string sql = "UPDATE Libro SET Nombre = @Nombre, Precio = @Precio, Autor = @Autor, Editorial = @Editorial WHERE ID = @ID AND MateriaID = @MateriaID;";
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
            lstParametros.Add(new SqlParameter("@MateriaID", this.Materia.ID));
            string sql = "DELETE FROM Libro WHERE ID = @ID AND MateriaID = @MateriaID";
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

        public List<Libro> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Libro> lstLibros = new List<Libro>();
            string sql = "SELECT * FROM Libro WHERE ID <> 0;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Libro libro = new Libro();
                    libro.ID = Convert.ToInt32(reader["ID"]);
                    libro.Nombre = reader["Nombre"].ToString().Trim();
                    libro.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    libro.Precio = Convert.ToDecimal(reader["Precio"]);
                    libro.Autor = reader["Autor"].ToString();
                    libro.Editorial = reader["Editorial"].ToString();
                    lstLibros.Add(libro);
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
            return lstLibros;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            lstParametros.Add(new SqlParameter("@MateriaID", this.Materia.ID));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            lstParametros.Add(new SqlParameter("@Autor", this.Autor));
            lstParametros.Add(new SqlParameter("@Editorial", this.Editorial));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public List<Libro> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        #endregion

    }
}
