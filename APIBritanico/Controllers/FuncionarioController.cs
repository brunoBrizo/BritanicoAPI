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
    public class FuncionarioController : Controller
    {
        private Fachada_001 Fachada { get; } = Fachada_001.getInstancia();


        //// GET: api/funcionario/getbyid/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Funcionario> GetById(int id)
        {
            try
            {
                if (id > 0)
                {
                    Funcionario funcionario = new Funcionario
                    {
                        ID = id
                    };
                    funcionario = Fachada.GetFuncionario(funcionario);
                    if (funcionario == null)
                    {
                        return BadRequest("No existe el funcionario");
                    }
                    return funcionario;
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


        //// GET: api/funcionario/getall/
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Funcionario>> GetAll()
        {
            try
            {
                List<Funcionario> lstFuncionarios = Fachada.ObtenerFuncionarios();
                return lstFuncionarios;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// GET: api/funcionario/getbysucursal/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Funcionario>> GetBySucursal(int id)
        {
            try
            {
                if (id < 1)
                {
                    return BadRequest("ID no puede ser vacio");
                }
                Sucursal sucursal = new Sucursal
                {
                    ID = id
                };
                List<Funcionario> lstFuncionarios = Fachada.ObtenerFuncionariosPorSucursal(sucursal);
                return lstFuncionarios;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/funcionario/login/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Funcionario> Login([FromBody]Funcionario data)
        {
            try
            {
                Funcionario funcionario = (Funcionario)data;
                if (funcionario == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (funcionario.CI.Equals(String.Empty))
                {
                    return BadRequest("CI no puede ser vacia");
                }
                if (funcionario.Clave.Equals(String.Empty))
                {
                    return BadRequest("Clave no puede ser vacia");
                }
                if (funcionario.SucursalID < 1)
                {
                    return BadRequest("Debe seleccionar una sucursal para loguearse");
                }
                funcionario = Fachada.Login(funcionario);
                if (funcionario == null)
                {
                    return BadRequest("Login incorrecto");
                }
                else
                {
                    return funcionario;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/funcionario/crear/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Funcionario> Crear([FromBody]Funcionario data)
        {
            try
            {
                Funcionario funcionario = (Funcionario)data;
                if (funcionario == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (funcionario.Sucursal == null)
                    funcionario.Sucursal = new Sucursal();
                funcionario.Sucursal.ID = funcionario.SucursalID;
                funcionario = Fachada.CrearFuncionario(funcionario);
                if (funcionario == null)
                {
                    return BadRequest("No se creo el funcionario");
                }
                else
                {
                    return funcionario;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// POST api/funcionario/olvidemipassword/
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> OlvideMiPassword([FromBody]Funcionario data)
        {
            try
            {
                Funcionario funcionario = (Funcionario)data;
                if (funcionario == null)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (funcionario.CI.Equals(String.Empty))
                {
                    return BadRequest("Cedula no puede ser vacia");
                }
                if (funcionario.Sucursal == null)
                    funcionario.Sucursal = new Sucursal();
                funcionario.Sucursal.ID = funcionario.SucursalID;
                return await Fachada.OlvideMiPassword(funcionario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //// PUT api/funcionario/modificar/
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<bool> Modificar([FromBody]Funcionario data)
        {
            try
            {
                Funcionario funcionario = (Funcionario)data;
                if (funcionario == null || funcionario.ID < 1)
                {
                    return BadRequest("Datos no validos en el request");
                }
                if (funcionario.Sucursal == null)
                    funcionario.Sucursal = new Sucursal();
                funcionario.Sucursal.ID = funcionario.SucursalID;
                if (Fachada.ModificarFuncionario(funcionario))
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


        //// DELETE api/funcionario/eliminar/
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
                Funcionario funcionario = new Funcionario
                {
                    ID = id
                };
                if (Fachada.EliminarFuncionario(funcionario))
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
