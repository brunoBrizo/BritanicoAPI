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
    public class Convenio : Persistencia, IPersistencia<Convenio>
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaHora { get; set; } //momento que se lo creo
        public int Anio { get; set; } //año al que hace referencia
        public string AsociadoNombre { get; set; } //nombre de la empresa asociada al convenio
        public string AsociadoTel { get; set; }
        public string AsociadoMail { get; set; }
        public string AsociadoDireccion { get; set; }
        public List<Estudiante> LstEstudiantes { get; set; } = new List<Estudiante>(); //el estudiante tiene el convenio tambien
        public decimal Descuento { get; set; }

        //validar que exista un convenio con la misma empresa por año, no mas de uno
        //hacer metodo y chequearlo en el guardar o hacer un validar

        public Convenio() { }

        public static bool ValidarConvenioInsert(Convenio convenio, string strCon)
        {
            string errorMsg = String.Empty;
            if (convenio.Nombre.Equals(String.Empty))
            {
                errorMsg = "Debe ingresar el nombre del Convenio \n";
            }
            if (convenio.Anio < 2010)
            {
                errorMsg += "Verifique el año del Convenio \n";
            }
            if (convenio.AsociadoNombre.Equals(String.Empty) || convenio.AsociadoTel.Equals(String.Empty))
            {
                errorMsg += "Debe ingresar Nombre y Telefono del asociado \n";
            }
            if (!convenio.AsociadoMail.Equals(String.Empty) && !Herramientas.ValidarMail(convenio.AsociadoMail))
            {
                errorMsg += "Mail invalido \n";
            }
            Convenio convenioAux = new Convenio
            {
                ID = 0,
                Nombre = convenio.Nombre,
                Anio = convenio.Anio
            };
            if (errorMsg.Equals(String.Empty) && convenioAux.Leer(strCon))
            {
                errorMsg += "Ya existe el Convenio: " + convenio.Nombre.ToUpper() + " para el año " + convenio.Anio.ToString().Trim();
            }
            if (!errorMsg.Equals(String.Empty))
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ValidarConvenioModificar(Convenio convenio, string strCon)
        {
            string errorMsg = String.Empty;
            if (convenio.Nombre.Equals(String.Empty))
            {
                errorMsg = "Debe ingresar el nombre del Convenio \n";
            }
            if (convenio.Anio < 2010)
            {
                errorMsg += "Verifique el año del Convenio \n";
            }
            if (convenio.AsociadoNombre.Equals(String.Empty) || convenio.AsociadoTel.Equals(String.Empty))
            {
                errorMsg += "Debe ingresar Nombre y Telefono del asociado \n";
            }
            if (!convenio.AsociadoMail.Equals(String.Empty) && !Herramientas.ValidarMail(convenio.AsociadoMail))
            {
                errorMsg += "Mail invalido \n";
            }
            Convenio convenioAux = new Convenio
            {
                ID = convenio.ID
            };
            if (errorMsg.Equals(String.Empty))
            {
                if (convenioAux.Leer(strCon))
                {
                    if (convenio.Anio != convenioAux.Anio)
                    {
                        convenioAux.ID = 0;
                        convenioAux.Nombre = convenio.Nombre;
                        convenioAux.Anio = convenio.Anio;
                        if (convenioAux.Leer(strCon))
                        {
                            errorMsg += "Ya existe el Convenio: " + convenio.Nombre.ToUpper() + " para el año " + convenio.Anio.ToString().Trim();
                        }
                    }
                }
                else
                {
                    errorMsg += "No existe el Convenio que desea modificar \n";
                }
            }
            if (!errorMsg.Equals(String.Empty))
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
                sql = "SELECT * FROM Convenio WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else if (this.Anio > 0 && !this.Nombre.Equals(String.Empty))
            {
                sql = "SELECT * FROM Convenio WHERE Anio = @Anio AND Nombre = @Nombre";
                lstParametros.Add(new SqlParameter("@Anio", this.Anio));
                lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Convenio");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.Nombre = reader["Nombre"].ToString();
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    this.Anio = Convert.ToInt32(reader["Anio"]);
                    this.AsociadoNombre = reader["AsociadoNombre"].ToString();
                    this.AsociadoTel = reader["AsociadoTel"].ToString();
                    this.AsociadoMail = reader["AsociadoMail"].ToString();
                    this.AsociadoDireccion = reader["AsociadoDireccion"].ToString();
                    this.Descuento = Convert.ToDecimal(reader["Descuento"]);
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
                string sql = "INSERT INTO Convenio VALUES (@Nombre, @FechaHora, @Anio, @AsociadoNombre, @AsociadoTel, @AsociadoMail, @AsociadoDireccion, @Descuento); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE Convenio SET Nombre = @Nombre, FechaHora = @FechaHora, Anio = @Anio, AsociadoNombre = @AsociadoNombre, AsociadoTel = @AsociadoTel, AsociadoMail = @AsociadoMail, AsociadoDireccion = @AsociadoDireccion, Descuento = @Descuento WHERE ID = @ID;";
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
            string sql = "DELETE FROM Convenio WHERE ID = @ID";
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

        public List<Convenio> GetAllByAnio(int anio, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Convenio> lstConvenio = new List<Convenio>();
            string sql = "SELECT * FROM Convenio WHERE Anio = @Anio;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Anio", anio));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Convenio convenio = new Convenio();
                    convenio.ID = Convert.ToInt32(reader["ID"]);
                    convenio.Nombre = reader["Nombre"].ToString();
                    convenio.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    convenio.Anio = Convert.ToInt32(reader["Anio"]);
                    convenio.AsociadoNombre = reader["AsociadoNombre"].ToString();
                    convenio.AsociadoTel = reader["AsociadoTel"].ToString();
                    convenio.AsociadoMail = reader["AsociadoMail"].ToString();
                    convenio.AsociadoDireccion = reader["AsociadoDireccion"].ToString();
                    convenio.Descuento = Convert.ToDecimal(reader["Descuento"]);
                    lstConvenio.Add(convenio);
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
            return lstConvenio;
        }

        public List<Convenio> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Convenio> lstConvenio = new List<Convenio>();
            string sql = "SELECT * FROM Convenio";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Convenio convenio = new Convenio();
                    convenio.ID = Convert.ToInt32(reader["ID"]);
                    convenio.Nombre = reader["Nombre"].ToString();
                    convenio.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    convenio.Anio = Convert.ToInt32(reader["Anio"]);
                    convenio.AsociadoNombre = reader["AsociadoNombre"].ToString();
                    convenio.AsociadoTel = reader["AsociadoTel"].ToString();
                    convenio.AsociadoMail = reader["AsociadoMail"].ToString();
                    convenio.AsociadoDireccion = reader["AsociadoDireccion"].ToString();
                    convenio.Descuento = Convert.ToDecimal(reader["Descuento"]);
                    lstConvenio.Add(convenio);
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
            return lstConvenio;
        }
        
        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@Anio", this.Anio));
            lstParametros.Add(new SqlParameter("@AsociadoNombre", this.AsociadoNombre));
            lstParametros.Add(new SqlParameter("@AsociadoTel", this.AsociadoTel));
            lstParametros.Add(new SqlParameter("@AsociadoMail", this.AsociadoMail));
            lstParametros.Add(new SqlParameter("@AsociadoDireccion", this.AsociadoDireccion));
            lstParametros.Add(new SqlParameter("@Descuento", this.Descuento));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            //no necesita lazy
            return this.Leer(strCon);
        }

        public List<Convenio> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        #endregion

    }
}
