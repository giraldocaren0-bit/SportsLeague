using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;    
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Text.RegularExpressions;


namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ILogger _logger;
        private readonly ISponsorRepository _sponsorRepository;


    public SponsorService(ILogger<SponsorService> logger, ISponsorRepository sponsorRepository)
        {
            _logger = logger;
            _sponsorRepository = sponsorRepository;
        }


        public async Task<Sponsor?> CreateAsync(Sponsor sponsor)
        {
            var existsName = await _sponsorRepository.ExistsbyNameAsync(sponsor.Name);
            var existsEmail = await _sponsorRepository.ExistsbyContactEmailAsync(sponsor.ContactEmail);

            if (existsName != null)
            {
                _logger.LogWarning("Sponsor with name {Name} already exists", sponsor.Name);
                throw new InvalidOperationException($"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
            }

            if (existsEmail != null)
            {
                _logger.LogWarning("Sponsor with contact email {Email} already exists", sponsor.ContactEmail);
                throw new InvalidOperationException($"Ya existe un patrocinador con el correo electrónico '{sponsor.ContactEmail}'");
            }

            if (string.IsNullOrWhiteSpace(sponsor.ContactEmail))
            {
                _logger.LogWarning("Contact email is required for sponsor '{Name}'", sponsor.Name);
                throw new InvalidOperationException("El email de contacto es obligatorio.");
            }

            
            if (!Regex.IsMatch(sponsor.ContactEmail,
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                RegexOptions.IgnoreCase))
            {
                _logger.LogWarning("Invalid email format for sponsor '{Name}': {Email}",
                    sponsor.Name, sponsor.ContactEmail);
                throw new InvalidOperationException($"El email de contacto '{sponsor.ContactEmail}' no tiene un formato válido.");
            }

            _logger.LogInformation("Creating sponsor: {Name}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task<Sponsor?> GetByContactEmailAsync(string contactEmail)
        {
            _logger.LogInformation("Retrieving sponsor by contact email: {Email}", contactEmail);
            var existingsponsor = await _sponsorRepository.ExistsbyContactEmailAsync(contactEmail);

            if (existingsponsor == null)
            {
                _logger.LogWarning("Sponsor with contact email '{Email}' not found.", contactEmail);
                throw new KeyNotFoundException($"No se encontro el patrocinador con email {contactEmail}");
            }
            return existingsponsor;
        }

        public async Task<Sponsor?> GetByNameAsync(string name)
        {
            _logger.LogInformation("Retrieving sponsor by name: {Name}", name);
            var existingsponsor = await _sponsorRepository.ExistsbyNameAsync(name);

            if (existingsponsor == null)
            {
                _logger.LogWarning("Sponsor with name '{Name}' not found.", name);
                throw new KeyNotFoundException($"No se encontro el patrocinador con nombre {name}");
            }
            return existingsponsor;
        }
        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor by ID: {Id}", id);
            var existingsponsor = await _sponsorRepository.GetByIdAsync(id);

            if (existingsponsor == null)
            {
                _logger.LogWarning("Sponsor with ID '{Id}' not found.", id);
                
            }
            return existingsponsor;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors.");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogWarning("Sponsor with ID '{Id}' does not exist.", id);
                throw new KeyNotFoundException($"No se encontro el patrocinador con ID{id}");
            }
            _logger.LogInformation("Deleting sponsor with ID: {Id}", id);
            await _sponsorRepository.DeleteAsync(id);
        }


        public async Task<Sponsor?> UpdateAsync(int id, Sponsor sponsor)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);

            if (existingSponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {Id} not found for update", id);
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID '{id}'");
            }

            if (existingSponsor.Name != sponsor.Name)
            {
                var existsName = await _sponsorRepository.ExistsbyNameAsync(sponsor.Name);
                if (existsName != null)
                {
                    _logger.LogWarning("Sponsor with name {Name} already exists", sponsor.Name);
                    throw new InvalidOperationException($"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
                }
            }

            if (existingSponsor.ContactEmail != sponsor.ContactEmail)
            {
                var existsEmail = await _sponsorRepository.ExistsbyContactEmailAsync(sponsor.ContactEmail);
                if (existsEmail != null)
                {
                    _logger.LogWarning("Sponsor with contact email {Email} already exists", sponsor.ContactEmail);
                    throw new InvalidOperationException($"Ya existe un patrocinador con el correo electrónico '{sponsor.ContactEmail}'");
                }
            }

            if (string.IsNullOrWhiteSpace(sponsor.ContactEmail))
            {
                _logger.LogWarning("Contact email is required for update on sponsor ID {Id}", id);
                throw new InvalidOperationException("El email de contacto es obligatorio.");
            }

            
            if (!Regex.IsMatch(sponsor.ContactEmail,
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                RegexOptions.IgnoreCase))
            {
                _logger.LogWarning("Invalid email format for update on sponsor ID {Id}: {Email}",
                    id, sponsor.ContactEmail);
                throw new InvalidOperationException($"El email de contacto '{sponsor.ContactEmail}' no tiene un formato válido.");
            }

            existingSponsor.Name = sponsor.Name;
            existingSponsor.ContactEmail = sponsor.ContactEmail;
            existingSponsor.Phone = sponsor.Phone;
            existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;

            _logger.LogInformation("Updating sponsor with ID: {Id}", id);
            await _sponsorRepository.UpdateAsync(existingSponsor);

            return existingSponsor;
        }


        public async Task UpdateCategoryAsync(int id, SponsorCategory newCategory)

        {

            var sponsorCategory = await _sponsorRepository.GetByIdAsync(id);

            if (sponsorCategory == null)

            {

                _logger.LogWarning("Sponsor with ID {Id} not found for category update", id);

                throw new KeyNotFoundException($"No se encontró el patrocinador con ID '{id}'");

            }


            if (sponsorCategory.Category == newCategory)

            {

                _logger.LogWarning("Sponsor {Id} already has category {Category}", id, newCategory);

                throw new InvalidOperationException("El patrocinador ya tiene esa categoría.");

            }


            sponsorCategory.Category = newCategory;


            _logger.LogInformation(

                "Actualizando categoría del patrocinador {SponsorId} a {NewCategory}",

                id, newCategory);


            await _sponsorRepository.UpdateAsync(sponsorCategory);

        }
    }
}


