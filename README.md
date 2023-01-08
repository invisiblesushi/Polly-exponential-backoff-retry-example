# Polly-exponential-backoff-retry-example
Application that implements exponential backoff and retry when an http reqest fails.
Utilizing Polly libary and https://uselessfacts.jsph.pl/ api.

        AsyncRetryPolicy<HttpResponseMessage> httpWaitAndRetryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(retryCount, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), onRetry:(exception, sleepDuration, attemptNumber, context) =>
            {
                Console.WriteLine($"Retrying '{url}' in {sleepDuration}. {attemptNumber}");
            });
