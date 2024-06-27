namespace apbd_task9.Models.DTOs;

public class TripDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDTO> Countries { get; set; } = new List<CountryDTO>();
    public List<ClientDTO> Clients { get; set; } = new List<ClientDTO>();
}
