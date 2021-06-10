using Xunit.Sdk;

namespace DockerRunner.Xunit
{
    /// <summary>
    /// A <see cref="DiagnosticMessage"/> implementation that overrides <see cref="ToString"/> in order to workaround a bug in Rider.
    /// <para>See https://youtrack.jetbrains.com/issue/RIDER-60811 and https://github.com/xunit/xunit/pull/2148#issuecomment-839838421</para>
    /// </summary>
    internal class PrintableDiagnosticMessage : DiagnosticMessage
    {
        public PrintableDiagnosticMessage(string message) : base(message)
        {
        }

        public override string ToString() => Message;
    }
}