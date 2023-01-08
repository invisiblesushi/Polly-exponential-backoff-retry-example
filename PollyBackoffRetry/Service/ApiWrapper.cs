using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using PollyBackoffRetry.Model;

namespace PollyBackoffRetry.Service;

public class ApiWrapper
{
    private static HttpClient? _httpClient;
    private readonly Uri _apiUrlBase;

    public ApiWrapper()
    {
        _apiUrlBase = new Uri("https://uselessfacts.jsph.pl/");
        _httpClient = new HttpClient();
    }

    public async Task<Fact> FetchFact()
    {
        var url = $@"{_apiUrlBase}random.json?language=en";

        return await FetchResult<Fact>(url);
    }
    
    // Example reqest that gives a 404 response
    public async Task<Fact> Fetch404Response()
    {
        var url = $@"{_apiUrlBase}random123.json?language=en";

        return await FetchResult<Fact>(url);
    }


    private async Task<TResult> FetchResult<TResult>(string url)
    {
        var response = await GetRequestAsync(url);
        var json = JsonConvert.DeserializeObject<JToken>(response) ?? new object().ToString();
        var result = JsonConvert.DeserializeObject<TResult>(json.ToString());
        
        return result!;
    }
    
    private static async Task<string> GetRequestAsync(string url)
    { 
        HttpResponseMessage response = null;
        
        // Wait and retry policy with exponential backoff
        int retryCount = 4;
        AsyncRetryPolicy<HttpResponseMessage> httpWaitAndRetryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(retryCount, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), onRetry:(exception, sleepDuration, attemptNumber, context) =>
            {
                Console.WriteLine($"Retrying '{url}' in {sleepDuration}. {attemptNumber}");
            });
        
        response = await httpWaitAndRetryPolicy.ExecuteAsync(() => _httpClient.GetAsync(url));

        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException("Request failed");
        }

        return response!.Content.ReadAsStringAsync().Result;
    }
}