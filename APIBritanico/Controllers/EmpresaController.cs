using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibliotecaBritanico.Fachada;
using BibliotecaBritanico.Modelo;
using BibliotecaBritanico.Utilidad;
using Microsoft.AspNetCore.Http;


namespace APIBritanico.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmpresaController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/empresa/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Empresa> GetById(int id)
        {
            try
            {
                if (id < 1)
                {
                    return BadRequest("ID no puede ser vacio");
                }
                Empresa empresa = new Empresa
                {
                    ID = id
                };
                empresa = Fachada.GetEmpresa(empresa);
                if (empresa == null)
                {
                    return BadRequest("No existe la empresa");
                }
                return empresa;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/empresa/getbyrut/12121212
        [HttpGet("{rut}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Empresa> GetByRut(string rut)
        {
            try
            {
                Empresa empresa = new Empresa
                {
                    Rut = rut
                };
                empresa = Fachada.GetEmpresa(empresa);
                if (empresa == null)
                {
                    return NotFound("No existe la empresa");
                }
                return empresa;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/empresa/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Empresa>> GetAll()
        {
            try
            {
                List<Empresa> lstEmpresas = Fachada.ObtenerEmpresas();
                return lstEmpresas;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/empresa/crearempresa/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Empresa> Crear([FromBody]Empresa data)
        {
            try
            {
                Empresa empresa = (Empresa)data;
                if (empresa == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                empresa = Fachada.CrearEmpresa(empresa);
                if (empresa == null)
                {
                    return BadRequest("No se creo la empresa");
                }
                else
                {
                    return empresa;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/empresa/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Empresa data)
        {
            try
            {
                Empresa empresa = (Empresa)data;
                if (empresa == null || empresa.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (Fachada.ModificarEmpresa(empresa))
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


        //// DELETE api/empresa/eliminar/
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
                Empresa empresa = new Empresa
                {
                    ID = id
                };
                if (Fachada.EliminarEmpresa(empresa))
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
