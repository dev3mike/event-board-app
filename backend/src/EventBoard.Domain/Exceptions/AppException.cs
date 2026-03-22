namespace EventBoard.Domain.Exceptions;

public abstract class AppException(string code, string message)
    : Exception(message)
{
    public string Code { get; } = code;
}

public class NotFoundException(string message)
    : AppException("NotFound", message);

public class BusinessRuleException(string code, string message)
    : AppException(code, message);
