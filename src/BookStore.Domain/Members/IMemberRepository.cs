using System;
using Volo.Abp.Domain.Repositories;

namespace BookStore.Members;
public interface IMemberRepository : IRepository<Member, Guid>
{

}
