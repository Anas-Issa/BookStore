using AutoMapper;
using BookStore.Authors;
using BookStore.Books;
using BookStore.Members;
using System.Linq;

namespace BookStore;

public class BookStoreApplicationAutoMapperProfile : Profile
{

    public BookStoreApplicationAutoMapperProfile(
      )
    {

        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Book, BookDto>()
              .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations))
              .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
        CreateMap<BookTranslation, BookTranslationDto>();
        CreateMap<UpdateBookDto, Book>();
        CreateMap<CreateBookDto, Book>();

        CreateMap<Author, AuthorDto>();
        CreateMap<Author, AuthorLookupDto>();
        CreateMap<CreateAuthorDto, Author>();
        CreateMap<UpdateAuthorDto, Author>();

        CreateMap<Member, MemberDto>()
            .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.BorrowedBooks.Select(b => new MemberBookDto
            {
                BookId = b.Id,
                BorrowedBookName = b.BorrowedBook.Name,
                BorrowingDate = b.BorrowingDate,
                ReturnDate = b.ReturnDate,
            })));
        CreateMap<CreateMemberDto, Member>();
        CreateMap<UpdateMemberDto, Member>();
        CreateMap<MemberBookDto, MemberBook>();
        CreateMap<MemberBook, MemberBookDto>();


    }

}

