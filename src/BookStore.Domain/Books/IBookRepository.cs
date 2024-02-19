using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BookStore.Books;
public interface IBookRepository : IRepository<Book, Guid>
{
    Task<Book> FindBookByNameAsync(Guid authorId, string name);
    Task<List<Guid>> GetExistingBookIdsAsync(List<Guid> bookIds);
}
