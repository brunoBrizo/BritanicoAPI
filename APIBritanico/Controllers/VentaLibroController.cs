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
    public class VentaLibroController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/ventalibro/getbyid/1
        [HttpGet("{id:int},{libroId:int},{materiaId:int},{estudianteId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VentaLibro> GetById(int id, int libroId, int materiaId, int estudianteId)
        {
            try
            {
                if (id > 0 && libroId > 0 && materiaId > 0 && estudianteId > 0)
                {
                    VentaLibro venta = new VentaLibro();
                    venta.ID = id;
                    venta.Libro.ID = libroId;
                    venta.Libro.Materia.ID = materiaId;
                    venta.Estudiante.ID = estudianteId;
                    venta = Fachada.GetVentaLibro(venta);
                    if (venta == null)
                    {
                        return BadRequest("No existe la venta");
                    }
                    return venta;
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


        //// GET: api/ventalibro/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<VentaLibro>> GetAll()
        {
            try
            {
                List<VentaLibro> lstVentas = Fachada.ObtenerVentaLibros();
                return lstVentas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/ventalibro/getbyestado/
        [HttpGet("{estado:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<VentaLibro>> GetByEstado(int estado)
        {
            try
            {
                VentaLibro venta = new VentaLibro
                {
                    Estado = (VentaLibroEstado)estado
                };
                List<VentaLibro> lstVentas = Fachada.ObtenerVentaLibrosByEstado(venta);
                return lstVentas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/ventalibro/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VentaLibro> Crear([FromBody]VentaLibro data)
        {
            try
            {
                VentaLibro venta = (VentaLibro)data;
                if (venta == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                venta = Fachada.CrearVentaLibro(venta);
                if (venta == null)
                {
                    return BadRequest("No se creo la venta");
                }
                else
                {
                    return venta;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/ventalibro/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]VentaLibro data)
        {
            try
            {
                VentaLibro venta = (VentaLibro)data;
                if (venta == null || venta.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (Fachada.ModificarVentaLibro(venta))
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


        //// DELETE api/ventalibro/eliminar/
        [HttpDelete("{id:int},{libroId:int},{materiaId:int},{estudianteId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Eliminar(int id, int libroId, int materiaId, int estudianteId)
        {
            try
            {
                if (id < 1 || libroId < 1 || materiaId < 1 || estudianteId < 1)
                {
                    return BadRequest("ID no puede ser vacio");
                }
                VentaLibro venta = new VentaLibro();
                venta.ID = id;
                venta.Libro.ID = libroId;
                venta.Libro.Materia.ID = materiaId;
                venta.Estudiante.ID = estudianteId;
                if (Fachada.EliminarVentaLibro(venta))
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
