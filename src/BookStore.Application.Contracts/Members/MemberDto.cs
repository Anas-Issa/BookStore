using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BookStore.Members;
public class MemberDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<MemberBookDto> Books { get; set; }
}
