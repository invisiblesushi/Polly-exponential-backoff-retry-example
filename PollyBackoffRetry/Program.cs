using PollyBackoffRetry.Model;
using PollyBackoffRetry.Service;

namespace PollyBackoffRetry;

public class Program
{
    private static ApiWrapper _apiWrapper;

    public static void Main(string[] args)
    {
        _apiWrapper = new ApiWrapper();

        var fact = _apiWrapper.FetchFact().Result;
        var errorResponse = _apiWrapper.Fetch404Response().Result;

        Console.WriteLine($"Fact: {fact.text}, Url: {fact.source_url}, Id: {fact.id}");
    }
}