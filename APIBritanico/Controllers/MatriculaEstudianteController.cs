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
    public class MatriculaEstudianteController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/matriculaestudiante/getbyid/1
        [HttpGet("{id:int},{matriculaID:int},{estudianteID:int},{grupoID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MatriculaEstudiante> GetById(int id, int matriculaID, int estudianteID, int grupoID)
        {
            try
            {
                if (id > 0 && matriculaID > 0 && estudianteID > 0 && grupoID > 0)
                {
                    MatriculaEstudiante matricula = new MatriculaEstudiante
                    {
                        ID = id
                    };
                    matricula.Matricula.ID = matriculaID;
                    matricula.Estudiante.ID = estudianteID;
                    matricula.Grupo.ID = grupoID;
                    matricula.GrupoID = grupoID;
                    matricula = Fachada.GetMatriculaEstudiante(matricula);
                    if (matricula == null)
                    {
                        return BadRequest("No existe la matricula");
                    }
                    return matricula;
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


        //// GET: api/matriculaestudiante/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<MatriculaEstudiante>> GetAll()
        {
            try
            {
                List<MatriculaEstudiante> lstMatriculas = Fachada.ObtenerMatriculaEstudiantes();
                return lstMatriculas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/matriculaestudiante/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<MatriculaEstudiante> Crear([FromBody]MatriculaEstudiante data)
        {
            try
            {
                MatriculaEstudiante matricula = (MatriculaEstudiante)data;
                if (matricula == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                matricula.Grupo.ID = matricula.GrupoID;
                matricula.Grupo.Materia.ID = matricula.MateriaID;
                matricula.Funcionario.ID = matricula.FuncionarioID;
                matricula = Fachada.CrearMatriculaEstudiante(matricula);
                if (matricula == null)
                {
                    return BadRequest("No se creo la sucursal");
                }
                else
                {
                    Estudiante estudiante = new Estudiante
                    {
                        GrupoID = matricula.GrupoID,
                        MateriaID = matricula.MateriaID,
                        ID = matricula.Estudiante.ID
                    };
                    if (Fachada.ModificarEstudianteGrupo(estudiante))
                        return matricula;
                    else
                        return BadRequest("Error al actualizar el Grupo del estudiante. DEBE hacerlo manualmente desde el listado de Estudiantes!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/matriculaestudiante/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]MatriculaEstudiante data)
        {
            try
            {
                MatriculaEstudiante matricula = (MatriculaEstudiante)data;
                if (matricula == null || matricula.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                matricula.Grupo.ID = matricula.GrupoID;
                matricula.Grupo.Materia.ID = matricula.MateriaID;
                matricula.Funcionario.ID = matricula.FuncionarioID;
                if (Fachada.ModificarMatriculaEstudiante(matricula))
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


        //// DELETE api/matriculaestudiante/eliminar/
        [HttpDelete("{id:int},{matriculaID:int},{estudianteID:int},{grupoID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Eliminar(int id, int matriculaID, int estudianteID, int grupoID)
        {
            try
            {
                if (id < 1 || matriculaID < 1 || estudianteID < 1 || grupoID < 1)
                {
                    return BadRequest("ID no puede ser vacio");
                }
                MatriculaEstudiante matricula = new MatriculaEstudiante
                {
                    ID = id
                };
                matricula.Matricula.ID = matriculaID;
                matricula.Estudiante.ID = estudianteID;
                matricula.Grupo.ID = grupoID;
                matricula.GrupoID = grupoID;
                if (Fachada.EliminarMatriculaEstudiante(matricula))
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
