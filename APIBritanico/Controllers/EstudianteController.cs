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
    public class EstudianteController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/estudiante/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Estudiante> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = id
                    };
                    estudiante = Fachada.GetEstudiante(estudiante);
                    if (estudiante == null)
                    {
                        return BadRequest("No existe el estudiante");
                    }
                    return estudiante;
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


        //// GET: api/estudiante/getbycedula/131515
        [HttpGet("{ci}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Estudiante> GetByCedula(string ci)
        {
            try
            {
                if (!ci.Equals(String.Empty))
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = 0,
                        CI = ci
                    };
                    estudiante = Fachada.GetEstudiante(estudiante);
                    if (estudiante == null)
                    {
                        return BadRequest("No existe el estudiante");
                    }
                    return estudiante;
                }
                else
                {
                    return BadRequest("Cedula no puede ser vacía");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetAll()
        {
            try
            {
                List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudiantes();
                return lstEstudiantes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/getbynombre/
        [HttpGet("{nombre}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetByNombre(string nombre)
        {
            try
            {
                if (nombre.Equals(String.Empty))
                {
                    return BadRequest("Nombre no puede ser vacío");
                }
                Estudiante estudiante = new Estudiante
                {
                    ID = 0,
                    Nombre = nombre
                };
                List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudianteByNombre(estudiante);
                return lstEstudiantes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/getallactivos/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetAllActivos()
        {
            try
            {
                List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudiantesActivos();
                return lstEstudiantes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/estudiante/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Estudiante> Crear([FromBody]Estudiante data)
        {
            try
            {
                Estudiante estudiante = (Estudiante)data;
                if (estudiante == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                estudiante = Fachada.CrearEstudiante(estudiante);
                if (estudiante == null)
                {
                    return BadRequest("No se creo el estudiante");
                }
                else
                {
                    return estudiante;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/estudiante/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Estudiante data)
        {
            try
            {
                Estudiante estudiante = (Estudiante)data;
                if (estudiante == null || estudiante.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (Fachada.ModificarEstudiante(estudiante))
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


        //// DELETE api/estudiante/eliminar/
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
                Estudiante estudiante = new Estudiante
                {
                    ID = id
                };
                if (Fachada.EliminarEstudiante(estudiante))
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
