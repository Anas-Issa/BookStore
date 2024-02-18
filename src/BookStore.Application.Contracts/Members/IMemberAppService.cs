using System;
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
}
