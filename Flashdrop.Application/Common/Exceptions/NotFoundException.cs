namespace Flashdrop.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.") { }
}
