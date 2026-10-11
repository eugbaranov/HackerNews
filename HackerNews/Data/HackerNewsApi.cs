using System.Text.Json;

namespace HackerNews.Data;

public interface IHackerNewsApi
{
    Task<IReadOnlyCollection<int>> GetBestAsync();

    Task<Story> GetStoryAsync(int storyId);
}

internal sealed class HackerNewsApi : IHackerNewsApi
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HackerNewsApi(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    private async Task<T?> GetAsync<T>(string url)
    {
        var client = _httpClientFactory.CreateClient(nameof(HackerNewsApi));

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(); // Streams are more efficient but strings are better for debugging
        var result = JsonSerializer.Deserialize<T>(content);

        return result;
    }

    public async Task<IReadOnlyCollection<int>> GetBestAsync() => await GetAsync<int[]>("beststories.json") ?? [];

    public async Task<Story> GetStoryAsync(int storyId)
    {
        var story = await GetAsync<StoryDto>($"item/{storyId}.json") ?? throw new ArgumentOutOfRangeException(nameof(storyId), storyId, null);
        return story.ToStory();
    }

    /*
    {
      "by": "ismaildonmez",
      "descendants": 588,
      "id": 21233041,
      "kids": [21233229, 21233577, 21235077, 21233633, 21233159, 21233523, 21233181, 21233289, 21233377],
      "score": 1757,
      "time": 1570887781,
      "title": "A uBlock Origin update was rejected from the Chrome Web Store",
      "type": "story",
      "url": "https://github.com/uBlockOrigin/uBlock-issues/issues/745"
    }
     */
    private sealed record StoryDto(
        string by,
        int descendants,
        int score,
        long time,
        string title,
        string url)
    {
        public Story ToStory() => new(title, url, by, DateTimeOffset.FromUnixTimeSeconds(time), score, descendants);
    }
}
