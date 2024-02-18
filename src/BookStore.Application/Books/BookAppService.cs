using BookStore.Authors;
using BookStore.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Settings;

namespace BookStore.Books;
public class BookAppService :
    CrudAppService<Book,
        BookDto,
        Guid,
        BookPagedAndSortedResultRequestDto,
        CreateBookDto,
        UpdateBookDto>, IBookAppService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ISettingProvider _settingProvider;
    public BookAppService(IBookRepository bookRepository,
           ISettingProvider settingProvider,
        IAuthorRepository authorRepository) : base(bookRepository)
    {
        _authorRepository = authorRepository;
        _bookRepository = bookRepository;
        _settingProvider = settingProvider;
        GetPolicyName = BookStorePermissions.Books.Default;
        GetListPolicyName = BookStorePermissions.Books.Default;
        CreatePolicyName = BookStorePermissions.Books.Create;
        UpdatePolicyName = BookStorePermissions.Books.Edit;
        DeletePolicyName = BookStorePermissions.Books.Delete;
    }

    //Get Book By Id
    protected override async Task<Book> GetEntityByIdAsync(Guid id)
    {
        return (await Repository.WithDetailsAsync(b => b.Author, b => b.Translations))
           .Where(x => x.Id == id)
           .FirstOrDefault();
    }
    protected override async Task<BookDto> MapToGetOutputDtoAsync(Book entity)
    {


        return await Task.FromResult(ObjectMapper.Map<Book, BookDto>(entity));
    }
    public override async Task<BookDto> GetAsync(Guid id)
    {
        await CheckGetPolicyAsync();
        var book = await GetEntityByIdAsync(id);
        if (book == null)
        {
            throw new EntityNotFoundException(typeof(Book), id);
        }
        return await MapToGetOutputDtoAsync(book);
    }

    // Get  filtered,sorted,paged list of books
    protected override async Task<IQueryable<Book>> CreateFilteredQueryAsync(BookPagedAndSortedResultRequestDto input)
    {
        return (await Repository.WithDetailsAsync(b => b.Author, b => b.Translations))
          .WhereIf(!input.Name.IsNullOrEmpty(), x => x.Name.Contains(input.Name))
         .WhereIf(input.MinPrice.HasValue, x => x.Price >= input.MinPrice)
         .WhereIf(input.MaxPrice.HasValue, x => x.Price <= input.MaxPrice)
         .WhereIf(input.PublishDate.HasValue, x => x.PublishDate.Date == input.PublishDate.Value.Date);
    }
    protected override IQueryable<Book> ApplySorting(IQueryable<Book> query, BookPagedAndSortedResultRequestDto input)
    {
        return base.ApplySorting(query, input);
    }
    protected override IQueryable<Book> ApplyPaging(IQueryable<Book> query, BookPagedAndSortedResultRequestDto input)
    {
        return base.ApplyPaging(query, input);
    }
    protected override async Task<List<BookDto>> MapToGetListOutputDtosAsync(List<Book> entities)
    {
        return await Task.FromResult(ObjectMapper.Map<List<Book>, List<BookDto>>(entities));
    }
    public override async Task<PagedResultDto<BookDto>> GetListAsync(BookPagedAndSortedResultRequestDto input)
    {
        await CheckGetListPolicyAsync();

        var query = await CreateFilteredQueryAsync(input);
        var totalCount = await AsyncExecuter.CountAsync(query);

        var entities = new List<Book>();
        var entityDtos = new List<BookDto>();

        if (totalCount > 0)
        {
            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            entities = await AsyncExecuter.ToListAsync(query);
            entityDtos = await MapToGetListOutputDtosAsync(entities);
        }

        return new PagedResultDto<BookDto>(
            totalCount,
            entityDtos
        );
    }

    //these methods as they are in the tutorial
    public async Task<ListResultDto<AuthorLookupDto>> GetAuthorLookupAsync()
    {
        var authors = await _authorRepository.GetListAsync();

        return new ListResultDto<AuthorLookupDto>(
            ObjectMapper.Map<List<Author>, List<AuthorLookupDto>>(authors)
        );
    }

    public async Task AddTranslationsAsync(Guid id, AddBookTranslationDto input)
    {
        var queryable = await Repository.WithDetailsAsync();

        var book = await AsyncExecuter.FirstOrDefaultAsync(queryable, x => x.Id == id);

        if (book.Translations != null && book.Translations.Any(x => x.Language == input.Language))
        {
            throw new UserFriendlyException($"Translation already available for {input.Language}");
        }

        book.Translations.Add(new BookTranslation
        {
            BookId = book.Id,
            Name = input.Name,
            Language = input.Language
        });

        await Repository.UpdateAsync(book);
    }



    //Create Book 
    public override async Task<BookDto> CreateAsync(CreateBookDto input)
    {
        await CheckCreatePolicyAsync();
        await CheckIfBookExistsAsync(input.AuthorId, input.Name);
        var entity = await MapToEntityAsync(input);
        await Repository.InsertAsync(entity, autoSave: true);
        return await MapToGetOutputDtoAsync(entity);
    }
    protected override async Task<Book> MapToEntityAsync(CreateBookDto createInput)
    {
        return await Task.FromResult(ObjectMapper.Map<CreateBookDto, Book>(createInput));
    }

    //Update Book 
    public override async Task<BookDto> UpdateAsync(Guid id, UpdateBookDto input)
    {
        await CheckUpdatePolicyAsync();
        await CheckIfBookExistsAsync(input.AuthorId, input.Name);
        var entity = await GetEntityByIdAsync(id);
        await MapToEntityAsync(input, entity);
        await Repository.UpdateAsync(entity, autoSave: true);
        return await MapToGetOutputDtoAsync(entity);
    }

    //check of the book is existed with same name for same author
    private async Task CheckIfBookExistsAsync(Guid authorId, string bookName)
    {
        var existingBook = await _bookRepository.FindBookByNameAsync(authorId, bookName);

        if (existingBook != null)
        {
            throw new BookAlreadyExistsEcxseption(bookName);
        }
    }
}

