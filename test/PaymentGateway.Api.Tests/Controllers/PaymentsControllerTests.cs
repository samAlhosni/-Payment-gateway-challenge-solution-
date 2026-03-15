using System.Net;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    [Fact]
    public async Task GetPayment_Returns200_WhenPaymentExists()
    {
        var payment = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = "1234",
            ExpiryMonth = 12,
            ExpiryYear = 2027,
            Currency = "USD",
            Amount = 100
        };

        var repo = new PaymentsRepository();
        repo.Add(payment);

        var factory = new WebApplicationFactory<PaymentsController>()
            .WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                    services.AddSingleton(repo)));

        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/payments/{payment.Id}");

        var json = await response.Content.ReadAsStringAsync();

        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

        var result = System.Text.Json.JsonSerializer.Deserialize<PostPaymentResponse>(
            json,
            options);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Id.Should().Be(payment.Id);
    }

    [Fact]
    public async Task GetPayment_Returns404_WhenPaymentDoesNotExist()
    {
        var factory = new WebApplicationFactory<PaymentsController>();
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/payments/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}