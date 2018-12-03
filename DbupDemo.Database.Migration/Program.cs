using DbUp;
using DbUp.Engine;
using NDesk.Options;
using System;
using System.Data.SqlClient;
using System.IO;

namespace DbupDemo.Database.Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = "";
            var database = "";
            var directory = "";
            var username = "";
            var password = "";
            bool mark = false;
            bool prompt = false;
            var connectionString = "";

            bool show_help = false;
            bool ensure_database = false;

            var optionSet = new OptionSet() {
                { "s|server=", "the SQL Server host", s => server = s },
                { "db|database=", "database to upgrade", d => database = d},
                { "d|directory=", "directory containing SQL Update files", dir => directory = dir },
                { "e|ensure", "ensure datbase exists", e => ensure_database = e != null },
                { "u|user=", "Database username", u => username = u},
                { "p|password=", "Database password", p => password = p},
                { "cs|connectionString=", "Full connection string", cs => connectionString = cs},
                { "h|help",  "show this message and exit", v => show_help = v != null },
                {"mark", "Mark scripts as executed but take no action", m => mark = true},
                {"prompt", "Prompt user when finish", p => prompt = true},
            };

            optionSet.Parse(args);

            if (args.Length == 0)
                show_help = true;

            if (show_help)
            {
                optionSet.WriteOptionDescriptions(System.Console.Out);
                Console.Write("Press a key to quit");
                Console.ReadKey();
                return;

            }

            if (String.IsNullOrEmpty(connectionString))
            {
                connectionString = BuildConnectionString(server, database, username, password);
            }

            if (!Path.IsPathRooted(directory))
                directory = Path.Combine(Environment.CurrentDirectory, directory);

            var dbup = DeployChanges.To
                .SqlDatabase(connectionString)
                .LogScriptOutput()
                .LogToConsole()
                .LogTo(new NLogUpgrade())
                .WithScriptsFromFileSystem(directory)
                .WithExecutionTimeout(TimeSpan.FromMinutes(10))
                .Build();

            DatabaseUpgradeResult result = null;
            if (!mark)
            {
                if (ensure_database) EnsureDatabase.For.SqlDatabase(connectionString);
                result = dbup.PerformUpgrade();
            }
            else
            {
                result = dbup.MarkAsExecuted();
            }

            if (!result.Successful)
            {
                Environment.ExitCode = 1;
            }

            if (prompt)
            {
                Console.Write("Press a key to quit");
                Console.ReadKey();
            }
        }

        private static string BuildConnectionString(string server, string database, string username, string password)
        {
            var conn = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database
            };
            if (!String.IsNullOrEmpty(username))
            {
                conn.UserID = username;
                conn.Password = password;
                conn.IntegratedSecurity = false;
            }
            else
            {
                conn.IntegratedSecurity = true;
            }

            return conn.ToString();
        }
    }
}
