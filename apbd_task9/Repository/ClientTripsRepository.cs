using apbd_task9.Data;
using apbd_task9.Models;
using apbd_task9.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace apbd_task9.Repository;

public class ClientTripsRepository : IClientTripsRepository
{

    private readonly ModelContext _context;

    public ClientTripsRepository(ModelContext context)
    {
        _context = context;
    }

    public async Task<PageDTO> GetClientsPages(int pageNum, int pageSize)
    {
        int totalTrips = await _context.Trips.CountAsync();

        var tripsDTO = await _context.Trips.Select(trip => new TripDTO
            {
                Name = trip.Name,
                Description = trip.Description,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                MaxPeople = trip.MaxPeople,
                Countries = trip.IdCountries.Select(c => new CountryDTO { Name = c.Name }).ToList(),
                Clients = trip.ClientTrips.Select(ct => new ClientDTO
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            }).OrderBy(e => e.DateFrom).Skip((pageNum - 1) * pageSize)
            .Take(pageSize).ToListAsync();
        int totalPageCount = (int)Math.Ceiling(totalTrips / (double)pageSize);

        return new PageDTO
        {
            PageNum = pageNum,
            PageSize = pageSize,
            AllPages = totalPageCount,
            Trips = tripsDTO
        };

    }

    public async Task DeleteClient(int id)
    {
        var hasTrips = await _context.Clients.AnyAsync(c => c.IdClient == id && c.ClientTrips.Any());
        if (hasTrips)
        {
            throw new ArgumentException("Character has assigned trips");
        }
        
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            // Optionally handle the case where the client does not exist
            throw new KeyNotFoundException($"No client found with ID {id}.");
        }

        _context.Clients.Remove(client);

        await _context.SaveChangesAsync();


    }






    public async Task AssignClientToATrip(int id, ClientTripRequestDTO clientTripRequestDto)
    {
        var clientWithPeselExists = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == clientTripRequestDto.Pesel);
        if (clientWithPeselExists!=null)
        {
            var tripAssigned = await _context.ClientTrips.AnyAsync(ct =>
                ct.IdClient == clientWithPeselExists.IdClient && ct.IdTrip == clientTripRequestDto.IdTrip);
            if (tripAssigned)
            {
                throw new ArgumentException("client with the given PESEL number is already registered for the given trip");
            }
            throw new ArgumentException("Client with the given PESEL number already exists");
        }

        var doesGivenTripExists =
            await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == clientTripRequestDto.IdTrip);
        if (doesGivenTripExists == null)
        {
            throw new KeyNotFoundException("the given trip doesn't exist");
        }
        if (doesGivenTripExists.DateFrom<=DateTime.Today)
        {
            
            throw new ArgumentException("The DateFrom is not a future date");
        }
        var newClient = new Client
        {
            FirstName = clientTripRequestDto.FirstName,
            LastName = clientTripRequestDto.LastName,
            Email = clientTripRequestDto.Email,
            Telephone = clientTripRequestDto.Telephone,
            Pesel = clientTripRequestDto.Pesel
        };
        _context.Clients.Add(newClient);
        await _context.SaveChangesAsync(); 

       
        var clientTrip = new ClientTrip
        {
            IdClient = newClient.IdClient,
            IdTrip = id,
            RegisteredAt = DateTime.Now,
            PaymentDate = clientTripRequestDto.PaymentDate
        };
        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();
        
    }
}