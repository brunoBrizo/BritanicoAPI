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
    public class PagoController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/pago/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Pago> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Pago pago = new Pago
                    {
                        ID = id
                    };
                    pago = Fachada.GetPago(pago);
                    if (pago == null)
                    {
                        return BadRequest("No existe el pago");
                    }
                    return pago;
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


        //// GET: api/pago/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Pago>> GetAll()
        {
            try
            {
                List<Pago> lstPagos = Fachada.ObtenerPagos();
                return lstPagos;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST: api/pago/getall/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Pago>> GetAll([FromBody]FiltrosPago data)
        {
            try
            {
                FiltrosPago filtros = (FiltrosPago)data;
                if (filtros == null)
                {
                    return BadRequest("Datos invalidos en el request");
                }
                if (filtros.FechaDesde > DateTime.MinValue && filtros.FechaHasta > DateTime.MinValue)
                {
                    List<Pago> lstPagos = Fachada.ObtenerPagos(filtros.FechaDesde, filtros.FechaHasta, filtros.Concepto);
                    return lstPagos;
                }
                else
                {
                    return BadRequest("Fechas inválidas");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/pago/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Pago> Crear([FromBody]Pago data)
        {
            try
            {
                Pago pago = (Pago)data;
                if (pago == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                Sucursal sucursal = new Sucursal
                {
                    ID = pago.SucursalID
                };
                pago.Sucursal = sucursal;
                Funcionario funcionario = new Funcionario
                {
                    ID = pago.FuncionarioID
                };
                pago.Funcionario = funcionario;
                pago = Fachada.CrearPago(pago);
                if (pago == null)
                {
                    return BadRequest("No se creo el pago");
                }
                else
                {
                    return pago;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/pago/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Pago data)
        {
            try
            {
                Pago pago = (Pago)data;
                if (pago == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                Sucursal sucursal = new Sucursal
                {
                    ID = pago.SucursalID
                };
                pago.Sucursal = sucursal;
                Funcionario funcionario = new Funcionario
                {
                    ID = pago.FuncionarioID
                };
                pago.Funcionario = funcionario;
                if (Fachada.ModificarPago(pago))
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


        //// DELETE api/pago/eliminar/
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
                Pago pago = new Pago
                {
                    ID = id
                };
                if (Fachada.EliminarPago(pago))
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
