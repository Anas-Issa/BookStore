using BookStore.Books;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace BookStore.Members;
public class MemberBook : Entity<Guid>, IMultiTenant, ISoftDelete
{
    public Guid? TenantId { get; set; }
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    public DateTime BorrowingDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public virtual Book BorrowedBook { get; set; }

    public bool IsDeleted { get; set; }
}
