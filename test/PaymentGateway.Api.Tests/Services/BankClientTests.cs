using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services;

public class BankClientTests
{
    private HttpClient CreateHttpClient(HttpStatusCode statusCode, object? responseBody = null)
    {
        var handler = new FakeHttpMessageHandler(statusCode, responseBody);
        return new HttpClient(handler);
    }

    [Fact]
    public async Task ProcessPayment_ReturnsTrue_WhenBankAuthorizes()
    {
        var response = new BankResponse
        {
            Authorized = true,
            Authorization_code = "ABC123"
        };

        var client = new BankClient(CreateHttpClient(HttpStatusCode.OK, response));

        var result = await client.ProcessPaymentAsync(
            "4111111111111111",
            12,
            2028,
            "USD",
            100,
            "123");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ProcessPayment_ReturnsFalse_WhenBankDeclines()
    {
        var response = new BankResponse
        {
            Authorized = false,
            Authorization_code = "XYZ999"
        };

        var client = new BankClient(CreateHttpClient(HttpStatusCode.OK, response));

        var result = await client.ProcessPaymentAsync(
            "4111111111111112",
            12,
            2028,
            "USD",
            100,
            "123");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessPayment_ReturnsNull_WhenBankUnavailable()
    {
        var client = new BankClient(CreateHttpClient(HttpStatusCode.ServiceUnavailable));

        var result = await client.ProcessPaymentAsync(
            "4111111111111110",
            12,
            2028,
            "USD",
            100,
            "123");

        result.Should().BeNull();
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly object? _responseBody;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, object? responseBody)
        {
            _statusCode = statusCode;
            _responseBody = responseBody;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode);

            if (_responseBody != null)
            {
                response.Content = JsonContent.Create(_responseBody);
            }

            return Task.FromResult(response);
        }
    }
}