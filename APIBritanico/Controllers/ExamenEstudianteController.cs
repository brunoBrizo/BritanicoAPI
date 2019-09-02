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


        //// GET: api/examenEstudiante/getbyid/1,1,1,1
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
                        ID = examenID
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


        //// GET: api/examenEstudiante/GetByEstudiante/1
        [HttpGet("{estudianteID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ExamenEstudiante>> GetNoPagosByEstudiante(int estudianteID)
        {
            try
            {
                if (estudianteID > 0)
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = estudianteID
                    };
                    List<ExamenEstudiante> lstExamenEstudiante = Fachada.GetExamenesNoPagosByEstudiante(estudiante);
                    if (lstExamenEstudiante == null || lstExamenEstudiante.Count < 1)
                    {
                        return BadRequest("No existen examenes sin pagar");
                    }
                    return lstExamenEstudiante;
                }
                else
                {
                    return BadRequest("ID de estudiante no puede ser vacío");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/examenEstudiante/GetActualByEstudiante/1
        [HttpGet("{estudianteID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ExamenEstudiante>> GetActualByEstudiante(int estudianteID)
        {
            try
            {
                if (estudianteID > 0)
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = estudianteID
                    };
                    List<ExamenEstudiante> lstExamenEstudiante = Fachada.GetExamenesActualesByEstudiante(estudiante);
                    if (lstExamenEstudiante == null || lstExamenEstudiante.Count < 1)
                    {
                        return BadRequest("No existen examenes sin pagar");
                    }
                    return lstExamenEstudiante;
                }
                else
                {
                    return BadRequest("ID de estudiante no puede ser vacío");
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


        //// GET: api/examenEstudiante/GetAllByExamen/1,1
        [HttpGet("{examenID:int},{grupoID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<ExamenEstudiante>> GetAllByExamen(int examenID, int grupoID)
        {
            try
            {
                Grupo grupo = new Grupo
                {
                    ID = grupoID
                };
                Examen examen = new Examen
                {
                    ID = examenID,
                    GrupoID = grupoID,
                    Grupo = grupo
                };
                List<ExamenEstudiante> lstExamenes = Fachada.GetExamenEstudianteByExamen(examen);
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


        //// PUT api/examenEstudiante/ModificarByLista/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> AsignarResultado([FromBody]List<ExamenEstudiante> lstData)
        {
            try
            {
                List<ExamenEstudiante> lstExamenEstudiante = (List<ExamenEstudiante>)lstData;
                if (lstExamenEstudiante == null || lstExamenEstudiante.Count < 1)
                {
                    return BadRequest("Debe enviar la lista de exámenes");
                }

                foreach (ExamenEstudiante examenEstudiante in lstExamenEstudiante)
                {
                    if (examenEstudiante.Examen == null || examenEstudiante.Examen.ID < 1 || examenEstudiante.ID < 1 ||
                    examenEstudiante.Examen.GrupoID < 1 || examenEstudiante.Estudiante.ID < 1)
                    {
                        return BadRequest("Datos de los exámenes no son válidos");
                    }
                }

                if (Fachada.AsignarResultadoListaExamenEstudiante(lstExamenEstudiante))
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


        //// PUT api/examenEstudiante/ModificarByLista/
        [HttpPut("{id:int},{examenID:int},{grupoID:int},{idCuota:int},{precio:int},{estudianteID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> PagarCuota(int id, int examenID, int grupoID, int idCuota, decimal precio, int estudianteID)
        {
            try
            {
                if (id < 1 || examenID < 1 || grupoID < 1 || idCuota < 1 || precio < 1)
                {
                    return BadRequest("Debe enviar los datos de la cuota");
                }
                Examen examen = new Examen
                {
                    ID = examenID,
                    GrupoID = grupoID
                };
                ExamenEstudiante examenEstudiante = new ExamenEstudiante
                {
                    ID = id,
                    Examen = examen
                };
                examenEstudiante.LstCuotas.Add(new ExamenEstudianteCuota
                {
                    ID = idCuota,
                    Precio = precio
                });
                Estudiante estudiante = new Estudiante()
                {
                    ID = estudianteID
                };
                examenEstudiante.Estudiante = estudiante;

                if (Fachada.PagarCuotaExamenEstudiante(examenEstudiante))
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
