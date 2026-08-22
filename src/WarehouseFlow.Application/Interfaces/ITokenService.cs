namespace WarehouseFlow.Application.Interfaces;

public interface ITokenService
{
	string GenerateToken(
		string userId,
		string email,
		string userName,
		string role
	);
}