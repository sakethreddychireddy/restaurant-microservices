using System.Data.Common;


namespace AuthService.Domain.Exceptions
{
    public class DbExceptions(string message) : Exception(message);
    public class EmailAlreadyExistsException(string email)
    : DbException($"A user with email '{email}' already exists.");
    public class InvalidCredentialsException()
    : DbException("Invalid email or password.");

    public class UserNotFoundException(int id)
        : DbException($"User with ID {id} was not found.");
}
