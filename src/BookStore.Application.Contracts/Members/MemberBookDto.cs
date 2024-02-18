using System;

namespace BookStore.Members;
public class MemberBookDto
{
    public Guid BookId { get; set; }
    //public Guid MemberId { get; set; }
    public DateTime BorrowingDate { get; set; }
    public DateTime ReturnDate { get; set; }
}
