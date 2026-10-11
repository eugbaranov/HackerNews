using HackerNews.Data;
using NFluent;
using NSubstitute;

internal sealed class HackerNewsApiTests
{
    private IHttpClientFactory _httpClientFactory;
    private HackerNewsApi _sut;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _sut = new HackerNewsApi(_httpClientFactory);
    }

    [Test]
    public async Task GetStoryAsyncTest()
    {
        // Arrange
        var storyJson = """
            {
                "by":"ismaildonmez",
                "descendants":588,
                "score":1757,
                "time":1570887781,
                "title":"A uBlock Origin update was rejected from the Chrome Web Store",
                "url":"https://github.com/uBlockOrigin/uBlock-issues/issues/745"
            }
            """;
        _httpClientFactory.CreateClient(Arg.Any<string>())
            .Returns(new HttpClient(new StaticResponseHttpMessageHandler(storyJson)) { BaseAddress = new Uri("https://localhost:1234/") });

        var expected = new Story(
            "A uBlock Origin update was rejected from the Chrome Web Store",
            "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
            "ismaildonmez",
            DateTimeOffset.FromUnixTimeSeconds(1570887781),
            1757,
            588);

        // Act
        var actual = await _sut.GetStoryAsync(123);

        // Assert
        Check.That(actual).Is(expected);
    }

    private sealed class StaticResponseHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;

        public StaticResponseHttpMessageHandler(string responseContent)
        {
            _responseContent = responseContent ?? throw new ArgumentNullException(nameof(responseContent));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(_responseContent)
            };
            return Task.FromResult(response);
        }
    }
}
