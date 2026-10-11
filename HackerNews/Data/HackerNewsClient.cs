namespace HackerNews.Data;

public interface IHackerNewsClient
{
    Task<IReadOnlyCollection<Story>> GetBestAsync(int count);
}

internal sealed class HackerNewsClient : IHackerNewsClient
{
    private readonly IHackerNewsApi _hackerNewsApi;

    public HackerNewsClient(IHackerNewsApi hackerNewsApi)
    {
        _hackerNewsApi = hackerNewsApi ?? throw new ArgumentNullException(nameof(hackerNewsApi));
    }

    public async Task<IReadOnlyCollection<Story>> GetBestAsync(int count)
    {
        var storyIds = await _hackerNewsApi.GetBestAsync();
        var stories = await Task.WhenAll(storyIds.Take(count).Select(_hackerNewsApi.GetStoryAsync));

        return stories;
    }
}
