using System;
using System.Collections.Generic;

namespace BookStore.Members;
public class CreateMemberDto
{

    public string Name { get; set; }
    public string Description { get; set; }

    public List<Guid> Books { get; set; } = new List<Guid>();
}
