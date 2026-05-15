namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Polyfill for the C# 9 <c>init</c> accessor on .NET Standard 2.0.
    /// The compiler requires this type to be present; the runtime never references it.
    /// </summary>
    internal static class IsExternalInit { }
}
