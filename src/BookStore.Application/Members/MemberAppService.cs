using BookStore.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public MemberAppService(IMemberRepository memberRepository) : base(memberRepository)
    {
        GetPolicyName = BookStorePermissions.Members.Default;
        GetListPolicyName = BookStorePermissions.Members.Default;
        CreatePolicyName = BookStorePermissions.Members.Create;
        UpdatePolicyName = BookStorePermissions.Members.Edit;
        DeletePolicyName = BookStorePermissions.Members.Delete;
        _memberRepository = memberRepository;
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
        var entity = await MapToEntityAsync(input);
        if (input.Books.Any())
        {

            foreach (var item in entity.BorrowedBooks)
            {
                EntityHelper.TrySetId(item, () => GuidGenerator.Create());
                item.MemberId = entity.Id;
            }
        }
        await Repository.InsertAsync(entity);
        return await MapToGetOutputDtoAsync(entity);
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
}
