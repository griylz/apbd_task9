using apbd_task9.Models.DTOs;
using apbd_task9.Repository;
using Microsoft.AspNetCore.Mvc;

namespace apbd_task9.Controller;

[ApiController]
[Route("api")]
public class ClientTripsController : ControllerBase
{
    private readonly IClientTripsRepository _clientTripsRepository;

    public ClientTripsController(IClientTripsRepository clientTripsRepository)
    {
        _clientTripsRepository = clientTripsRepository;
    }

    [HttpGet]
    [Route("trips")]
    public async Task<ActionResult<PageDTO>> GetTrips(int pageNum, int pageSize)
    {
        return Ok(await _clientTripsRepository.GetClientsPages(pageNum, pageSize));
    }
    
    
    [HttpDelete("clients/{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        try
        {
            await _clientTripsRepository.DeleteClient(idClient);
            return NoContent(); 
        }
        catch (KeyNotFoundException knfex)
        {
            return NotFound(knfex.Message); 
        }
        catch (ArgumentException argex)
        {
            return BadRequest(argex.Message);
        }
    }

    [HttpPost("/trips/{idTrip}/clients")]
    public async Task<IActionResult> AddClintToTrip(ClientTripRequestDTO clientTripRequestDto)
    {
        try
        {
            await _clientTripsRepository.AssignClientToATrip(clientTripRequestDto.IdTrip, clientTripRequestDto);
            return Ok();
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            return NotFound(keyNotFoundException.Message);
        }
        catch (ArgumentException argumentException)
        {
            return BadRequest(argumentException.Message);
        }
    } 
    
    
    
    
    
    
}