namespace BO;
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
}
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

[Serializable]
public class BlCanNotBeDeletedException : Exception
{
    public BlCanNotBeDeletedException(string? message) : base(message) { }
}
[Serializable]
public class BlValidationException : Exception
{
    public BlValidationException(string? message) : base(message) { }
}

[Serializable]
public class BlCanNotAssignCall : Exception
{
    public BlCanNotAssignCall(string? message) : base(message) { }
}

[Serializable]
public class BlCanNotUpdateTreatment : Exception
{
    public BlCanNotUpdateTreatment(string? message) : base(message) { }
}
[Serializable]
public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string? message) : base(message) { }
}

[Serializable]
public class BlAccessDeniedException : Exception
{
    public BlAccessDeniedException(string? message) : base(message) { }
}