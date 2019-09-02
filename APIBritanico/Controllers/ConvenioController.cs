using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibliotecaBritanico.Fachada;
using BibliotecaBritanico.Modelo;
using Microsoft.AspNetCore.Http;


namespace APIBritanico.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ConvenioController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/convenio/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Convenio> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Convenio convenio = new Convenio
                    {
                        ID = id
                    };
                    convenio = Fachada.GetConvenio(convenio);
                    if (convenio == null)
                    {
                        return BadRequest("No existe el convenio");
                    }
                    return convenio;
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


        //// GET: api/convenio/getmontoapagar/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<decimal> GetMontoAPagar(int id)
        {
            try
            {
                if (id > 0)
                {
                    Convenio convenio = new Convenio
                    {
                        ID = id
                    };
                    decimal monto = Fachada.GetMontoAPagarPorConvenio(convenio);
                    return Ok(monto);
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


        //// GET: api/convenio/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Convenio>> GetAll()
        {
            try
            {
                List<Convenio> lstConvenios = Fachada.ObtenerConvenios();
                return lstConvenios;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/convenio/getall/
        [HttpGet("{anio:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Convenio>> GetAllByAnio(int anio)
        {
            try
            {
                if (anio < 2000 || anio > 3000)
                {
                    return BadRequest("Año invalido");
                }
                List<Convenio> lstConvenios = Fachada.ObtenerConveniosByAnio(anio);
                return lstConvenios;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/convenio/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Convenio> Crear([FromBody]Convenio data)
        {
            try
            {
                Convenio convenio = (Convenio)data;
                if (convenio == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                convenio = Fachada.CrearConvenio(convenio);
                if (convenio == null)
                {
                    return BadRequest("No se creo el convenio");
                }
                else
                {
                    return convenio;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/convenio/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Convenio data)
        {
            try
            {
                Convenio convenio = (Convenio)data;
                if (convenio == null || convenio.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (Fachada.ModificarConvenio(convenio))
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


        //// DELETE api/convenio/eliminar/
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
                Convenio convenio = new Convenio
                {
                    ID = id
                };
                if (Fachada.EliminarConvenio(convenio))
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
