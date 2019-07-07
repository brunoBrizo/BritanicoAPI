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
    public class LibroController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/libro/getbyid/1
        [HttpGet("{id:int},{materiaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Libro> GetById(int id, int materiaId)
        {
            try
            {
                if (id > 0 && materiaId > 0)
                {
                    Materia materia = new Materia
                    {
                        ID = materiaId
                    };
                    Libro libro = new Libro
                    {
                        ID = id,
                        Materia = materia
                    };
                    libro = Fachada.GetLibro(libro);
                    if (libro == null)
                    {
                        return BadRequest("No existe el libro");
                    }
                    return libro;
                }
                else
                {
                    return BadRequest("ID de Libro y Materia no puede ser vacio");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/libro/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Libro>> GetAll()
        {
            try
            {
                List<Libro> lstLibros = Fachada.ObtenerLibros();
                return lstLibros;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/libro/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Libro> Crear([FromBody]Libro data)
        {
            try
            {
                Libro libro = (Libro)data;
                if (libro == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                libro = Fachada.CrearLibro(libro);
                if (libro == null)
                {
                    return BadRequest("No se creo el libro");
                }
                else
                {
                    return libro;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/libro/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Libro data)
        {
            try
            {
                Libro libro = (Libro)data;
                if (libro == null || libro.ID < 1 || libro.Materia.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (Fachada.ModificarLibro(libro))
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


        //// DELETE api/libro/eliminar/
        [HttpDelete("{id:int},{materiaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Eliminar(int id, int materiaId)
        {
            try
            {
                if (id < 1 || materiaId < 1)
                {
                    return BadRequest("ID no puede ser vacio");
                }
                Materia materia = new Materia
                {
                    ID = materiaId
                };
                Libro libro = new Libro
                {
                    ID = id,
                    Materia = materia
                };
                if (Fachada.EliminarLibro(libro))
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
