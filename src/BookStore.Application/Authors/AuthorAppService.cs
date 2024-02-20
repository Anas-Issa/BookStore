using BookStore.Books;
using BookStore.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;

namespace BookStore.Authors;
public class AuthorAppService : CrudAppService<Author, AuthorDto, Guid, AuthorPagedAndSortedResultRequestDto, CreateAuthorDto, UpdateAuthorDto>,
                                IAuthorAppService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly AuthorManager _authorManager;
    public AuthorAppService(IAuthorRepository repository, AuthorManager authorManager) : base(repository)
    {
        _authorRepository = repository;
        _authorManager = authorManager;

        GetPolicyName = BookStorePermissions.Authors.Default;
        GetListPolicyName = BookStorePermissions.Authors.Default;
        CreatePolicyName = BookStorePermissions.Authors.Create;
        UpdatePolicyName = BookStorePermissions.Authors.Edit;
        DeletePolicyName = BookStorePermissions.Authors.Delete;
    }

    //Get Author with his Books
    public override async Task<AuthorDto> GetAsync(Guid id)
    {
        await CheckGetPolicyAsync();
        var author = await GetEntityByIdAsync(id);
        if (author == null)
        {
            throw new EntityNotFoundException(typeof(Author), id);
        }

        return await MapToGetOutputDtoAsync(author);
    }
    protected override async Task<Author> GetEntityByIdAsync(Guid id)
    {
        return (await Repository.WithDetailsAsync(a => a.Books))
                         .Where(a => a.Id == id)
                         .FirstOrDefault();
    }


    //get filterd,sorted,paged list of authors
    [Authorize(policy: "FullNameControl")]

    public override async Task<PagedResultDto<AuthorDto>> GetListAsync(AuthorPagedAndSortedResultRequestDto input)
    {
        await CheckGetListPolicyAsync();

        var query = await CreateFilteredQueryAsync(input);
        var totalCount = await AsyncExecuter.CountAsync(query);

        var entities = new List<Author>();
        var entityDtos = new List<AuthorDto>();
        if (totalCount > 0)
        {
            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            entities = await AsyncExecuter.ToListAsync(query);
            entityDtos = await MapToGetListOutputDtosAsync(entities);
        }

        return new PagedResultDto<AuthorDto>(
            totalCount,
            entityDtos
        );
    }

    protected override async Task<IQueryable<Author>> CreateFilteredQueryAsync(AuthorPagedAndSortedResultRequestDto input)
    {
        return (await Repository.WithDetailsAsync(a => a.Books))
          .WhereIf(!input.Name.IsNullOrEmpty(), x => x.Name.Contains(input.Name));

    }

    //Create Author With Books
    protected override Task<Author> MapToEntityAsync(CreateAuthorDto createInput)
    {
        var entity = ObjectMapper.Map<CreateAuthorDto, Author>(createInput);
        return base.MapToEntityAsync(createInput);
    }
    public override async Task<AuthorDto> CreateAsync(CreateAuthorDto input)
    {
        await CheckCreatePolicyAsync();
        await CheckIfAuthorExist(input.Name, default);

        var existingAuthor = await _authorRepository.FindByNameAsync(input.Name);
        var result = new AuthorDto();
        if (existingAuthor != null)
        {
            throw new AuthorAlreadyExistsException(input.Name);
        }
        try
        {
            var author = ObjectMapper.Map<CreateAuthorDto, Author>(input);
            foreach (var item in author.Books)
            {
                EntityHelper.TrySetId(item, () => GuidGenerator.Create());
            }
            await _authorRepository.InsertAsync(author);
            var authorId = author.Id;
            result = ObjectMapper.Map<Author, AuthorDto>(author);
        }
        catch (Exception)
        {

            throw;
        }



        return result;
    }
    protected override Author MapToEntity(CreateAuthorDto createInput)
    {

        var entity = ObjectMapper.Map<CreateAuthorDto, Author>(createInput);
        return entity;
    }
    //Update Author and his books
    public override async Task<AuthorDto> UpdateAsync(Guid id, UpdateAuthorDto input)
    {
        await CheckUpdatePolicyAsync();
        var existingAuthor = await GetEntityByIdAsync(id);

        if (existingAuthor.Name != input.Name)
        {
            await _authorManager.ChangeNameAsync(existingAuthor, input.Name);
        }

        var existingBooks = existingAuthor.Books.ToDictionary(b => b.Id);

        var booksToRemove = existingAuthor.Books
            .Where(book => !input.Books.Any(b => b.Id == book.Id))
            .ToList();

        var booksToUpdateAndInsert = input.Books
            .Select(bookDto => new
            {
                BookDto = bookDto,
                ExistingBook = existingBooks.ContainsKey(bookDto.Id) ? existingBooks[bookDto.Id] : null
            })
            .ToList();

        // Remove books
        foreach (var bookToRemove in booksToRemove)
        {
            existingAuthor.Books.Remove(bookToRemove);
        }

        // Update existing books and insert new books
        foreach (var bookData in booksToUpdateAndInsert)
        {
            if (bookData.ExistingBook != null)
            {
                // Update existing book
                ObjectMapper.Map(bookData.BookDto, bookData.ExistingBook);
            }
            else
            {
                // Insert new book
                var newBook = ObjectMapper.Map<UpdateBookDto, Book>(bookData.BookDto);
                EntityHelper.TrySetId(newBook, () => GuidGenerator.Create());

                newBook.AuthorId = existingAuthor.Id;
                existingAuthor.Books.Add(newBook);
            }
        }
        existingAuthor.ShortBio = input.ShortBio;
        existingAuthor.BirthDate = input.BirthDate;
        await Repository.UpdateAsync(existingAuthor, autoSave: true);
        return await MapToGetOutputDtoAsync(existingAuthor);
    }
    //delete author with his books
    public override async Task DeleteAsync(Guid id)
    {
        await CheckDeletePolicyAsync();
        var existingAuthor = await GetEntityByIdAsync(id);
        existingAuthor.Books.Clear();
        await DeleteByIdAsync(id);
    }
    //check if there is an author with similar name
    private async Task<Author> CheckIfAuthorExist(string name, Guid? id)
    {
        var existingAuthor = await _authorRepository.FindByNameAsync(name);

        if (existingAuthor != null && existingAuthor.Id != id)
        {
            throw new AuthorAlreadyExistsException(name);
        }
        return existingAuthor;
    }
}
