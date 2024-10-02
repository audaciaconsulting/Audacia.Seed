namespace Audacia.Seed.Exceptions;

/// <summary>
/// Exception to be thrown for handled errors to do with data seeding.
/// </summary>
public class DataSeedingException : Exception
{
    private DataSeedingException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSeedingException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public DataSeedingException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSeedingException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">Exception that was thrown to trigger this exception.</param>
    public DataSeedingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}