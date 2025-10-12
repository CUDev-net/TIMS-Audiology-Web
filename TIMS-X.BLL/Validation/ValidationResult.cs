using System;

namespace TIMS_X.BLL.Validation;

public enum Severity
{
    Warning,
    Error
}

public class ValidationResult : IEquatable<ValidationResult>
{
    public ValidationResult(string message, Severity severity)
    {
        Message = message;
        Severity = severity;
    }

    public string Message { get; }
    public Severity Severity { get; }

    public bool Equals(ValidationResult other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Message == other.Message && Severity == other.Severity;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ValidationResult)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Message, (int)Severity);
    }
}