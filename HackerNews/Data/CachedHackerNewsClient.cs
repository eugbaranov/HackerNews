using Microsoft.Extensions.Caching.Memory;

namespace HackerNews.Data;

internal sealed class CachedHackerNewsApi : IHackerNewsApi
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    private readonly IHackerNewsApi _hackerNewsApi;
    private readonly IMemoryCache _memoryCache;

    public CachedHackerNewsApi([FromKeyedServices("base")] IHackerNewsApi hackerNewsApi, IMemoryCache memoryCache)
    {
        _hackerNewsApi = hackerNewsApi ?? throw new ArgumentNullException(nameof(hackerNewsApi));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async Task<IReadOnlyCollection<int>> GetBestAsync()
    {
        var bestStories = await _memoryCache.GetOrCreateAsync("best", async entry =>
        {
            entry.SetAbsoluteExpiration(CacheDuration);
            return await _hackerNewsApi.GetBestAsync();
        });
        return bestStories!;
    }

    public Task<Story> GetStoryAsync(int storyId)
    {
        var story = _memoryCache.GetOrCreateAsync(storyId, async entry =>
        {
            entry.SetAbsoluteExpiration(CacheDuration);
            return await _hackerNewsApi.GetStoryAsync(storyId);
        });
        return story!;
    }
}
