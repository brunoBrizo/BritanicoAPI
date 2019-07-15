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
    public class MensualidadController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/mensualidad/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Mensualidad> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Mensualidad mensualidad = new Mensualidad
                    {
                        ID = id
                    };
                    mensualidad = Fachada.GetMensualidad(mensualidad);
                    if (mensualidad == null)
                    {
                        return BadRequest("No existe la mensualidad");
                    }
                    return mensualidad;
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


        //// GET: api/mensualidad/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Mensualidad>> GetAll()
        {
            try
            {
                List<Mensualidad> lstMensualidades = Fachada.ObtenerMensualidades();
                return lstMensualidades;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/mensualidad/getallbyestudiantegrupo/1,1,1
        [HttpGet("{estudianteID:int},{grupoID:int},{materiaID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Mensualidad>> GetAllByEstudianteGrupo(int estudianteID, int grupoID, int materiaID)
        {
            try
            {
                if (estudianteID < 1 || grupoID < 1 || materiaID < 1)
                {
                    return BadRequest("Datos invalidos en el request");
                }
                Mensualidad mensualidad = new Mensualidad
                {
                    ID = 0,
                    GrupoID = grupoID,
                    MateriaID = materiaID
                };
                Estudiante estudiante = new Estudiante
                {
                    ID = estudianteID
                };
                mensualidad.Estudiante = estudiante;
                List<Mensualidad> lstMensualidades = Fachada.ObtenerMensualidadesByEstudianteGrupo(mensualidad);
                return lstMensualidades;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/mensualidad/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Mensualidad> Crear([FromBody]Mensualidad data)
        {
            try
            {
                Mensualidad mensualidad = (Mensualidad)data;
                if (mensualidad == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                mensualidad.Grupo = new Grupo();
                mensualidad.Sucursal = new Sucursal();
                mensualidad.Funcionario = new Funcionario();
                mensualidad.Grupo.ID = mensualidad.GrupoID;
                mensualidad.Grupo.Materia.ID = mensualidad.MateriaID;
                mensualidad.Funcionario.ID = mensualidad.FuncionarioID;
                mensualidad.Sucursal.ID = mensualidad.SucursalID;
                mensualidad = Fachada.CrearMensualidad(mensualidad);
                if (mensualidad == null)
                {
                    return BadRequest("No se creo la mensualidad");
                }
                else
                {
                    return mensualidad;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/mensualidad/crearporanio/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Mensualidad>> CrearPorAnio([FromBody]List<Mensualidad> data)
        {
            try
            {
                List<Mensualidad> lstMensualidades = (List<Mensualidad>)data;
                if (lstMensualidades == null || lstMensualidades.Count < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                List<Mensualidad> lstMensualidadesRet = new List<Mensualidad>();
                foreach (Mensualidad mensualidad in lstMensualidades)
                {
                    mensualidad.Grupo = new Grupo();
                    mensualidad.Sucursal = new Sucursal();
                    mensualidad.Funcionario = new Funcionario();
                    mensualidad.Grupo.ID = mensualidad.GrupoID;
                    mensualidad.Grupo.Materia.ID = mensualidad.MateriaID;
                    mensualidad.Funcionario.ID = mensualidad.FuncionarioID;
                    mensualidad.Sucursal.ID = mensualidad.SucursalID;
                    lstMensualidadesRet.Add(Fachada.CrearMensualidad(mensualidad));
                }
                if (lstMensualidadesRet.Count < 1)
                {
                    return BadRequest("No se crearon las mensualidades");
                }
                else
                {
                    return lstMensualidadesRet;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/mensualidad/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Mensualidad data)
        {
            try
            {
                Mensualidad mensualidad = (Mensualidad)data;
                if (mensualidad == null || mensualidad.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                mensualidad.Grupo = new Grupo();
                mensualidad.Sucursal = new Sucursal();
                mensualidad.Funcionario = new Funcionario();
                mensualidad.Grupo.ID = mensualidad.GrupoID;
                mensualidad.Grupo.Materia.ID = mensualidad.MateriaID;
                mensualidad.Funcionario.ID = mensualidad.FuncionarioID;
                mensualidad.Sucursal.ID = mensualidad.SucursalID;
                if (Fachada.ModificarMensualidad(mensualidad))
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


        //// DELETE api/mensualidad/eliminar/
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
                Mensualidad mensualidad = new Mensualidad
                {
                    ID = id
                };
                if (Fachada.EliminarMensualidad(mensualidad))
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
