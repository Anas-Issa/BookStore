using BookStore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BookStore.Members;
public class EfCoreMemberRepository : EfCoreRepository<BookStoreDbContext, Member, Guid>, IMemberRepository
{
    public EfCoreMemberRepository(IDbContextProvider<BookStoreDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
    public override async Task<IQueryable<Member>> WithDetailsAsync()
    {
        return (await GetQueryableAsync()).IncludeDetails();
    }
}
public static class MembersQueryableExtensions
{
    public static IQueryable<Member> IncludeDetails(
        this IQueryable<Member> queryable,
        bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            .Include(x => x.BorrowedBooks)
            .ThenInclude(b => b.BorrowedBook);

    }
}

