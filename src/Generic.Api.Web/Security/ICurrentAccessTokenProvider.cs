namespace Generic.Api.Web.Security;

public interface ICurrentAccessTokenProvider
{
    string GetCurrentToken();
}
