using BookStore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace BookStore.Books;
public class EfCoreBookRepository : EfCoreRepository<BookStoreDbContext, Book, Guid>, IBookRepository
{
    public EfCoreBookRepository(IDbContextProvider<BookStoreDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }





    public async Task<Book> FindBookByNameAsync(Guid authorId, string name)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.FirstOrDefaultAsync(b => b.Name == name && b.AuthorId == authorId);

    }

    public async Task<List<Guid>> GetExistingBookIdsAsync(List<Guid> bookIds)
    {
        var existingBookIds = (await GetDbSetAsync())

                     .Where(book => bookIds.Contains(book.Id))
                     .Select(book => book.Id)
                     .ToListAsync();

        return (await existingBookIds);
    }
}
