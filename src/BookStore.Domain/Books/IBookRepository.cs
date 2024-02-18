using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace BookStore.Books;
public interface IBookRepository : IRepository<Book, Guid>
{
    Task<Book> FindBookByNameAsync(Guid authorId, string name);
}
