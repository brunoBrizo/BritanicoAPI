using BibliotecaBritanico.Utilidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace BibliotecaBritanico.Modelo
{
    public class Email : Persistencia, IPersistencia<Email>
    {
        public int ID { get; set; }
        public string DestinatarioEmail { get; set; }
        public string DestinatarioNombre { get; set; }
        public string Asunto { get; set; }
        public string CuerpoHTML { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Enviado { get; set; }


        public Email() { }


        public async Task Enviar(string strCon, Parametro paramEmail, Parametro paramClave)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(this.DestinatarioEmail);
                mailMessage.Subject = this.Asunto;
                mailMessage.SubjectEncoding = Encoding.UTF8;

                mailMessage.Body = this.ArmarCuerpoHtml();
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;


                mailMessage.From = new MailAddress(paramEmail.Valor);

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential(paramEmail.Valor, paramClave.Valor);
                client.Port = 587;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                await client.SendMailAsync(mailMessage);
            }
            catch (SqlException ex)
            {
                this.Enviado = false;
                try
                {
                    this.Modificar(strCon);
                }
                catch (Exception ex2)
                {
                    throw ex2;
                }
                throw ex;
            }
            catch (Exception ex)
            {
                this.Enviado = false;
                try
                {
                    this.Modificar(strCon);
                }
                catch(Exception ex2)
                {
                    throw ex2;
                }
                throw ex;
            }
        }

        private string ObtenerPieEmailHtml()
        {
            string pieHtml = "";
            pieHtml = "<table class='sc - jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical - align: -webkit - baseline - middle; font - size: medium; font - family: &quot; Trebuchet MS&quot; ; '>";
            pieHtml += "<tbody><tr><td> ";
            pieHtml += "<table class='sc - jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical - align: -webkit - baseline - middle; font - size: medium; font - family: &quot; Trebuchet MS&quot; ; '>";
            pieHtml += "<tbody><tr><td style='vertical-align: middle;'><h3 class='sc-hzDkRC kpsoyz' color='#0710C6' style='margin: 0px; font-size: 18px; color: rgb(7, 16, 198);'><span>Administración</span><span>&nbsp;</span><span></span></h3><p class='sc-fBuWsC eeihxG' color='#0710C6' font-size='medium' style='margin: 0px; font-weight: 500; color: rgb(7, 16, 198); font-size: 14px; line-height: 22px;'><span>Instituto Británico de Rivera</span></p></td><td width='30'></td><td width='1' class='sc-bRBYWo ccSRck' color='#e4003c' direction='vertical' style='width: 1px; border-bottom: none; border-left: 1px solid rgb(228, 0, 60);'></td><td width='30'></td><td style='vertical-align: middle;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;;'><tbody><tr height='25' style='vertical-align: middle;'><td width='30' style='vertical-align: middle;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;;'><tbody><tr><td style='vertical-align: bottom;'><span width='11' class='sc-gPEVay eQYmiW' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'><img width='12' class='sc-jDwBTQ dWtMUn' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/phone-icon-2x.png' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'>";
            //pieHtml += "</span></td></tr></tbody></table></td><td style='padding: 0px; color: rgb(7, 16, 198);'><a class='sc-iRbamj blSEcj' href='tel:462-24260' color='#0710C6' style='text-decoration: none; color: rgb(7, 16, 198); font-size: 12px;'><span>462-24260</span></a> | <a class='sc-iRbamj blSEcj' href='tel:099057586' color='#0710C6' style='text-decoration: none; color: rgb(7, 16, 198); font-size: 12px;'><span>099057586</span></a></td></tr><tr height='25' style='vertical-align: middle;'><td width='30' style='vertical-align: middle;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;;'><tbody><tr><td style='vertical-align: bottom;'><span width='11' class='sc-gPEVay eQYmiW' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'><img width='0' class='sc-jDwBTQ dWtMUn' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/email-icon-2x.png' color='#e4003c' style='display: none !important; background-color: rgb(228, 0, 60); visibility: hidden !important; opacity: 0 !important; background-position: 12px 0px;' height='0'></span></td></tr></tbody></table></td><td style='padding: 0px;'><a class='sc-iRbamj blSEcj' href='mailto:bbrizolara7@gmail.com' color='#0710C6' style='text-decoration: none; color: rgb(7, 16, 198); font-size: 12px;'><span>bbrizolara7@gmail.com</span></a></td></tr><tr height='25' style='vertical-align: middle;'><td width='30' style='vertical-align: middle;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;;'><tbody><tr><td style='vertical-align: bottom;'><span width='11' class='sc-gPEVay eQYmiW' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'><img width='12' class='sc-jDwBTQ dWtMUn' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/link-icon-2x.png' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'></span></td></tr></tbody></table></td><td style='padding: 0px;'><a class='sc-iRbamj blSEcj' href='https://britanico.com.uy' color='#0710C6' style='text-decoration: none; color: rgb(7, 16, 198); font-size: 12px;'><span>https://britanico.com.uy</span></a></td></tr><tr height='25' style='vertical-align: middle;'><td width='30' style='vertical-align: middle;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;;'><tbody><tr><td style='vertical-align: bottom;'><span width='11' class='sc-gPEVay eQYmiW' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'><img width='12' class='sc-jDwBTQ dWtMUn' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/address-icon-2x.png' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'></span></td></tr></tbody></table></td><td style='padding: 0px;'><span class='sc-jlyJG bbyJzT' color='#0710C6' style='font-size: 12px; color: rgb(7, 16, 198);'><span>Joaquin Suarez 526, Rivera</span></span></td></tr></tbody></table></td></tr></tbody>";
            pieHtml += "</span></td></tr></tbody></table></td><td style='padding: 0px; color: rgb(7, 16, 198);'><a class='sc-iRbamj blSEcj' href='tel:462-24260' color='#0710C6' style='text-decoration: none; color: rgb(7, 16, 198); font-size: 12px;'><span> 462-24260</span></a></td></tr><tr height='25' style='vertical-align: middle;'><td width='30' style='vertical-align: middle;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;;'><tbody><tr><td style='vertical-align: bottom;'><span width='11' class='sc-gPEVay eQYmiW' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'><img width='0' class='sc-jDwBTQ dWtMUn' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/email-icon-2x.png' color='#e4003c' style='display: none !important; background-color: rgb(228, 0, 60); visibility: hidden !important; opacity: 0 !important; background-position: 12px 0px;' height='0'></span></td></tr></tbody></table></td><td style='padding: 0px;'><a class='sc-iRbamj blSEcj' href='mailto:bbrizolara7@gmail.com' color='#0710C6' style='text-decoration: none; color: rgb(7, 16, 198); font-size: 12px;'><span>bbrizolara7@gmail.com</span></a></td></tr><tr height='25' style='vertical-align: middle;'><td width='30' style='vertical-align: middle;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;;'><tbody><tr><td style='vertical-align: bottom;'><span width='11' class='sc-gPEVay eQYmiW' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'><img width='12' class='sc-jDwBTQ dWtMUn' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/link-icon-2x.png' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'></span></td></tr></tbody></table></td><td style='padding: 0px;'><a class='sc-iRbamj blSEcj' href='https://britanico.com.uy' color='#0710C6' style='text-decoration: none; color: rgb(7, 16, 198); font-size: 12px;'><span> https://britanico.com.uy </span></a></td></tr><tr height='25' style='vertical-align: middle;'><td width='30' style='vertical-align: middle;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;;'><tbody><tr><td style='vertical-align: bottom;'><span width='11' class='sc-gPEVay eQYmiW' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'><img width='12' class='sc-jDwBTQ dWtMUn' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/address-icon-2x.png' color='#e4003c' style='display: block; background-color: rgb(228, 0, 60);'></span></td></tr></tbody></table></td><td style='padding: 0px;'><span class='sc-jlyJG bbyJzT' color='#0710C6' style='font-size: 12px; color: rgb(7, 16, 198);'><span> Joaquin Suarez 526, Rivera</span></span></td></tr></tbody></table></td></tr></tbody>";
            pieHtml += "</table></td ></tr><tr><td>";
            pieHtml += "<table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;; width: 100%;'><tbody><tr><td height = '30' ></td></tr><tr><td height = '1' class='sc-bRBYWo ccSRck' color='#e4003c' direction='horizontal' style='width: 100%; border-bottom: 1px solid rgb(228, 0, 60); border-left: none; display: block;'></td></tr><tr><td height = '30'></td></tr></tbody></table></td></tr>";
            pieHtml += "<tr><td><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;; width: 100%;'><tbody><tr><td style='vertical-align: top;'></td><td style='text-align: right; vertical-align: top;'><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;; display: inline-block;'><tbody><tr style='text-align: right;'><td><a class='sc-Rmtcm gwGgYM' href='https://www.facebook.com/instituto.britanico.98' color='#5757bf' style='display: inline-block; padding: 0px; background-color: rgb(87, 87, 191);'><img height='0' class='sc-csuQGl CQhxV' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/facebook-icon-2x.png' alt='facebook' color='#5757bf' style='background-color: rgb(87, 87, 191); max-width: 135px; display: none !important; visibility: hidden !important; opacity: 0 !important; background-position: 0px 24px;' width='0'></a></td><td width='5'></td><td><a class='sc-Rmtcm gwGgYM' href='https://www.instagram.com/institutobritanico_rivera' color='#5757bf' style='display: inline-block; padding: 0px; background-color: rgb(87, 87, 191);'><img height='0' class='sc-csuQGl CQhxV' src='https://cdn2.hubspot.net/hubfs/53/tools/email-signature-generator/icons/instagram-icon-2x.png' alt='instagram' color='#5757bf' style='background-color: rgb(87, 87, 191); max-width: 135px; display: none !important; visibility: hidden !important; opacity: 0 !important; background-position: 0px 24px;' width='0'></a></td><td width='5'></td></tr></tbody></table></td></tr></tbody></table></td></tr>";
            pieHtml += "<tr><td><table class='sc-jAaTju fNCBho' cellpadding='0' cellspacing='0' style='vertical-align: -webkit-baseline-middle; font-size: medium; font-family: &quot;Trebuchet MS&quot;; width: 100%;'><tbody><tr><td height='15'></td></tr><tr><td style='text-align: right;'><span style='display: block; text-align: right;'><a class='sc-dVhcbM fghLuF' rel='noopener noreferrer' href='' color='#37c760' style='border-width: 6px 12px; border-style: solid; border-color: rgb(55, 199, 96); display: inline-block; background-color: rgb(55, 199, 96); color: rgb(255, 255, 255); font-weight: 700; text-decoration: none; text-align: center; line-height: 40px; font-size: 12px; border-radius: 3px;'>Environmental Sustainability starts at home, think before you print!   Antes de imprimir este documento piense bien si es necesario hacerlo, el árbol que servirá para hacer el papel tardará 7 años en crecer.</a></span></td></tr></tbody></table></td></tr>";
            pieHtml += "</tbody></table>";
            return pieHtml;
        }

        private string ArmarCuerpoHtml()
        {
            string cuerpoHtml = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>";
            cuerpoHtml += "<html xmlns='http://www.w3.org/1999/xhtml'>";
            cuerpoHtml += "<head>";
            cuerpoHtml += "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />";
            cuerpoHtml += "<meta name='viewport' content='width=device-width, initial-scale=1.0'/></head>";
            cuerpoHtml += "<body>";
            cuerpoHtml += "<br/>";
            cuerpoHtml += this.CuerpoHTML;
            cuerpoHtml += "<br/>";
            cuerpoHtml += "<br/>";
            cuerpoHtml += "<br/>";
            cuerpoHtml += "</body></html>";
            cuerpoHtml += this.ObtenerPieEmailHtml();
            return cuerpoHtml;
        }
        
        public static bool ValidarEmail(Email email)
        {
            string errorMsg = String.Empty;
            if (email.DestinatarioEmail.Equals(String.Empty))
            {
                errorMsg = "Destinatario no puede ser vacio \n";
            }
            if (email.Asunto.Equals(String.Empty))
            {
                errorMsg += "Asunto no puede ser vacio \n";
            }
            if (email.CuerpoHTML.Equals(String.Empty))
            {
                errorMsg += "Contenido del email no puede ser vacio \n";
            }
            if (errorMsg != String.Empty)
            {
                throw new ValidacionException(errorMsg);
            }
            return true;
        }

        public static bool ExisteEmail(Email email, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool ok = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            SqlDataReader reader = null;
            string sql = "";
            if (email.ID > 0)
            {
                sql = "SELECT * FROM Email WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", email.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Email");
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
                sql = "SELECT * FROM Email WHERE ID = @ID";
                lstParametros.Add(new SqlParameter("@ID", this.ID));
            }
            else
            {
                throw new ValidacionException("Datos insuficientes para buscar al Email");
            }
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    this.ID = Convert.ToInt32(reader["ID"]);
                    this.DestinatarioEmail = reader["DestinatarioEmail"].ToString();
                    this.DestinatarioNombre = reader["DestinatarioNombre"].ToString();
                    this.Asunto = reader["Asunto"].ToString();
                    this.CuerpoHTML = reader["CuerpoHTML"].ToString();
                    this.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    this.Enviado = Convert.ToBoolean(reader["Enviado"]);
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
                string sql = "INSERT INTO Email VALUES (@DestinatarioEmail, @DestinatarioNombre, @Asunto, @CuerpoHTML, @FechaHora, @Enviado); SELECT CAST (SCOPE_IDENTITY() AS INT);";
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
            string sql = "UPDATE Email SET DestinatarioEmail = @DestinatarioEmail, DestinatarioNombre = @DestinatarioNombre, Asunto = @Asunto, CuerpoHTML = @CuerpoHTML, FechaHora = @FechaHora, Enviado = @Enviado WHERE ID = @ID;";
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
            string sql = "DELETE FROM Email WHERE ID = @ID";
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

        public static bool EliminarEntreFechas(string strCon, DateTime desde, DateTime hasta)
        {
            SqlConnection con = new SqlConnection(strCon);
            bool seBorro = false;
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@desde", desde));
            lstParametros.Add(new SqlParameter("@hasta", hasta));
            string sql = "DELETE FROM Email WHERE FechaHora >= @desde AND FechaHora <= @hasta";
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

        public List<Email> GetAll(string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Email> lstEmail = new List<Email>();
            string sql = "SELECT * FROM Email;";
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, null, CommandType.Text);
                while (reader.Read())
                {
                    Email email = new Email();
                    email.ID = Convert.ToInt32(reader["ID"]);
                    email.DestinatarioEmail = reader["DestinatarioEmail"].ToString();
                    email.DestinatarioNombre = reader["DestinatarioNombre"].ToString();
                    email.Asunto = reader["Asunto"].ToString();
                    email.CuerpoHTML = reader["CuerpoHTML"].ToString();
                    email.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    email.Enviado = Convert.ToBoolean(reader["Enviado"]);
                    lstEmail.Add(email);
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
            return lstEmail;
        }

        public List<Email> GetEntreFechas(DateTime desde, DateTime hasta, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Email> lstEmail = new List<Email>();
            string sql = "SELECT * FROM Email WHERE FechaHora >= @Desde AND FechaHora <= @Hasta;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Desde", desde));
            lstParametros.Add(new SqlParameter("@Hasta", hasta));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Email email = new Email();
                    email.ID = Convert.ToInt32(reader["ID"]);
                    email.DestinatarioEmail = reader["DestinatarioEmail"].ToString();
                    email.DestinatarioNombre = reader["DestinatarioNombre"].ToString();
                    email.Asunto = reader["Asunto"].ToString();
                    email.CuerpoHTML = reader["CuerpoHTML"].ToString();
                    email.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    email.Enviado = Convert.ToBoolean(reader["Enviado"]);
                    lstEmail.Add(email);
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
            return lstEmail;
        }

        public List<Email> GetPendientesEntreFechas(DateTime desde, DateTime hasta, string strCon)
        {
            SqlConnection con = new SqlConnection(strCon);
            List<Email> lstEmail = new List<Email>();
            string sql = "SELECT * FROM Email WHERE FechaHora >= @Desde AND FechaHora <= @Hasta AND Enviado = 0;";
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@Desde", desde));
            lstParametros.Add(new SqlParameter("@Hasta", hasta));
            SqlDataReader reader = null;
            try
            {
                con.Open();
                reader = Persistencia.EjecutarConsulta(con, sql, lstParametros, CommandType.Text);
                while (reader.Read())
                {
                    Email email = new Email();
                    email.ID = Convert.ToInt32(reader["ID"]);
                    email.DestinatarioEmail = reader["DestinatarioEmail"].ToString();
                    email.DestinatarioNombre = reader["DestinatarioNombre"].ToString();
                    email.Asunto = reader["Asunto"].ToString();
                    email.CuerpoHTML = reader["CuerpoHTML"].ToString();
                    email.FechaHora = Convert.ToDateTime(reader["FechaHora"]);
                    email.Enviado = Convert.ToBoolean(reader["Enviado"]);
                    lstEmail.Add(email);
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
            return lstEmail;
        }


        public override List<SqlParameter> ObtenerParametros()
        {
            List<SqlParameter> lstParametros = new List<SqlParameter>();
            lstParametros.Add(new SqlParameter("@ID", this.ID));
            lstParametros.Add(new SqlParameter("@DestinatarioEmail", this.DestinatarioEmail));
            lstParametros.Add(new SqlParameter("@DestinatarioNombre", this.DestinatarioNombre));
            lstParametros.Add(new SqlParameter("@Asunto", this.Asunto));
            lstParametros.Add(new SqlParameter("@CuerpoHTML", this.CuerpoHTML));
            lstParametros.Add(new SqlParameter("@FechaHora", this.FechaHora));
            lstParametros.Add(new SqlParameter("@Enviado", this.Enviado));
            return lstParametros;
        }

        public bool LeerLazy(string strCon)
        {
            //no necesita lazy
            return this.Leer(strCon);
        }

        public List<Email> GetAllLazy(string strCon)
        {
            return this.GetAll(strCon);
        }


        #endregion

        

    }
}
