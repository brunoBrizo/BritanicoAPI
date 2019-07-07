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
    public class MateriaController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/materia/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Materia> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Materia materia = new Materia
                    {
                        ID = id
                    };
                    materia = Fachada.GetMateria(materia);
                    if (materia == null)
                    {
                        return BadRequest("No existe la materia");
                    }
                    return materia;
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


        //// GET: api/materia/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Materia>> GetAll()
        {
            try
            {
                List<Materia> lstMaterias = Fachada.ObtenerMaterias();
                return lstMaterias;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/materia/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Materia> Crear([FromBody]Materia data)
        {
            try
            {
                Materia materia = (Materia)data;
                if (materia == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                Sucursal sucursal = new Sucursal
                {
                    ID = materia.SucursalID
                };
                materia.Sucursal = sucursal;
                materia = Fachada.CrearMateria(materia);
                if (materia == null)
                {
                    return BadRequest("No se creo la materia");
                }
                else
                {
                    return materia;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/materia/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Materia data)
        {
            try
            {
                Materia materia = (Materia)data;
                if (materia == null || materia.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                Sucursal sucursal = new Sucursal
                {
                    ID = materia.SucursalID
                };
                materia.Sucursal = sucursal;
                if (Fachada.ModificarMateria(materia))
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


        //// DELETE api/materia/eliminar/
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
                Materia materia = new Materia
                {
                    ID = id
                };
                if (Fachada.EliminarMateria(materia))
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
