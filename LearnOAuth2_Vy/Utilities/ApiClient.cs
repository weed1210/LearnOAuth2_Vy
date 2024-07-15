using System.Net.Http.Headers;
using System.Text.Json;

namespace LearnOAuth2_Vy.Utilities;

public class ApiClient
{
    private static readonly HttpClient client = new HttpClient();
    private string accessToken;

    public ApiClient(string accessToken)
    {
        this.accessToken = accessToken;
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd("https://example-app.com/");
    
    }

    public async Task<Dictionary<string, object>> ApiRequest(string url, Dictionary<string, string> post = null)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        HttpResponseMessage response;

        if (post != null)
        {
            var content = new FormUrlEncodedContent(post);
            response = await client.PostAsync(url, content);
        }
        else
        {
            response = await client.GetAsync(url);
        }

        response.EnsureSuccessStatusCode();

        string responseData = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Dictionary<string, object>>(responseData);
    }
}
