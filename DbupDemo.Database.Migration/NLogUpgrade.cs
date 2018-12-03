using DbUp.Engine.Output;
using NLog;

namespace DbupDemo.Database.Migration
{
    public class NLogUpgrade : IUpgradeLog
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public void WriteError(string format, params object[] args)
        {
            logger.Error(format, args);
        }

        public void WriteInformation(string format, params object[] args)
        {
            logger.Info(format, args);
        }

        public void WriteWarning(string format, params object[] args)
        {
            logger.Warn(format, args);
        }
    }
}
