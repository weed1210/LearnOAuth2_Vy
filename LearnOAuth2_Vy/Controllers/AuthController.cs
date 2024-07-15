using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace LearnOAuth2_Vy.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var session = HttpContext.Session;
        // Unset access token
        session.Remove("access_token");

        // Generate a random state and store it in the session
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[16];
            rng.GetBytes(randomBytes);
            string state = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
            session.SetString("state", state);
        }

        var parameters = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", _config["Auth:ClientId"] },
            { "redirect_uri", _config["Auth:BaseUrl"] },
            { "scope", "user public_repo" },
            { "state", session.Get("state").ToString() }
        };

        // Construct the URL
        var query = string.Join("&", parameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
        var redirectUrl = $"{_config["Auth:AuthUrl"]}?{query}";

        // Redirect the user to GitHub's authorization page
        return Redirect(redirectUrl);
    }

    string ToQueryString(Dictionary<string, string> dict)
    {
        List<string> items = new List<string>();
        foreach (var kvp in dict)
        {
            items.Add($"{System.Uri.EscapeDataString(kvp.Key)}={System.Uri.EscapeDataString(kvp.Value)}");
        }
        return string.Join("&", items);
    }
}
