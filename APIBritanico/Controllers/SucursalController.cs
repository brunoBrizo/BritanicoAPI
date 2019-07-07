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
    public class SucursalController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/sucursal/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Sucursal> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Sucursal sucursal = new Sucursal
                    {
                        ID = id
                    };
                    sucursal = Fachada.GetSucursal(sucursal);
                    if (sucursal == null)
                    {
                        return BadRequest("No existe la sucursal");
                    }
                    return sucursal;
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


        //// GET: api/sucursal/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Sucursal>> GetAll()
        {
            try
            {
                List<Sucursal> lstSucursales = Fachada.ObtenerSucursales();
                return lstSucursales;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/sucursal/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Sucursal> Crear([FromBody]Sucursal data)
        {
            try
            {
                Sucursal sucursal = (Sucursal)data;
                if (sucursal == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                sucursal = Fachada.CrearSucursal(sucursal);
                if (sucursal == null)
                {
                    return BadRequest("No se creo la sucursal");
                }
                else
                {
                    return sucursal;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/sucursal/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Sucursal data)
        {
            try
            {
                Sucursal sucursal = (Sucursal)data;
                if (sucursal == null || sucursal.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (Fachada.ModificarSucursal(sucursal))
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


        //// DELETE api/sucursal/eliminar/
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
                Sucursal sucursal = new Sucursal
                {
                    ID = id
                };
                if (Fachada.EliminarSucursal(sucursal))
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
