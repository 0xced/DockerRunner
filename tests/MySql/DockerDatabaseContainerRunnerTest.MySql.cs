using DockerRunner.Database.MySql;
using Xunit.Abstractions;

namespace DockerRunner.Tests.MySql
{
    public class MariaDbAlphaTest : DockerDatabaseContainerRunnerTest<MariaDbAlphaConfiguration>
    {
        public MariaDbAlphaTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MariaDbTest : DockerDatabaseContainerRunnerTest<MariaDbConfiguration>
    {
        public MariaDbTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MariaDb104Test : DockerDatabaseContainerRunnerTest<MariaDb104Configuration>
    {
        public MariaDb104Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MariaDb103Test : DockerDatabaseContainerRunnerTest<MariaDb103Configuration>
    {
        public MariaDb103Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MariaDb102Test : DockerDatabaseContainerRunnerTest<MariaDb102Configuration>
    {
        public MariaDb102Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MySqlServerTest : DockerDatabaseContainerRunnerTest<MySqlServerConfiguration>
    {
        public MySqlServerTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MySqlServer56Test : DockerDatabaseContainerRunnerTest<MySqlServer56Configuration>
    {
        public MySqlServer56Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}
    }

    public class MySqlServer57Test : DockerDatabaseContainerRunnerTest<MySqlServer57Configuration>
    {
        public MySqlServer57Test(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}

        // This consistently fails with the following exception:
        // System.TimeoutException : Database was not available on "Server=127.0.0.1;Port=49211;Database=database;User Id=root;***" after waiting for 30.0 seconds.
        // ---- MySqlConnector.MySqlException : SSL Authentication Error
        // -------- System.Security.Authentication.AuthenticationException : Authentication failed, see inner exception.
        // ------------ Interop+OpenSsl+SslException : SSL Handshake failed with OpenSSL error - SSL_ERROR_SSL.
        // ---------------- Interop+Crypto+OpenSslCryptographicException : error:14094410:SSL routines:ssl3_read_bytes:sslv3 alert handshake failure
        protected override bool SkipOnGitHubActions => true;
    }
}