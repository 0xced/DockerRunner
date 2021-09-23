using DockerRunner.Database.SqlServer;
using Xunit.Abstractions;

namespace DockerRunner.Tests.SqlServer
{
    /*
     * 2021-06-18: A System.EntryPointNotFoundException is thrown on Mono because SafeNativeMethods.GetModuleHandle calls into kernel32.dll
     * Might work again on Mono if https://github.com/dotnet/SqlClient/pull/1120 gets merged.
     * GetModuleHandle assembly:<unknown assembly> type:<unknown type> member:(null)
     *   at (wrapper managed-to-native) Microsoft.Data.Common.SafeNativeMethods.GetModuleHandle(string)
     *   at Microsoft.Data.SqlClient.InOutOfProcHelper..ctor () [0x00006] in <de6019a43b5544ecb987a2a77d294ad3>:0
     *   at Microsoft.Data.SqlClient.InOutOfProcHelper..cctor () [0x00000] in <de6019a43b5544ecb987a2a77d294ad3>:0
     * 2021-09-23: Version 4.0.0-preview2.21264.2 of Microsoft.Data.SqlClient fixed this issue, but another one popped:
     * see https://github.com/dotnet/SqlClient/issues/1263#issuecomment-926093690
     */
    public class SqlServer2017Test : DockerDatabaseContainerRunnerTest<SqlServer2017Configuration>
    {
        public SqlServer2017Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        // commenting out to see what happens with Microsoft.Data.SqlClient v4 on Mono
        // protected override bool SkipOnMono => true;
    }

    public class SqlServer2019Test : DockerDatabaseContainerRunnerTest<SqlServer2019Configuration>
    {
        public SqlServer2019Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        // commenting out to see what happens with Microsoft.Data.SqlClient v4 on Mono
        // protected override bool SkipOnMono => true;
    }
}