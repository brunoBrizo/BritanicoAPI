using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibliotecaBritanico.Fachada;
using BibliotecaBritanico.Utilidad;
using Microsoft.AspNetCore.Http;


namespace APIBritanico.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ParametroController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/parametro/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Parametro> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Parametro parametro = new Parametro
                    {
                        ID = id
                    };
                    parametro = Fachada.GetParametro(parametro);
                    if (parametro == null)
                    {
                        return BadRequest("No existe el parametro");
                    }
                    return parametro;
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


        //// GET: api/parametro/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Parametro>> GetAll()
        {
            try
            {
                List<Parametro> lstParametros = Fachada.ObtenerParametros();
                return lstParametros;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/parametro/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Parametro> Crear([FromBody]Parametro data)
        {
            try
            {
                Parametro parametro = (Parametro)data;
                if (parametro == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                parametro = Fachada.CrearParametro(parametro);
                if (parametro == null)
                {
                    return BadRequest("No se creo el parametro");
                }
                else
                {
                    return parametro;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //// PUT api/parametro/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Parametro data)
        {
            try
            {
                Parametro parametro = (Parametro)data;
                if (parametro == null || parametro.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (Fachada.ModificarParametro(parametro))
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


        //// DELETE api/parametro/eliminar/
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
                Parametro parametro = new Parametro
                {
                    ID = id
                };
                if (Fachada.EliminarParametro(parametro))
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
