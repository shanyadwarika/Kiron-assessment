using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class NavigationRepository : Repository<BankHoliday>
{
    private readonly KironDbContext _context;
    private readonly ICacheService _cacheService;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public NavigationRepository(
        KironDbContext context,
        ICacheService cacheService)
        : base(context)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<List<Navigation>> GetNavigationAsync()
    {
        const string cacheKey = "Navigation_All";

        var cachedData = _cacheService.Get<List<Navigation>>(cacheKey);
        if (cachedData != null)
        {
            return cachedData;
        }

        await _semaphore.WaitAsync();
        try
        {
            // Double check cache inside lock
            cachedData = _cacheService.Get<List<Navigation>>(cacheKey);
            if (cachedData != null)
            {
                return cachedData;
            }

            var navigation = await _context.Navigation
            .Where(n => n.IsVisible)
            .Include(n => n.Children)
            .ToListAsync();

            var hierarchy = BuildHierarchy(navigation);

            _cacheService.Set(cacheKey, hierarchy, TimeSpan.FromMinutes(30));
            return hierarchy;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static List<Navigation> BuildHierarchy(List<Navigation> flatList)
    {
        var lookup = flatList.ToLookup(n => n.ParentId);
        return BuildTree(lookup, null);
    }

    private static List<Navigation> BuildTree(ILookup<int?, Navigation> lookup, int? parentId)
    {
        return lookup[parentId]
            .Select(n =>
            {
                n.Children = BuildTree(lookup, n.Id);
                return n;
            })
            .ToList();
    }


}
