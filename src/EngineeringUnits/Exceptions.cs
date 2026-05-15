using System;

namespace EngineeringUnits
{
    /// <summary>Base class for all exceptions thrown by this library.</summary>
    public class EngineeringUnitException : Exception
    {
        public EngineeringUnitException(string message) : base(message) { }
        public EngineeringUnitException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Thrown when an operation requires two quantities of the same dimension but they
    /// differ (e.g. adding a Length to a Time, or calling <c>As("ft")</c> on a Mass).
    /// </summary>
    public sealed class DimensionMismatchException : EngineeringUnitException
    {
        public DimensionSignature Expected { get; }
        public DimensionSignature Actual { get; }

        public DimensionMismatchException(string message) : base(message)
        {
            Expected = default;
            Actual = default;
        }

        public DimensionMismatchException(DimensionSignature expected, DimensionSignature actual)
            : base($"Dimension mismatch: expected {expected}, got {actual}")
        {
            Expected = expected;
            Actual = actual;
        }
    }

    /// <summary>Thrown when a unit symbol is not registered in the catalog.</summary>
    public sealed class UnknownUnitException : EngineeringUnitException
    {
        public string Symbol { get; }

        public UnknownUnitException(string symbol)
            : base($"Unknown unit symbol: '{symbol}'.")
        {
            Symbol = symbol;
        }
    }

    /// <summary>Thrown when a value+unit string cannot be parsed.</summary>
    public sealed class UnitParseException : EngineeringUnitException
    {
        public string Input { get; }

        public UnitParseException(string input, string reason)
            : base($"Could not parse '{input}' — {reason}.")
        {
            Input = input;
        }
    }
}
