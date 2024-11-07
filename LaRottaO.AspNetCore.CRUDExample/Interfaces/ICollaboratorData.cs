using LaRottaO.AspNetCore.CRUDExample.Models;

namespace LaRottaO.AspNetCore.CRUDExample.Interfaces
{
    public interface ICollaboratorData
    {
        Task<(bool success, int httpCode, string errorReason, List<Collaborator> results)> GetAllCollaboratorsAsync();

        Task<(bool success, int httpCode, string errorReason, Collaborator collaborator)> AddNewCollaboratorAsync(Collaborator argNewCollaborator);

        Task<(bool success, int httpCode, string errorReason, CollaboratorData collaboratorData)> AddCollaboratorDataAsync(CollaboratorData argCollaboratorData);
    }
}