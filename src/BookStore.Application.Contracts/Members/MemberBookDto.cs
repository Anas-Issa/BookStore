using System;

namespace BookStore.Members;
public class MemberBookDto
{
    public Guid BookId { get; set; }
    public string? BorrowedBookName { get; set; }
    public DateTime BorrowingDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}
