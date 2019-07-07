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
    public class MatriculaController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/matricula/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Matricula> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Matricula matricula = new Matricula
                    {
                        ID = id
                    };
                    matricula = Fachada.GetMatricula(matricula);
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


        //// GET: api/matricula/getbyanio/2019,1
        [HttpGet("{anio:int},{sucursalID:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Matricula> GetByAnio(int anio, int sucursalID)
        {
            try
            {
                if (anio < 2000 || anio > 3000)
                {
                    return BadRequest("Año invalido");
                }
                Matricula matricula = new Matricula
                {
                    ID = 0,
                    Anio = anio,
                    SucursalID = sucursalID
                };
                matricula = Fachada.GetMatricula(matricula);
                return matricula;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/matricula/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Matricula>> GetAll()
        {
            try
            {
                List<Matricula> lstMatriculas = Fachada.ObtenerMatriculas();
                return lstMatriculas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/matricula/getallbyanio/2019
        [HttpGet("{anio:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Matricula>> GetAllByAnio(int anio)
        {
            try
            {
                if (anio < 2000 || anio > 3000)
                {
                    return BadRequest("Año invalido");
                }
                List<Matricula> lstMatriculas = Fachada.ObtenerMatriculasByAnio(anio);
                return lstMatriculas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/matricula/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Matricula> Crear([FromBody]Matricula data)
        {
            try
            {
                Matricula matricula = (Matricula)data;
                if (matricula == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                Sucursal sucursal = new Sucursal
                {
                    ID = matricula.SucursalID
                };
                matricula.Sucursal = sucursal;
                matricula = Fachada.CrearMatricula(matricula);
                if (matricula == null)
                {
                    return BadRequest("No se creo la matricula");
                }
                else
                {
                    return matricula;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/matricula/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Matricula data)
        {
            try
            {
                Matricula matricula = (Matricula)data;
                if (matricula == null || matricula.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                Sucursal sucursal = new Sucursal
                {
                    ID = matricula.SucursalID
                };
                matricula.Sucursal = sucursal;
                if (Fachada.ModificarMatricula(matricula))
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


        //// DELETE api/matricula/eliminar/
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
                Matricula matricula = new Matricula
                {
                    ID = id
                };
                if (Fachada.EliminarMatricula(matricula))
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
