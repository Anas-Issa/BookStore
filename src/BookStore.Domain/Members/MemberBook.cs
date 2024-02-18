using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace BookStore.Members;
public class MemberBook : Entity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    public DateTime BorrowingDate { get; set; }
    public DateTime ReturnDate { get; set; }
    //public virtual Book Book { get; set; }
    //public virtual Member Member { get; set; }

}
