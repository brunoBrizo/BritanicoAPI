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


        //// GET: api/estudiante/getexamenpendiente/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Examen>> GetExamenPendiente(int id)
        {
            try
            {
                if (id > 0)
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = id
                    };
                    List<Examen> lstExamenes = Fachada.GetExamenPendienteByEstudiante(estudiante);
                    if (lstExamenes == null || lstExamenes.Count < 1)
                    {
                        return BadRequest("No existen examenes pendientes");
                    }
                    return lstExamenes;
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


        //// GET: api/estudiante/GetExamenEstudianteCuota/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ExamenEstudiante> GetExamenEstudianteCuota(int id)
        {
            try
            {
                if (id > 0)
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = id
                    };
                    ExamenEstudiante examenEstudiante = Fachada.ObtenerExamenEstudianteCuotas(estudiante);
                    return Ok(examenEstudiante);
                }
                else
                {
                    return BadRequest("Id de estudiante no puede ser vacío");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/GetExamenEstudiantePorRendir/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ExamenEstudiante> GetExamenEstudiantePorRendir(int id)
        {
            try
            {
                if (id > 0)
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = id
                    };
                    ExamenEstudiante examenEstudiante = Fachada.GetExamenEstudiantePorRendir(estudiante);
                    if (examenEstudiante == null)
                    {
                        return BadRequest("El estudiante no esta anotado a ningun examen");
                    }
                    return examenEstudiante;
                }
                else
                {
                    return BadRequest("Id de estudiante no puede ser vacío");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/GetEscolaridad/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<DatosEscolaridad>> GetEscolaridad(int id)
        {
            try
            {
                if (id > 0)
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = id
                    };
                    List<DatosEscolaridad> lstDatosEstolaridad = Fachada.ObtenerEscolaridad(estudiante);
                    if (lstDatosEstolaridad.Count < 1)
                    {
                        return BadRequest("El estudiante no tiene datos registrados");
                    }
                    return lstDatosEstolaridad;
                }
                else
                {
                    return BadRequest("Id de estudiante no puede ser vacío");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/ExisteEstudiante/131515
        [HttpGet("{ci}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> ExisteEstudiante(string ci)
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
                    return Fachada.ExisteEstudiante(estudiante);
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


        //// GET: api/estudiante/GetConMensualidades/1,23232,2019
        [HttpGet("{id:int},{ci},{anioAsociado:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Estudiante> GetConMensualidades(int id, string ci, int anioAsociado)
        {
            try
            {
                if ((id > 0 || !ci.Equals(String.Empty)) && anioAsociado > 2000)
                {
                    Estudiante estudiante = new Estudiante
                    {
                        ID = id,
                        CI = ci
                    };
                    estudiante = Fachada.GetEstudianteConMensualidad(estudiante, anioAsociado);
                    if (estudiante == null)
                    {
                        return BadRequest("No existe el estudiante");
                    }
                    return estudiante;
                }
                else
                {
                    return BadRequest("Debe enviar ID o CI y el año de las matriculas");
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

        
        //// GET: api/estudiante/GetDeudores/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetDeudores()
        {
            try
            {
                List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudiantesDeudores();
                return lstEstudiantes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/getbynombre/qweqwe
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


        //// GET: api/estudiante/getbyconvenio/2323
        [HttpGet("{convenioID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetByConvenio(int convenioID)
        {
            try
            {
                if (convenioID < 1)
                {
                    return BadRequest("ID de convenio no puede ser vacío");
                }
                Convenio convenio = new Convenio
                {
                    ID = convenioID
                };
                List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudiantesByConvenio(convenio);
                return lstEstudiantes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/GetPublicidadCantidad/2323
        [HttpGet("{anio:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<PublicidadCantidad>> GetPublicidadCantidad(int anio)
        {
            try
            {
                if (anio < 2000)
                {
                    anio = 0;
                }
                List<PublicidadCantidad> lstPublicidad = Fachada.ObtenerPublicidadCantidad(anio);
                return Ok(lstPublicidad);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/getallconconvenio/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetAllConConvenio()
        {
            try
            {
                List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudiantesConConvenio();
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


        //// GET: api/estudiante/getallnoactivos/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetAllNoActivos()
        {
            try
            {
                List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudiantesNoActivos();
                return lstEstudiantes;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/estudiante/GetAllNoValidados/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Estudiante>> GetAllNoValidados()
        {
            try
            {
                List<Estudiante> lstEstudiantes = Fachada.ObtenerEstudiantesNoValidados();
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


        //este metodo es llamado por un servicio que se ejecuta automaticamente
        //// GET api/estudiante/ActualizarEstudiantesDeudores/
        [HttpGet]
        public void ActualizarEstudiantesDeudores()
        {
            Fachada.MarcarEstudianteComoDeudor();
        }


        //este metodo es llamado por un servicio que se ejecuta automaticamente
        //// GET api/estudiante/MarcarEstudiantesInactivosSinGrupoSinConvenio/
        [HttpGet]
        public void MarcarEstudiantesInactivosSinGrupoSinConvenio()
        {
            Fachada.MarcarEstudiantesInactivosSinGrupoSinConvenio();
        }


        //// POST api/estudiante/DarDeBaja/1,8
        [HttpPost("{id:int},{mes:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> DarDeBaja(int id, int mes)
        {
            try
            {
                if (id < 1 || mes < 1)
                {
                    return BadRequest("ID y mes no pueden ser vacíos");
                }
                Estudiante estudiante = new Estudiante
                {
                    ID = id
                };
                bool ret = Fachada.DarBajaEstudiante(estudiante, mes);
                return ret;
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
