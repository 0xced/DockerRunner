using DockerRunner.Database.SqlServer;
using Xunit.Abstractions;

namespace DockerRunner.Tests.SqlServer
{
    /*
     * A System.EntryPointNotFoundException is thrown on Mono because SafeNativeMethods.GetModuleHandle calls into kernel32.dll
     * Might work again on Mono if https://github.com/dotnet/SqlClient/pull/1120 gets merged.
     * GetModuleHandle assembly:<unknown assembly> type:<unknown type> member:(null)
     *   at (wrapper managed-to-native) Microsoft.Data.Common.SafeNativeMethods.GetModuleHandle(string)
     *   at Microsoft.Data.SqlClient.InOutOfProcHelper..ctor () [0x00006] in <de6019a43b5544ecb987a2a77d294ad3>:0
     *   at Microsoft.Data.SqlClient.InOutOfProcHelper..cctor () [0x00000] in <de6019a43b5544ecb987a2a77d294ad3>:0
     */
    public class SqlServer2017Test : DockerDatabaseContainerRunnerTest<SqlServer2017Configuration>
    {
        public SqlServer2017Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        protected override bool SkipOnMono => true;
    }

    public class SqlServer2019Test : DockerDatabaseContainerRunnerTest<SqlServer2019Configuration>
    {
        public SqlServer2019Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        protected override bool SkipOnMono => true;
    }
}