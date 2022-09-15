using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers.v1
{
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("/open-banking/admin/v1")]
    public class ClientesController : BaseController
    {
        public ClientesController()
        {
        }

        [HttpPost("incluir-cliente")]
        public async Task<IActionResult> IncluirCliente()
        {
            try
            {
                
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPatch("alterar-cliente")]
        public async Task<IActionResult> AlterarCliente()
        {
            try
            {

                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [HttpDelete("excluir-cliente")]
        public async Task<IActionResult> ExcluirCliente()
        {
            try
            {

                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [HttpGet("listar-clientes")]
        public async Task<IActionResult> ListarClientes()
        {
            try
            {

                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
        [HttpGet("detalhar-cliente/{clienteId}")]
        public async Task<IActionResult> DetalharClientes(int clienteId)
        {
            try
            {

                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
