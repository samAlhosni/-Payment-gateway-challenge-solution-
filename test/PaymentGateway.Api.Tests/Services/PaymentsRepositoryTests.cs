using FluentAssertions;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services;

public class PaymentsRepositoryTests
{
    [Fact]
    public void Add_ShouldStorePayment()
    {
        var repository = new PaymentsRepository();

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

        repository.Add(payment);

        var result = repository.Get(payment.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(payment.Id);
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenPaymentDoesNotExist()
    {
        var repository = new PaymentsRepository();

        var result = repository.Get(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public void Add_ShouldAllowMultiplePayments()
    {
        var repository = new PaymentsRepository();

        var payment1 = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = "1111",
            ExpiryMonth = 1,
            ExpiryYear = 2027,
            Currency = "USD",
            Amount = 100
        };

        var payment2 = new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Declined,
            CardNumberLastFour = "2222",
            ExpiryMonth = 2,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 200
        };

        repository.Add(payment1);
        repository.Add(payment2);

        repository.Get(payment1.Id).Should().NotBeNull();
        repository.Get(payment2.Id).Should().NotBeNull();
    }
}