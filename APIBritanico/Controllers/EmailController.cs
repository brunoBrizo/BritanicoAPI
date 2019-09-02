using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibliotecaBritanico.Fachada;
using BibliotecaBritanico.Modelo;
using BibliotecaBritanico.Utilidad;
using Microsoft.AspNetCore.Http;


namespace APIBritanico.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmailController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/email/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Email> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Email email = new Email
                    {
                        ID = id
                    };
                    email = Fachada.GetEmail(email);
                    if (email == null)
                    {
                        return BadRequest("No existe el email");
                    }
                    return email;
                }
                else
                {
                    return BadRequest("ID no puede ser vacio");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/email/getentrefechas/desde,hasta
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Email>> GetEntreFechas(DateTime desde, DateTime hasta)
        {
            try
            {
                if (desde <= DateTime.MinValue || hasta <= DateTime.MinValue)
                    return BadRequest("Fechas invalidas");
                if (desde > hasta)
                    return BadRequest("Fecha desde no puede ser mayor a fecha hasta");

                List<Email> lstEmails = Fachada.ObtenerEmailsEntreFechas(desde, hasta);
                return lstEmails;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/email/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Email>> GetAll()
        {
            try
            {
                List<Email> lstEmails = Fachada.ObtenerEmails();
                return lstEmails;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/email/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> Crear([FromBody]Email data)
        {
            try
            {
                Email email = (Email)data;
                if (email == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                email = await Fachada.CrearEmail(email);
                if (email == null)
                {
                    return BadRequest(false);
                }
                else
                {
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/email/EnviarMailPorGrupo/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> EnviarMailPorGrupo([FromBody]EmailPorGrupo data)
        {
            try
            {
                EmailPorGrupo emailPorGrupo = (EmailPorGrupo)data;
                if (emailPorGrupo == null)
                {
                    return BadRequest("Datos invalidos en el request");
                }
                if (emailPorGrupo.Grupo.ID < 1)
                {
                    return BadRequest("Debe enviar un grupo");
                }
                if (emailPorGrupo.Email.Asunto.Equals(String.Empty) || emailPorGrupo.Email.CuerpoHTML.Equals(String.Empty))
                {
                    return BadRequest("Debe ingresar asunto y un mensaje");
                }
                bool ret = await Fachada.EnviarMailPorGrupo(emailPorGrupo.Email, emailPorGrupo.Grupo);
                if (!ret)
                {
                    return BadRequest(ret);
                }
                else
                {
                    return Ok(ret);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/email/enviarpendientes/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> EnviarPendientes()
        {
            try
            {
                await Fachada.EnviarEmailsPendientes();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/email/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Email data)
        {
            try
            {
                Email email = (Email)data;
                if (email == null || email.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (Fachada.ModificarEmail(email))
                {
                    return true;
                }
                else
                {
                    return BadRequest(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// DELETE api/email/eliminar/
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Eliminar(int id)
        {
            try
            {
                if (id < 1)
                {
                    return BadRequest("ID no puede ser vacio");
                }
                Email email = new Email
                {
                    ID = id
                };
                if (Fachada.EliminarEmail(email))
                {
                    return true;
                }
                else
                {
                    return BadRequest(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
