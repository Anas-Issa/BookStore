using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.Books;
public interface IBookAppService :
        ICrudAppService<BookDto,
        Guid,
        BookPagedAndSortedResultRequestDto,
        CreateBookDto,
        UpdateBookDto>
{
    Task<ListResultDto<AuthorLookupDto>> GetAuthorLookupAsync();

    Task AddTranslationsAsync(Guid id, AddBookTranslationDto input);
}
