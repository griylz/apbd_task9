using apbd_task9.Models.DTOs;

namespace apbd_task9.Repository;

public interface IClientTripsRepository
{
    Task<PageDTO> GetClientsPages(int pageNum, int pageSize);

    Task DeleteClient(int id);

    Task AssignClientToATrip(int id,ClientTripRequestDTO clientTripRequestDto);
}