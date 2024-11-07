using LaRottaO.AspNetCore.CRUDExample.Context;
using LaRottaO.AspNetCore.CRUDExample.Interfaces;
using LaRottaO.AspNetCore.CRUDExample.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LaRottaO.AspNetCore.CRUDExample.Services
{
    public class CollaboratorDataService : ICollaboratorData, IDisposable
    {
        private readonly RepositoryContext _context;

        public CollaboratorDataService(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<(bool success, int httpCode, string errorReason, List<Collaborator> results)> GetAllCollaboratorsAsync()
        {
            try
            {
                // Does not Include CollaboratorDataEntries
                //var results = await _context.CollaboratrorTable.ToListAsync();

                // Include CollaboratorDataEntries
                var results = await _context.CollaboratrorTable
                 .Include(c => c.CollaboratorDataEntries)
                 .ToListAsync();

                if (results == null)
                {
                    return (false, 500, GlobalVariables.MESSAGE_ERROR_IN_DATABASE, new List<Collaborator>());
                }

                if (results.Count == 0)
                {
                    return (false, 200, "No records to show.", new List<Collaborator>());
                }

                return (true, 200, string.Empty, results);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.ToString());
                return (false, 500, GlobalVariables.MESSAGE_ERROR_IN_DATABASE, new List<Collaborator>());
            }
        }

        public async Task<(bool success, int httpCode, string errorReason, Collaborator result)> GetFullNameByPassportNumber(String argPassportNumber)
        {
            try
            {
                var result = await _context.CollaboratrorTable.FirstOrDefaultAsync(x => x.PassportNumber.Equals(argPassportNumber));

                if (result == null)
                {
                    return (false, 401, $"Passport number {argPassportNumber} not found", Collaborator.CreateEmpty());
                }

                return (true, 200, "", result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.ToString());
                return (false, 500, GlobalVariables.MESSAGE_ERROR_IN_DATABASE, Collaborator.CreateEmpty());
            }
        }

        public async Task UpdateAsync(Collaborator collaborator)
        {
            _context.CollaboratrorTable.Update(collaborator);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(String argPassportNumber)
        {
            var result = await _context.CollaboratrorTable.FirstOrDefaultAsync(x => x.PassportNumber.Equals(argPassportNumber));
            if (result != null)
            {
                _context.CollaboratrorTable.Remove(result);
                await _context.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<(bool success, int httpCode, string errorReason, Collaborator collaborator)> AddNewCollaboratorAsync(Collaborator argNewCollaborator)
        {
            try
            {
                var colabExistsResult = await checkIfCollabExists(argNewCollaborator.PassportNumber);

                if (!colabExistsResult.success)
                {
                    return (false, 500, GlobalVariables.MESSAGE_ERROR_IN_DATABASE, Collaborator.CreateEmpty());
                }

                if (colabExistsResult.collabExists)
                {
                    return (true, 200, "", colabExistsResult.collaborator);
                }

                var colabCreatResult = await createCollaborator(argNewCollaborator.PassportNumber, argNewCollaborator.FullName);

                if (!colabCreatResult.success)
                {
                    return (false, 500, colabCreatResult.errorReason, Collaborator.CreateEmpty());
                }

                return (true, 200, "", colabCreatResult.collaborator);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.ToString());
                return (false, 500, "Unable to create / retrieve collaborator: " + ex.ToString(), Collaborator.CreateEmpty());
            }
        }

        public async Task<(bool success, int httpCode, string errorReason, CollaboratorData collaboratorData)> AddCollaboratorDataAsync(CollaboratorData argCollaboratorData)
        {
            try
            {
                var colabExistsResult = await checkIfCollabExists(argCollaboratorData.PassportNumber);

                if (!colabExistsResult.success)
                {
                    return (false, 500, GlobalVariables.MESSAGE_ERROR_IN_DATABASE, CollaboratorData.CreateEmpty());
                }

                if (!colabExistsResult.collabExists)
                {
                    return (false, 400, "The entered passport number was not found", CollaboratorData.CreateEmpty());
                }

                // Add each CollaboratorData item and set the Passport Number as foreign key

                argCollaboratorData.EntryCreationDate = DateTime.Now;
                argCollaboratorData.PassportNumber = colabExistsResult.collaborator.PassportNumber;
                colabExistsResult.collaborator.CollaboratorDataEntries.Add(argCollaboratorData);

                await _context.SaveChangesAsync();

                return (true, 200, "", argCollaboratorData);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to add data after creating / retrieving collaborator");
                return (false, 500, GlobalVariables.MESSAGE_ERROR_IN_DATABASE, CollaboratorData.CreateEmpty());
            }
        }

        public async Task<(bool success, string errorReason, bool collabExists, Collaborator collaborator)> checkIfCollabExists(string argPassportNumber)
        {
            try
            {
                var collaborator = await _context.CollaboratrorTable
              .Include(c => c.CollaboratorDataEntries)
              .FirstOrDefaultAsync(c => c.PassportNumber == argPassportNumber);

                if (collaborator != null)
                {
                    return (true, "", true, collaborator);
                }
                else
                {
                    return (true, "Collaborator not found", false, Collaborator.CreateEmpty());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.ToString());
                return (false, GlobalVariables.MESSAGE_ERROR_IN_DATABASE, false, Collaborator.CreateEmpty());
            }
        }

        public async Task<(bool success, string errorReason, Collaborator collaborator)> createCollaborator(string argPassportNumber, string argFullName)
        {
            try
            {
                Collaborator collaborator =
                    new Collaborator
                    {
                        PassportNumber = argPassportNumber,
                        FullName = argFullName,
                        EntryCreationDate = DateTime.Now
                    };

                _context.CollaboratrorTable.Add(collaborator);
                await _context.SaveChangesAsync();

                return (true, "", collaborator);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.ToString());
                return (false, "Unable to create new collaborator", Collaborator.CreateEmpty());
            }
        }
    }
}