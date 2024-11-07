using LaRottaO.AspNetCore.CRUDExample.Interfaces;
using LaRottaO.AspNetCore.CRUDExample.Models;
using Microsoft.AspNetCore.Mvc;

namespace LaRottaO.AspNetCore.CRUDExample.Controllers
{
    [Route("[controller]")]
    public class Controller : ControllerBase
    {
        private readonly ICollaboratorData _iCollaboratorData;

        public Controller(ICollaboratorData iCollaboratorData)

        {
            _iCollaboratorData = iCollaboratorData;
        }

        [HttpGet("GetAllCollaboratorsEndpoint")]
        public async Task<IActionResult> GetAllCollaborators()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _iCollaboratorData.GetAllCollaboratorsAsync();

            if (!result.success)
            {
                if (result.httpCode == 400)
                {
                    return BadRequest(result.errorReason);
                }
                if (result.httpCode == 500)
                {
                    return Problem(result.errorReason);
                }
            }

            return Ok(result.results); // 200 OK with list of entries
        }

        [HttpPost("AddNewCollaboratorEndpoint")]
        public async Task<IActionResult> AddNewCollaborator(Collaborator argNewCollaborator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _iCollaboratorData.AddNewCollaboratorAsync(argNewCollaborator);

            if (!result.success)
            {
                if (result.httpCode == 400)
                {
                    return BadRequest(result.errorReason);
                }
                if (result.httpCode == 500)
                {
                    return Problem(result.errorReason);
                }
            }

            return Ok(result.collaborator); // 200 OK with created collaborator
        }

        [HttpPost("AddCollaboratorDataEndpoint")]
        public async Task<IActionResult> AddCollaboratorData(CollaboratorData argCollaboratorData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _iCollaboratorData.AddCollaboratorDataAsync(argCollaboratorData);

            if (!result.success)
            {
                if (result.httpCode == 400)
                {
                    return BadRequest(result.errorReason);
                }
                if (result.httpCode == 500)
                {
                    return Problem(result.errorReason);
                }
            }

            return Ok(result.collaboratorData); // 200 OK with created collaborator
        }
    }
}