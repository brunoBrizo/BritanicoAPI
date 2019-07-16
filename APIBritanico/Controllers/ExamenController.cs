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
    public class ExamenController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/examen/getbyid/1,1
        [HttpGet("{id:int},{grupoID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Examen> GetById(int id, int grupoID)
        {
            try
            {
                if (id > 0 && grupoID > 0)
                {
                    Grupo grupo = new Grupo
                    {
                        ID = grupoID
                    };
                    Examen examen = new Examen
                    {
                        ID = id,
                        GrupoID = grupoID
                    };
                    examen.Grupo = grupo;
                    examen = Fachada.GetExamen(examen);
                    if (examen == null)
                    {
                        return BadRequest("No existe el examen");
                    }
                    return examen;
                }
                else
                {
                    return BadRequest("ID y GrupoID no pueden ser vacios");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/examen/GetByGrupoAnio/2020,1
        [HttpGet("{anio:int},{grupoID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Examen> GetByGrupoAnio(int anio, int grupoID)
        {
            try
            {
                if (anio > 2000 && grupoID > 0)
                {
                    Grupo grupo = new Grupo
                    {
                        ID = grupoID
                    };
                    Examen examen = new Examen
                    {
                        ID = 0,
                        GrupoID = grupoID,
                        AnioAsociado = anio
                    };
                    examen.Grupo = grupo;
                    examen = Fachada.GetExamen(examen);
                    if (examen == null)
                    {
                        return BadRequest("No existe el examen");
                    }
                    return examen;
                }
                else
                {
                    return BadRequest("Año y Grupo invalidos");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/examen/getallbyanio/
        [HttpGet("{anio:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Examen>> GetAllByAnio(int anio)
        {
            try
            {
                if (anio < 2000 || anio > 3000)
                {
                    return BadRequest("Año invalido");
                }
                Examen examen = new Examen
                {
                    AnioAsociado = anio
                };
                List<Examen> lstExamenes = Fachada.ObtenerExamenesByAnioAsociado(examen);
                return lstExamenes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/examen/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Examen>> GetAll()
        {
            try
            {
                List<Examen> lstExamenes = Fachada.ObtenerExamenes();
                return lstExamenes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/examen/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Examen> Crear([FromBody]Examen data)
        {
            try
            {
                Examen examen = (Examen)data;
                if (examen == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (examen.Grupo == null)
                {
                    examen.Grupo = new Grupo();
                }
                examen.Grupo.ID = examen.GrupoID;
                examen.Grupo.Materia.ID = examen.MateriaID;
                examen = Fachada.CrearExamen(examen);
                if (examen == null)
                {
                    return BadRequest("No se creo el Examen");
                }
                else
                {
                    return examen;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/examen/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Examen data)
        {
            try
            {
                Examen examen = (Examen)data;
                if (examen == null || examen.ID < 1 || examen.GrupoID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                examen.Grupo.ID = examen.GrupoID;
                examen.Grupo.Materia.ID = examen.MateriaID;
                if (Fachada.ModificarExamen(examen))
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


        //// DELETE api/examen/eliminar/1,1
        [HttpDelete("{id:int},{grupoID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Eliminar(int id, int grupoID)
        {
            try
            {
                if (id < 1 || grupoID < 1)
                {
                    return BadRequest("ID y GrupoID no pueden ser vacios");
                }
                Examen examen = new Examen
                {
                    ID = id,
                    GrupoID = grupoID
            };
                Grupo grupo = new Grupo
                {
                    ID = grupoID
                };
                examen.Grupo = grupo;
                if (Fachada.EliminarExamen(examen))
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
