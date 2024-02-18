using BookStore.Authors;
using BookStore.MultiLingualObjects;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BookStore.Books;
public class Book : FullAuditedAggregateRoot<Guid>, IMultiTenant, IMultiLingualObject<BookTranslation>
{
    public Guid? TenantId { get; set; }
    public Guid AuthorId { get; set; }
    public string Name { get; set; }

    public BookType Type { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }

    public virtual Author Author { get; set; }
    public virtual ICollection<BookTranslation> Translations { get; set; } = new List<BookTranslation>();

    // public virtual ICollection<MemberBook> Members { get; set; } 
}
