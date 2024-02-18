using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BookStore.Members;
public class Member : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<MemberBook> Books { get; set; }
}
