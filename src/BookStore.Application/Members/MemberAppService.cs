using BookStore.Books;
using BookStore.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;

namespace BookStore.Members;
public class MemberAppService : CrudAppService<
        Member,
        MemberDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateMemberDto,
        UpdateMemberDto>, IMemberAppService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IBookRepository _bookRepository;
    public MemberAppService(IMemberRepository memberRepository,
                             IBookRepository bookRepository) : base(memberRepository)
    {
        GetPolicyName = BookStorePermissions.Members.Default;
        GetListPolicyName = BookStorePermissions.Members.Default;
        CreatePolicyName = BookStorePermissions.Members.Create;
        UpdatePolicyName = BookStorePermissions.Members.Edit;
        DeletePolicyName = BookStorePermissions.Members.Delete;
        _memberRepository = memberRepository;
        _bookRepository = bookRepository;
    }
    protected override async Task<Member> GetEntityByIdAsync(Guid id)
    {
        return await _memberRepository.GetAsync(id);
    }
    public async Task BorrowBooksAsync(Guid memberId, List<Guid> booksIds)
    {
        var member = await GetEntityByIdAsync(memberId);
        foreach (var bookId in booksIds)
        {
            var memberBook = new MemberBook()
            {
                BookId = bookId,
                MemberId = memberId,
                BorrowingDate = DateTime.Now,
            };
            EntityHelper.TrySetId(memberBook, () => GuidGenerator.Create());
            member.BorrowBook(memberBook);

        }
        await Repository.UpdateAsync(member);
    }
    public async Task ReturnBook(Guid memberId, List<Guid> booksIds)
    {
        var member = await GetEntityByIdAsync(memberId);
        foreach (var bookId in booksIds)
        {
            member.ReturnBook(bookId);
        }
        await Repository.UpdateAsync(member);

    }
    public override async Task<MemberDto> CreateAsync(CreateMemberDto input)
    {
        await CheckCreatePolicyAsync();
        var booksIds = input.Books.ToList();
        if (await CheckBooksIds(booksIds))
        {
            throw new UserFriendlyException("Some Books not Existed");
        }
        //var entity = await MapToEntityAsync(input);
        var newMember = new Member
        {
            Name = input.Name,
            Description = input.Description,
        };
        if (input.Books.Any())
        {

            foreach (var bookId in input.Books)
            {

                // Create MemberBook entity
                var memberBook = new MemberBook
                {
                    BookId = bookId,
                    BorrowingDate = DateTime.Now,
                    MemberId = newMember.Id,
                };
                EntityHelper.TrySetId(memberBook, () => GuidGenerator.Create());
                // Borrow the book
                newMember.BorrowBook(memberBook);

            }
        }
        await Repository.InsertAsync(newMember);
        var result = new MemberDto
        {
            Id = newMember.Id
        };
        return result;
    }

    public override async Task<MemberDto> GetAsync(Guid id)
    {
        await CheckGetPolicyAsync();
        var member = await GetEntityByIdAsync(id);
        if (member == null)
        {
            throw new EntityNotFoundException(typeof(Member), id);
        }

        return await MapToGetOutputDtoAsync(member);
    }
    public override async Task<PagedResultDto<MemberDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        await CheckGetListPolicyAsync();

        var query = await CreateFilteredQueryAsync(input);
        var totalCount = await AsyncExecuter.CountAsync(query);

        var entities = new List<Member>();
        var entityDtos = new List<MemberDto>();
        if (totalCount > 0)
        {
            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            entities = await AsyncExecuter.ToListAsync(query);
            entityDtos = await MapToGetListOutputDtosAsync(entities);
        }

        return new PagedResultDto<MemberDto>(
            totalCount,
            entityDtos
        );


    }
    protected override async Task<IQueryable<Member>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
    {
        return (await _memberRepository.WithDetailsAsync());
    }
    private async Task<bool> CheckBooksIds(List<Guid> booksIds)
    {
        var checkedList = await _bookRepository.GetExistingBookIdsAsync(booksIds);
        return booksIds.Count() != checkedList.Count();

    }

}
