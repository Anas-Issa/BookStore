using Volo.Abp.Application.Dtos;

namespace BookStore.Authors;
public class AuthorPagedAndSortedResultRequestDto : PagedAndSortedResultRequestDto
{
    public string? Name { get; set; }
}
