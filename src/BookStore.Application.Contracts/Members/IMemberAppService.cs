using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BookStore.Members;
public interface IMemberAppService : ICrudAppService<
    MemberDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateMemberDto,
    UpdateMemberDto>
{
    Task BorrowBooksAsync(Guid MemberId, List<Guid> BooksIds);

}
