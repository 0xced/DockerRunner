using System;
using System.Data.Common;

namespace DockerRunner
{
    /// <summary>
    /// Event arguments raised when a <see cref="DbCommand"/> is executing as part of <see cref="DockerDatabaseContainerRunner"/> startup.
    /// </summary>
    public class DbCommandEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="DbCommand"/> that is going to be executed.
        /// </summary>
        public DbCommand DbCommand { get; }

        /// <summary>
        /// Initialize a new instance of the <see cref="DbCommandEventArgs"/> class.
        /// </summary>
        /// <param name="dbCommand">The <see cref="DbCommand"/> that is going to be executed.</param>
        public DbCommandEventArgs(DbCommand dbCommand)
        {
            DbCommand = dbCommand ?? throw new ArgumentNullException(nameof(dbCommand));
        }
    }
}