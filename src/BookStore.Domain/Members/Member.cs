using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BookStore.Members;
public class Member : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<MemberBook> BorrowedBooks { get; set; }
    public Member()
    {
        BorrowedBooks = new Collection<MemberBook>();
    }

    public void BorrowBook(MemberBook borrowedBook)
    {
        if (BorrowedBooks.Count < MemberConsts.MaxNumberOfBorrowedBooks)
        {
            BorrowedBooks.Add(borrowedBook);
            return;
        }
        else
        {
            throw new UserFriendlyException($"Cannot borrow more than {MemberConsts.MaxNumberOfBorrowedBooks} books.");

        }
    }

    public void ReturnBook(Guid bookId)
    {
        var temp = BorrowedBooks.FirstOrDefault(x => x.BookId == bookId);
        temp.ReturnDate = DateTime.Now;
        BorrowedBooks.Remove(temp);
    }
}
