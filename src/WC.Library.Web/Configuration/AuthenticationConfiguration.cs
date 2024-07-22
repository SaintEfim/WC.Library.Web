using Microsoft.Extensions.Configuration;

namespace WC.Library.Web.Configuration;

public class AuthenticationConfiguration
{
    public AuthenticationConfiguration(
        IConfiguration config)
    {
        config.GetSection("AuthenticationConfiguration")
            .Bind(this);
    }

    public string AccessSecretKey { get; set; } = string.Empty;
}
