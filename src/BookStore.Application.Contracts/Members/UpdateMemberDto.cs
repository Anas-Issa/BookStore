using System;
using Volo.Abp.Application.Dtos;

namespace BookStore.Members;
public class UpdateMemberDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ListResultDto<MemberBookDto> Books { get; set; }
}
