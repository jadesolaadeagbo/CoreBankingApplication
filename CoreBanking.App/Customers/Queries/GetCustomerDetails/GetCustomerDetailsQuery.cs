using CoreBanking.Application.Accounts.Queries.GetAccountDetails;
using CoreBanking.Application.Common.Interfaces;
using CoreBanking.Application.Common.Models;
using CoreBanking.Application.Customers.Queries.GetCustomers;
using CoreBanking.Core.Interfaces;
using CoreBanking.Core.ValueObjects;
using MediatR;

namespace CoreBanking.Application.Customers.Queries.GetCustomerDetails;

public record GetCustomerDetailsQuery : IQuery<CustomerDetailsDto>
{
    public CustomerId CustomerId { get; init; }
}


public class GetCustomersQueryHandler : IRequestHandler<GetCustomerDetailsQuery, Result<CustomerDetailsDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDetailsDto>> Handle(GetCustomerDetailsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

        var customerDetailsDto = new CustomerDetailsDto
        {
            CustomerId = customer.CustomerId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.PhoneNumber,
            DateRegistered = customer.DateCreated,
            IsActive = customer.IsActive
        };

        return Result<CustomerDetailsDto>.Success(customerDetailsDto);
    }


}
