﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BookStore.Books;
public class BookDto : AuditedEntityDto<Guid>
{
    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; }
    public string Name { get; set; }

    public BookType Type { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }


    public List<BookTranslationDto> Translations { get; set; }


}
