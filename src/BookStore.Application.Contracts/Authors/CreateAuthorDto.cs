using BookStore.Books;
using System;
using System.Collections.Generic;

namespace BookStore.Authors;
public class CreateAuthorDto
{
    public string Name { get; set; }

    public DateTime BirthDate { get; set; }

    public string ShortBio { get; set; }

    public List<CreateBookDto> Books { get; set; }
}
