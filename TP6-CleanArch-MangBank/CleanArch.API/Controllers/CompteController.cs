namespace CleanArch.API.Controllers
{
    //using CleanArch.Application.DTOs;
    //using CleanArch.Application.Interfaces.Services;
    using global::CleanArch.Application.DTOs;
    using global::CleanArch.Application.Interfaces.Services;
    using Microsoft.AspNetCore.Mvc;



    [Route("api/[controller]")]
    [ApiController]
    public class CompteController : ControllerBase
    {
        private readonly ICompteService _service;

        public CompteController(ICompteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompteDTO>>> GetComptes()
        {
            return Ok(await _service.GetComptesAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompteDTO>> GetCompte(int id)
        {
            return Ok(await _service.GetCompteAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<CompteDTO>> CreateCompte(CreateCompteDTO dto)
        {
            var created = await _service.CreateCompteAsync(dto);
            return CreatedAtAction(nameof(GetCompte), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CompteDTO>> UpdateCompte(int id, UpdateCompteDTO dto)
        {
            return Ok(await _service.UpdateCompteAsync(id, dto));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompte(int id)
        {
            await _service.DeleteCompteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CompteDTO>>> Search([FromQuery] int clientId)
        {
            return Ok(await _service.SearchAsync(clientId));
        }

        [HttpPost("operations")]
        public async Task<ActionResult<OperationDTO>> CreateOperation(CreateOperationDTO dto)
        {
            return Ok(await _service.CreateOperationAsync(dto));
        }

        [HttpGet("{numCompte:int}/nombre-operations")]
        public async Task<ActionResult<int>> NombreOperations(int numCompte)
        {
            return Ok(await _service.NombreOperationsAsync(numCompte));
        }

        [HttpGet("{num:int}/extrait")]
        public async Task<ActionResult<IEnumerable<OperationDTO>>> Extrait(int num, [FromQuery] DateTime date)
        {
            return Ok(await _service.ExtraitAsync(num, date));
        }

        [HttpGet("solde-total/{clientId:int}")]
        public async Task<ActionResult<decimal>> SoldeTotal(int clientId)
        {
            return Ok(await _service.SoldeTotalAsync(clientId));
        }

        [HttpPost("virement")]
        public async Task<ActionResult> MAJVirement(VirementDTO dto)
        {
            await _service.VirementAsync(dto);
            return Ok(new { message = "Virement effectué avec succès." });
        }

        [HttpGet("total-retraits/{clientId:int}")]
        public async Task<ActionResult<decimal>> TotalRetraits(int clientId)
        {
            return Ok(await _service.TotalRetraitsAsync(clientId));
        }
    }
}
