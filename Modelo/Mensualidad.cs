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
    public class Mensualidad : Persistencia, IPersistencia<Mensualidad>
    {
        public int ID { get; set; }
        [JsonIgnore]
        public Sucursal Sucursal { get; set; } //en que sucursal la pago
        public int SucursalID { get; set; }
        public Estudiante Estudiante { get; set; }
        public DateTime FechaHora { get; set; } //hora de pago
        [JsonIgnore]
        public Grupo Grupo { get; set; }  //grupo al que pertenece el estudiante
        public int GrupoID { get; set; }
        public int MateriaID { get; set; }
        public int MesAsociado { get; set; }  //que mes esta pagando
        public int AnioAsociado { get; set; }  //que año corresponde
        [JsonIgnore]
        public Funcionario Funcionario { get; set; }
        public int FuncionarioID { get; set; }
        public decimal Precio { get; set; }
        public bool Paga { get; set; }
        public DateTime FechaVencimiento { get; set; }
        private static decimal Recargo { get; set; } // se hace privado para hacer un metodo static que si el recargo esta en 0, va a la base y lo carga y devuelve

        //un registro unico por estudiante + mes + anio

        public Mensualidad()
        {
            this.Sucursal = new Sucursal();
            this.Estudiante = new Estudiante();
            this.Grupo = new Grupo();
            this.Funcionario = new Funcionario();
        }

        public static bool ValidarMensualidadInsert(Mensualidad mensualidad, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (mensualidad.Estudiante.ID < 1)
                {
                    errorMsg = "Debe ingresar un estudiante \n";
                }
                if (mensualidad.Grupo.ID < 1 || mensualidad.Grupo.Materia.ID < 1)
                {
                    errorMsg += "Debe asociar la mensualidad a un grupo \n";
                }
                if (mensualidad.AnioAsociado < 2000 || mensualidad.AnioAsociado >= 2050)
                {
                    errorMsg += "Año invalido \n";
                }
                if (mensualidad.MesAsociado < 1 || mensualidad.MesAsociado > 12)
                {
                    errorMsg += "Mes invalido \n";
                }
                if (mensualidad.Precio < 1)
                {
                    errorMsg += "Debe ingresar un precio \n";
                }
                if (errorMsg.Equals(String.Empty) && Mensualidad.ExisteMensualidad(mensualidad, strCon))
                {
                    errorMsg = "Ya existe una mensualidad asociada al estudiante para el mes seleccionado";
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

        public static bool ValidarMensualidadModificar(Mensualidad mensualidad, string strCon)
        {
            try
            {
                string errorMsg = String.Empty;
                if (mensualidad.Grupo.ID < 1 || mensualidad.Grupo.Materia.ID < 1)
                {
                    errorMsg = "Debe asociar la mensualidad a un grupo \n";
                }
                if (mensualidad.AnioAsociado < 2000)
                {
                    errorMsg += "Año invalido \n";
                }
                if (mensualidad.MesAsociado < 1 || mensualidad.MesAsociado > 12)
                {
                    errorMsg += "Mes invalido \n";
                }
                if (mensualidad.Precio < 1)
                {
                    errorMsg += "Debe ingresar un precio \n";
                }
                if (errorMsg.Equals(String.Empty))
                {
                    Mensualidad mensualidadAux = new Mensualidad();
                    mensualidadAux.ID = mensualidad.ID;
                    if (mensualidadAux.Leer(strCon))
                    {
                        if (mensualidadAux.AnioAsociado != mensualidad.AnioAsociado || mensualidadAux.MesAsociado != mensualidad.MesAsociado)
                        {
                            if (Mensualidad.ExisteMensualidad(mensualidad, strCon))
                            {
                                errorMsg = "Ya existe la mensualidad del estudiante para el año " + mensualidad.AnioAsociado.ToString().Trim() + " y mes " + mensualidad.MesAsociado.ToString().Trim();
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "No existe la mensualidad que desea modificar";
                    }
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

        public static bool ExisteMensualidad(Mensualidad mensualidad, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (mensualidad.Estudiante.ID > 0 && mensualidad.MesAsociado > 0 && mensualidad.AnioAsociado > 0)
            {
                sql = "SELECT * FROM Mensualidad WHERE EstudianteID = @EstudianteID AND MesAsociado = @MesAsociado AND AnioAsociado = @AnioAsociado";
                lstParametros.Add(new SqlParameter("@EstudianteID", mensualidad.Estudiante.ID));
                lstParametros.Add(new SqlParameter("@MesAsociado", mensualidad.MesAsociado));
                lstParametros.Add(new SqlParameter("@AnioAsociado", mensualidad.AnioAsociado));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar a la Mensualidad");
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

        public static bool ExisteMensualidadByID(Mensualidad mensualidad, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (mensualidad.ID > 0)
            {
                sql = "SELECT * FROM Mensualidad WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", mensualidad.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar a la Mensualidad");
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
                sql = "SELECT * FROM Mensualidad WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else if (this.Estudiante.ID > 0 && MesAsociado > 0 && AnioAsociado > 0)
            {
                sql = "SELECT * FROM Mensualidad WHERE EstudianteID = @EstudianteID AND MesAsociado = @MesAsociado AND AnioAsociado = @AnioAsociado";
                lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
                lstParametros.Add(new SqlParameter("@MesAsociado", this.MesAsociado));
                lstParametros.Add(new SqlParameter("@AnioAsociado", this.AnioAsociado));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar la Mensualidad");
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
                    this.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    this.Estudiante.LeerLazy(strCon);
                    this.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    this.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    this.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    this.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    this.MesAsociado = Convert.ToInt32(reader["MesAsociado"]);
                    this.AnioAsociado = Convert.ToInt32(reader["AnioAsociado"]);
                    this.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    this.Precio = Convert.ToDecimal(reader["Precio"]);
                    this.Paga = Convert.ToBoolean(reader["Paga"]);
                    this.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
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
                string sql = "INSERT INTO Mensualidad VALUES (@SucursalID, @EstudianteID, @FechaHora, @GrupoID, @MateriaID, @MesAsociado, @AnioAsociado, @FuncionarioID, @Precio, @Paga, @FechaVencimiento); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE Mensualidad SET SucursalID = @SucursalID, FechaHora = @FechaHora, GrupoID = @GrupoID, MateriaID = @MateriaID, MesAsociado = @MesAsociado, AnioAsociado = @AnioAsociado, Precio = @Precio, Paga = @Paga, FechaVencimiento = @FechaVencimiento WHERE ID = @ID;";
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
            string sql = "DELETE FROM Mensualidad WHERE ID = @ID";
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

        public List<Mensualidad> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Mensualidad> lstMensualidades = new List<Mensualidad>();
            string sql = "SELECT * FROM Mensualidad;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Mensualidad mensualidad = new Mensualidad();
                    mensualidad.ID = Convert.ToInt32(reader["ID"]);
                    mensualidad.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    mensualidad.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    mensualidad.Estudiante.ID = Convert.ToInt32(reader["EstudianteID"]);
                    mensualidad.Estudiante.LeerLazy(strCon);
                    mensualidad.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    mensualidad.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    mensualidad.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    mensualidad.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    mensualidad.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    mensualidad.MesAsociado = Convert.ToInt32(reader["MesAsociado"]);
                    mensualidad.AnioAsociado = Convert.ToInt32(reader["AnioAsociado"]);
                    mensualidad.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    mensualidad.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    mensualidad.Precio = Convert.ToDecimal(reader["Precio"]);
                    mensualidad.Paga = Convert.ToBoolean(reader["Paga"]);
                    mensualidad.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                    lstMensualidades.Add(mensualidad);
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
            return lstMensualidades;
        }

        public List<Mensualidad> GetAllByEstudianteGrupo(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Mensualidad> lstMensualidades = new List<Mensualidad>();
            string sql = "SELECT * FROM Mensualidad WHERE EstudianteID = @EstudianteID AND GrupoID = @GrupoID AND MatriculaID = @MatriculaID;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            lstParametros.Add(new SqlParameter("@MatriculaID", this.MateriaID));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Mensualidad mensualidad = new Mensualidad();
                    mensualidad.ID = Convert.ToInt32(reader["ID"]);
                    mensualidad.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    mensualidad.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    mensualidad.Estudiante = this.Estudiante;
                    mensualidad.GrupoID = this.GrupoID;
                    mensualidad.MateriaID = this.MateriaID;
                    mensualidad.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    mensualidad.MesAsociado = Convert.ToInt32(reader["MesAsociado"]);
                    mensualidad.AnioAsociado = Convert.ToInt32(reader["AnioAsociado"]);
                    mensualidad.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    mensualidad.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    mensualidad.Precio = Convert.ToDecimal(reader["Precio"]);
                    mensualidad.Paga = Convert.ToBoolean(reader["Paga"]);
                    mensualidad.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                    lstMensualidades.Add(mensualidad);
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
            return lstMensualidades;
        }

        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@SucursalID", this.SucursalID));
            lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@GrupoID", this.GrupoID));
            lstParametros.Add(new SqlParameter("@MateriaID", this.MateriaID));
            lstParametros.Add(new SqlParameter("@MesAsociado", this.MesAsociado));
            lstParametros.Add(new SqlParameter("@AnioAsociado", this.AnioAsociado));
            lstParametros.Add(new SqlParameter("@FuncionarioID", this.FuncionarioID));
            lstParametros.Add(new SqlParameter("@Precio", this.Precio));
            lstParametros.Add(new SqlParameter("@Paga", this.Paga));
            lstParametros.Add(new SqlParameter("@FechaVencimiento", this.FechaVencimiento));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            return this.Leer(strCon);
        }

        public List<Mensualidad> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }

        public List<Mensualidad> LeerPorEstudiante(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Mensualidad> lstMensualidades = new List<Mensualidad>();
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            string sql = "";
            if (this.Estudiante.ID > 0 && this.AnioAsociado > 0)
            {
                sql = "SELECT * FROM Mensualidad WHERE EstudianteID = @EstudianteID AND AnioAsociado = @AnioAsociado";
                lstParametros.Add(new SqlParameter("@EstudianteID", this.Estudiante.ID));
                lstParametros.Add(new SqlParameter("@AnioAsociado", this.AnioAsociado));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar la Mensualidad");
            }
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Mensualidad mensualidad = new Mensualidad();
                    mensualidad.ID = Convert.ToInt32(reader["ID"]);
                    mensualidad.Sucursal.ID = Convert.ToInt32(reader["SucursalID"]);
                    mensualidad.SucursalID = Convert.ToInt32(reader["SucursalID"]);
                    mensualidad.Estudiante = this.Estudiante;
                    mensualidad.Grupo.ID = Convert.ToInt32(reader["GrupoID"]);
                    mensualidad.GrupoID = Convert.ToInt32(reader["GrupoID"]);
                    mensualidad.Grupo.Materia.ID = Convert.ToInt32(reader["MateriaID"]);
                    mensualidad.MateriaID = Convert.ToInt32(reader["MateriaID"]);
                    mensualidad.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    mensualidad.MesAsociado = Convert.ToInt32(reader["MesAsociado"]);
                    mensualidad.AnioAsociado = this.AnioAsociado;
                    mensualidad.Funcionario.ID = Convert.ToInt32(reader["FuncionarioID"]);
                    mensualidad.FuncionarioID = Convert.ToInt32(reader["FuncionarioID"]);
                    mensualidad.Precio = Convert.ToDecimal(reader["Precio"]);
                    mensualidad.Paga = Convert.ToBoolean(reader["Paga"]);
                    mensualidad.FechaVencimiento = Convert.ToDateTime(reader["FechaVencimiento"]);
                    lstMensualidades.Add(mensualidad);
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
            return lstMensualidades;
        }


        #endregion


    }
}
