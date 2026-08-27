namespace Flashdrop.Application.Common.Exceptions;

public class AppValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public AppValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
