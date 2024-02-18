using Volo.Abp;

namespace BookStore.Books;
public class BookAlreadyExistsEcxseption : BusinessException
{
    public BookAlreadyExistsEcxseption(string name) : base(BookStoreDomainErrorCodes.BookAlreadyExist)
    {
        WithData("name", name);
    }
}
