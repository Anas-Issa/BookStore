using System.Collections.Generic;

namespace BookStore.Members;
public class CreateMemberDto
{

    public string Name { get; set; }
    public string Description { get; set; }

    public List<MemberBookDto> Books { get; set; } = new List<MemberBookDto>();
}
