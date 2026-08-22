namespace WarehouseFlow.Application.Interfaces;

using WarehouseFlow.Application.Dtos;

public interface IAuthenticationService
{
	Task<LoginResponse> LoginAsync(
		LoginRequest request,
		CancellationToken cancellationToken = default
	);

	Task<CreatedUserResponse> RegisterCustomerAsync(
		RegisterCustomerDto registration,
		CancellationToken cancellationToken = default
	);

    Task<CreatedUserResponse> RegisterEmployeeAsync(
		CreateEmployeeUserDto employeeUserDto,
		string createdByUserId,
		CancellationToken cancellationToken = default
	);
}
