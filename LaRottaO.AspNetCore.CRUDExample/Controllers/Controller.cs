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
        //Hardcoded credentials for example purposes only, replace with
        //a decent implementation. Included is a PBKDF2 hashing code you
        //can use for this purpose
        //*****************************************************************//

        [HttpPost("AuthorizeWithRole")]
        public IActionResult AuthorizeWithRole(string username = "admin", string password = "password", string role = "Admin")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate username and password (replace with actual authentication logic)
            var isValidUser = username == "admin" && password == "password";

            if (!isValidUser)
            {
                return Unauthorized("Invalid credentials");
            }

            // Validate role (replace with your role validation logic if needed)
            var validRoles = new[] { "Admin", "User", "Manager" }; // Replace with your roles
            if (!validRoles.Contains(role))
            {
                return BadRequest("Invalid role");
            }

            // Generate the JWT token
            var token = GenerateToken(username, role);
            return Ok(new { Token = token });
        }

        private string GenerateToken(string username, string role)
        {

            //*****************************************************************//
            //Attached key is an example, change it and do not reveal it
            //*****************************************************************//
         
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role) // Add the role claim
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize(Roles = "User")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "User")]
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