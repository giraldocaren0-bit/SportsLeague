using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using SportsLeague.Domain.Services;

namespace SportsLeague.API.Controllers

{


    [ApiController]

    [Route("api/[controller]")]


    public class SponsorController : ControllerBase
    {

        private readonly ISponsorService _sponsorService;

        private readonly IMapper _mapper;

        private readonly ILogger<SponsorController> _logger;

        public SponsorController(ISponsorService sponsorService, IMapper mapper, ILogger<SponsorController> logger)
        {
            _sponsorService = sponsorService;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<SponsorResponseDTO>> CreateSponsor(SponsorRequestDTO sponsorRequest)
        {
            try
            {
                var sponsorEntity = _mapper.Map<Sponsor>(sponsorRequest);
                var createdSponsor = await _sponsorService.CreateAsync(sponsorEntity);
                var sponsorResponse = _mapper.Map<SponsorResponseDTO>(createdSponsor);
                return CreatedAtAction(nameof(GetSponsorById), new { id = sponsorResponse.Id }, sponsorResponse);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to create sponsor with name {Name} and email {Email}", sponsorRequest.Name, sponsorRequest.ContactEmail);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a sponsor with name {Name} and email {Email}", sponsorRequest.Name, sponsorRequest.ContactEmail);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<SponsorResponseDTO>> GetSponsorById(int id)
        {
           
                var sponsor = await _sponsorService.GetByIdAsync(id);
                if (sponsor == null)
                {
                    return NotFound();
                }
                var sponsorResponse = _mapper.Map<SponsorResponseDTO>(sponsor);
                return Ok(sponsorResponse);
            
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAllSponsors()
        {
            var sponsors = await _sponsorService.GetAllAsync();
            var sponsorResponses = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
            return Ok(sponsorResponses);
        }
        [HttpGet("Search/{name}")]
        public async Task<ActionResult<SponsorResponseDTO>> SearchSponsorsByName(string name)
        {
            try
            {
                var sponsors = await _sponsorService.GetByNameAsync(name);
                if (sponsors == null)
                {
                    return NotFound(new { Message = $"No pudo encontrar a '{name}'." });

                }
                var sponsorResponses = _mapper.Map<SponsorResponseDTO>(sponsors);
                return Ok(sponsorResponses);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError("An unexpected error occurred while searching for sponsors with name {Name}", name);
                return NotFound(new { Message = $"No pudo encontrar a '{name}'." });
            }

        }

        [HttpGet("Search/Email/{contactEmail}")]
        public async Task<ActionResult<SponsorResponseDTO>> SearchSponsorsByContactEmail(string contactEmail)
        {
            try
            {
                var sponsors = await _sponsorService.GetByContactEmailAsync(contactEmail);
                var sponsorResponses = _mapper.Map<SponsorResponseDTO>(sponsors);
                return Ok(sponsorResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while searching for sponsors with email {Email}", contactEmail);
                return NotFound(new { Message = $"No pudo encontrar a '{contactEmail}'." });
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<SponsorResponseDTO>> UpdateSponsor(int id, SponsorRequestDTO sponsorRequest)
        {
            try
            {
                var sponsorEntity = _mapper.Map<Sponsor>(sponsorRequest);
                var updatedSponsor = await _sponsorService.UpdateAsync(id, sponsorEntity);
                if (updatedSponsor == null)
                {
                    return NotFound();
                }
                var sponsorResponse = _mapper.Map<SponsorResponseDTO>(updatedSponsor);
                return Ok(sponsorResponse);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Failed to update sponsor with ID {Id} using name {Name} and email {Email}", id, sponsorRequest.Name, sponsorRequest.ContactEmail);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating sponsor with ID {Id} using name {Name} and email {Email}", id, sponsorRequest.Name, sponsorRequest.ContactEmail);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSponsor(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Failed to delete sponsor with ID {Id} because it was not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting sponsor with ID {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpPatch("{id}/category")]
        public async Task<IActionResult> UpdateSponsorCategory(int id, SponsorUpdateCategoryDTO updateDTO)
        {
            try
            {
                await _sponsorService.UpdateCategoryAsync(id, updateDTO.Category);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Failed to update category for sponsor with ID {Id} because it was not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating category for sponsor with ID {Id}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}