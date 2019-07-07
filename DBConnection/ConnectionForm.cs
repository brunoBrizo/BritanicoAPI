using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace DBConnection
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();
        }

        public object ConfigurationManager { get; private set; }
        public object ConfigurationUserLevel { get; private set; }

        private void btnProbarConexion_Click(object sender, EventArgs e)
        {
            try
            {
                string server = txtServidor.Text.Trim();
                string db = txtBaseDeDatos.Text.Trim();
                string user = txtUsuario.Text.Trim();
                string password = txtPassword.Text.Trim();

                string strConnection = "Server= " + server + "; Database = " + db + "; User Id = " + user + "; Password = " + password + ";";
                string strConnectionEncripted = Encriptacion.Encriptar(strConnection);

                SqlConnection con = new SqlConnection(strConnection);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                        MessageBox.Show("Conectado correctamente!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    con.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string server = txtServidor.Text.Trim();
                string db = txtBaseDeDatos.Text.Trim();
                string user = txtUsuario.Text.Trim();
                string password = txtPassword.Text.Trim();

                string strConnection = "Server= " + server + "; Database = " + db + "; User Id = " + user + "; Password = " + password + ";";
                string strConnectionEncripted = Encriptacion.Encriptar(strConnection);

                SqlConnection con = new SqlConnection(strConnection);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {
                        MessageBox.Show("Conexion guardada correctamente!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        con.Close();
                        AppSettings.GuardarConnectionString(strConnectionEncripted);
                    }
                    else
                    {
                        MessageBox.Show("Error al abrir la conexion, verifique los datos!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
