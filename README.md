# HackerNews API sample

This project is a simple sample application that demonstrates how to interact with the HackerNews API.
It is built using .NET 10.0 and requires C# 14.0 or later.

## Running the application

To run the application, execute the following command in the terminal:

```bash
dotnet run
```

While the application is running, you can access the HackerNews API endpoints to fetch data.

E.g. to fetch top 3 stories, you can call the following endpoint: [http://localhost:5026/best?count=3]()

## Assumptions
1. We cache the results of the API calls for 1 minute to avoid making repeated requests for the same data.
2. Stories we receive from `beststories.json` are already sorted by score, so we don't need to sort them again.
3. HackerNews API returns a well-formed JSON response with all the required fields.
4. We are fine with caching the results for 1 minute, even if the data changes during that time.

## Improvements
In a fullness of time, there are a few improvements that could be made to the application:
1. Instead of accessing the HackerNews API with unbounded concurrency, we could implement a rate limiter to prevent exceeding API rate limits.
2. Possible failures are not handled gracefully. Should implement proper error handling and logging, potentially with a library like Polly.
3. The application could benefit from additional unit and integration tests to ensure reliability and prevent regressions.
4. Add logging to track the application's behavior and performance.
5. If we want to scale horizontally, we could consider using a distributed cache like Redis to share cached data across multiple instances of the application.
