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
    public class GrupoController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/grupo/getbyid/1,1
        [HttpGet("{id:int},{materiaID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Grupo> GetById(int id, int materiaID)
        {
            try
            {
                if (id > 0 && materiaID > 0)
                {
                    Materia materia = new Materia
                    {
                        ID = materiaID
                    };
                    Grupo grupo = new Grupo
                    {
                        ID = id,
                        MateriaID = materiaID
                    };
                    grupo.Materia = materia;
                    grupo = Fachada.GetGrupo(grupo);
                    if (grupo == null)
                    {
                        return BadRequest("No existe el grupo");
                    }
                    return grupo;
                }
                else
                {
                    return BadRequest("ID y MateriaID no pueden ser vacios");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/grupo/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Grupo>> GetAll()
        {
            try
            {
                List<Grupo> lstGrupos = Fachada.ObtenerGrupos();
                return lstGrupos;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/grupo/getallbyanio/
        [HttpGet("{anio:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Grupo>> GetAllByAnio(int anio)
        {
            try
            {
                if (anio < 2000 || anio > 3000)
                {
                    return BadRequest("Año invalido");
                }
                List<Grupo> lstGrupos = Fachada.ObtenerGruposByAnio(anio);
                return lstGrupos;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/grupo/getestudiantesbygrupo/
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetEstudiantesByGrupo(int id)
        {
            try
            {
                if (id > 0)
                {
                    Grupo grupo = new Grupo
                    {
                        ID = id
                    };
                    List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudiantesByGrupo(grupo);
                    return lstEstudiantes;
                }
                return BadRequest("ID no puede ser vacio");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/grupo/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Grupo> Crear([FromBody]Grupo data)
        {
            try
            {
                Grupo grupo = (Grupo)data;
                if (grupo == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                Materia materia = new Materia
                {
                    ID = grupo.MateriaID
                };
                grupo.Materia = materia;
                Funcionario funcionario = new Funcionario
                {
                    ID = grupo.FuncionarioID
                };
                grupo.Funcionario = funcionario;
                Sucursal sucursal = new Sucursal
                {
                    ID = grupo.SucursalID
                };
                grupo.Sucursal = sucursal;
                grupo = Fachada.CrearGrupo(grupo);
                if (grupo == null)
                {
                    return BadRequest("No se creo el grupo");
                }
                else
                {
                    return grupo;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/grupo/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Grupo data)
        {
            try
            {
                Grupo grupo = (Grupo)data;
                if (grupo == null || grupo.ID < 1 || grupo.MateriaID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                Materia materia = new Materia
                {
                    ID = grupo.MateriaID
                };
                grupo.Materia = materia;
                Funcionario funcionario = new Funcionario
                {
                    ID = grupo.FuncionarioID
                };
                grupo.Funcionario = funcionario;
                Sucursal sucursal = new Sucursal
                {
                    ID = grupo.SucursalID
                };
                grupo.Sucursal = sucursal;
                if (Fachada.ModificarGrupo(grupo))
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


        //// DELETE api/grupo/eliminar/1,1
        [HttpDelete("{id:int},{materiaID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Eliminar(int id, int materiaID)
        {
            try
            {
                if (id < 1 || materiaID < 1)
                {
                    return BadRequest("ID no puede ser vacio");
                }
                Materia materia = new Materia
                {
                    ID = materiaID
                };
                Grupo grupo = new Grupo
                {
                    ID = id,
                    MateriaID = materiaID
                };
                grupo.Materia = materia;
                if (Fachada.EliminarGrupo(grupo))
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
