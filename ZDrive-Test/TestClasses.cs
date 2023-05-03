using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace ZDrive_Test;

public sealed class TestHttpRequest : HttpRequest
{
    private readonly TestCookieColletion _cookie;

    public TestHttpRequest(TestCookieColletion cookie) => _cookie = cookie;

    public override HttpContext HttpContext => throw new NotImplementedException();

    public override string Method { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override string Scheme { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override bool IsHttps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override HostString Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override PathString PathBase { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override PathString Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override QueryString QueryString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override IQueryCollection Query { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override string Protocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override IHeaderDictionary Headers => throw new NotImplementedException();

    public override IRequestCookieCollection Cookies { get => _cookie; set => throw new NotImplementedException(); }
    public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override string? ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override Stream Body { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override bool HasFormContentType => throw new NotImplementedException();

    public override IFormCollection Form { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public sealed class TestCookieColletion : IRequestCookieCollection
{
    private readonly string? _ssid;

    public TestCookieColletion(string? ssid) => _ssid = ssid;
    public string? this[string key]
    {
        get { if (key == "sessionId") return _ssid; else return null;}
        set { throw new NotImplementedException(); }
    }

    public int Count => throw new NotImplementedException();

    public ICollection<string> Keys => throw new NotImplementedException();

    public bool ContainsKey(string key)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}