using System.Net;
using System.Net.Http;
using Questionarios.PortalAdmin.Models;
using Questionarios.PortalAdmin.Services;

namespace Questionarios.Tests.Unit.PortalAdmin;

public class QuestionariosApiClientTests
{
    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenUnauthorized()
    {
        var handler = new FakeHandler(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var client = new QuestionariosApiClient(http);

        var result = await client.LoginAsync("user@test.com", "bad-password", CancellationToken.None);

        Assert.Null(result);
        Assert.Equal("/api/users/login", handler.LastRequest?.RequestUri?.AbsolutePath);
        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public HttpRequestMessage? LastRequest { get; private set; }

        public FakeHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(_response);
        }
    }
}
