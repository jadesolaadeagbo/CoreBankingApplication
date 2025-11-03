using CoreBanking.Application.Common.Models;
using CoreBanking.Core.Interfaces;
using MediatR;

namespace CoreBanking.Application.Customers.Queries.GetCustomerDetails;

public class GetCustomerDetailsQueryHandler
    : IRequestHandler<GetCustomerDetailsQuery, Result<CustomerDetailsDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerDetailsQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDetailsDto>> Handle(
        GetCustomerDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

        if (customer is null)
        {
            return Result<CustomerDetailsDto>.Failure("Customer not found");
        }

        var dto = new CustomerDetailsDto
        {
            CustomerId = customer.CustomerId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.PhoneNumber,
            DateRegistered = customer.DateCreated,
            IsActive = customer.IsActive
        };

        return Result<CustomerDetailsDto>.Success(dto);
    }
}
