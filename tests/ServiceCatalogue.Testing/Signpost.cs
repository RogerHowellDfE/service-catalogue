using System.Runtime.ExceptionServices;

namespace ServiceCatalogue.Testing;

/// <summary>
/// Wraps the assertions of a test that protects a documented decision, so a failure says which decision and where to read about it.
/// </summary>
public static class Signpost
{
    public static void Check(string decision, Action assertion)
    {
        ArgumentNullException.ThrowIfNull(assertion);

        try
        {
            assertion();
        }
        catch (Exception exception)
        {
            throw Violated(decision, exception);
        }
    }

    public static async Task CheckAsync(string decision, Func<Task> assertion)
    {
        ArgumentNullException.ThrowIfNull(assertion);

        try
        {
            await assertion().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            throw Violated(decision, exception);
        }
    }

    private static DecisionViolatedException Violated(string decision, Exception exception) =>
        new($"This protects a documented decision. Read it before changing the code or the test.{Environment.NewLine}{decision}{Environment.NewLine}{Environment.NewLine}{exception.Message}", exception);
}

public sealed class DecisionViolatedException : Exception
{
    public DecisionViolatedException()
    {
    }

    public DecisionViolatedException(string message)
        : base(message)
    {
    }

    public DecisionViolatedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
