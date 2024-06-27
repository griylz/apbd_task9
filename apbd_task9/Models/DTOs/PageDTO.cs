namespace apbd_task9.Models.DTOs;

public class PageDTO
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<TripDTO> Trips { get; set; } = new List<TripDTO>();
}
