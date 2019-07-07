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
    public class ExamenEstudianteController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/examenEstudiante/getbyid/1
        [HttpGet("{id:int},{examenID:int},{grupoID:int},{estudianteID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ExamenEstudiante> GetById(int id, int examenID, int grupoID, int estudianteID)
        {
            try
            {
                if (id > 0 && grupoID > 0 && examenID > 0 && estudianteID > 0)
                {
                    Grupo grupo = new Grupo
                    {
                        ID = grupoID
                    };
                    Examen examen = new Examen
                    {
                        ID = id
                    };
                    Estudiante estudiante = new Estudiante
                    {
                        ID = estudianteID
                    };
                    examen.Grupo = grupo;
                    examen.GrupoID = grupoID;
                    ExamenEstudiante examenEstudiante = new ExamenEstudiante
                    {
                        ID = id,
                        Examen = examen,
                        Estudiante = estudiante
                    };
                    examenEstudiante = Fachada.GetExamenEstudiante(examenEstudiante);
                    if (examenEstudiante == null)
                    {
                        return BadRequest("No existe el examen");
                    }
                    return examenEstudiante;
                }
                else
                {
                    return BadRequest("ID, examenID, grupoID y estudianteID no pueden ser vacios");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/examenEstudiante/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ExamenEstudiante>> GetAll()
        {
            try
            {
                List<ExamenEstudiante> lstExamenes = Fachada.ObtenerExamenEstudiantes();
                return lstExamenes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/examenEstudiante/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ExamenEstudiante> Crear([FromBody]ExamenEstudiante data)
        {
            try
            {
                ExamenEstudiante examenEstudiante = (ExamenEstudiante)data;
                if (examenEstudiante == null || examenEstudiante.Examen == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                examenEstudiante.Examen.Grupo = new Grupo
                {
                    ID = examenEstudiante.Examen.GrupoID
                };
                examenEstudiante.Funcionario = new Funcionario
                {
                    ID = examenEstudiante.FuncionarioID
                };
                examenEstudiante = Fachada.CrearExamenEstudiante(examenEstudiante);
                if (examenEstudiante == null)
                {
                    return BadRequest("No se creo el Examen");
                }
                else
                {
                    return examenEstudiante;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/examenEstudiante/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]ExamenEstudiante data)
        {
            try
            {
                ExamenEstudiante examenEstudiante = (ExamenEstudiante)data;
                if (examenEstudiante == null || examenEstudiante.Examen == null || examenEstudiante.Examen.ID < 1 || examenEstudiante.ID < 1 ||
                    examenEstudiante.Examen.GrupoID < 1 || examenEstudiante.Estudiante.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                examenEstudiante.Examen.Grupo = new Grupo
                {
                    ID = examenEstudiante.Examen.GrupoID
                };
                examenEstudiante.Funcionario = new Funcionario
                {
                    ID = examenEstudiante.FuncionarioID
                };
                if (Fachada.ModificarExamenEstudiante(examenEstudiante))
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


        //// DELETE api/examenEstudiante/eliminar/
        [HttpDelete("{id:int},{examenID:int},{grupoID:int},{estudianteID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Eliminar(int id, int examenID, int grupoID, int estudianteID)
        {
            try
            {
                if (id < 1 || grupoID < 1 || examenID < 1 || estudianteID < 1)
                {
                    return BadRequest("ID y GrupoID no pueden ser vacios");
                }
                Grupo grupo = new Grupo
                {
                    ID = grupoID
                };
                Examen examen = new Examen
                {
                    ID = id
                };
                Estudiante estudiante = new Estudiante
                {
                    ID = estudianteID
                };
                examen.Grupo = grupo;
                examen.GrupoID = grupoID;
                ExamenEstudiante examenEstudiante = new ExamenEstudiante
                {
                    ID = id,
                    Examen = examen,
                    Estudiante = estudiante
                };
                if (Fachada.EliminarExamenEstudiante(examenEstudiante))
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
