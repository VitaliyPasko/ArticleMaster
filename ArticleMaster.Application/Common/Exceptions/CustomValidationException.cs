namespace ArticleMaster.Application.Common.Exceptions;

public class CustomValidationException : Exception
{
    public CustomValidationException(string name, object key)
        : base($"Данные \"{name}\" ({key}) невалидны.") { }
}