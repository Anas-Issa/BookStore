using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace BookStore.Members;
public class MemberAppService : CrudAppService<
        Member,
        MemberDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateMemberDto,
        UpdateMemberDto>, IMemberAppService
{
    public MemberAppService(IRepository<Member, Guid> repository) : base(repository)
    {
    }

    public override async Task<MemberDto> CreateAsync(CreateMemberDto input)
    {
        await CheckCreatePolicyAsync();
        var entity = await MapToEntityAsync(input);
        foreach (var item in entity.Books)
        {
            EntityHelper.TrySetId(item, () => GuidGenerator.Create());
            item.MemberId = entity.Id;
        }
        await Repository.InsertAsync(entity);
        return await MapToGetOutputDtoAsync(entity);

    }
}
