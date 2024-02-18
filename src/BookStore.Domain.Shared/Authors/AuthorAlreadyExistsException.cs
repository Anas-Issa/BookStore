using Volo.Abp;

namespace BookStore.Authors;
public class AuthorAlreadyExistsException : BusinessException
{
    public AuthorAlreadyExistsException(string name)
        : base(BookStoreDomainErrorCodes.AuthorAlreadyExist)
    {
        WithData("name", name);
    }
}
