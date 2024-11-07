using LaRottaO.AspNetCore.CRUDExample.Interfaces;
using LaRottaO.AspNetCore.CRUDExample.Models;
using LaRottaO.AspNetCore.CRUDExample.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LaRottaO.AspNetCore.CRUDExample.Controllers
{
    [Route("[controller]")]
    public class Controller : ControllerBase
    {
        private readonly ICollaboratorData _iCollaboratorData;

        private readonly IConfiguration _configuration;

        public Controller(ICollaboratorData iCollaboratorData, IConfiguration configuration)

        {
            _iCollaboratorData = iCollaboratorData;
            _configuration = configuration;
        }

        //*****************************************************************//
        //Hardcoded credentials, replace with a decent implementation
        //*****************************************************************//

        [HttpPost("LoginEndpoint")]
        public IActionResult Login(string username = "admin", string password = "password")
        {
            // Simulate checking user credentials (replace this with actual authentication logic)
            var isValidUser = username == "admin" && password == "password";

            if (!isValidUser)
            {
                return Unauthorized("Invalid credentials");
            }

            var token = new GenerateJWTToken().generate(_configuration, username);

            return Ok(new { Token = token });
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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