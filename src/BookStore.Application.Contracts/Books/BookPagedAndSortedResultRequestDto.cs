using System;
using Volo.Abp.Application.Dtos;

namespace BookStore.Books;
public class BookPagedAndSortedResultRequestDto : PagedAndSortedResultRequestDto
{
    public string? Name { get; set; }
    public DateTime? PublishDate { get; set; }
    public float? MinPrice { get; set; }
    public float? MaxPrice { get; set; }
}
