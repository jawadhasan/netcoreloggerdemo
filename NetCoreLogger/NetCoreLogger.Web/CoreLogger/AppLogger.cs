using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace NetCoreLogger.Web.CoreLogger
{
    public class AppLogger
    {
        private static readonly ILogger _perfLogger;
        private static readonly ILogger _usageLogger;
        private static readonly ILogger _errorLogger;
        private static readonly ILogger _diagnosticLogger;

        static AppLogger()
        {
            _perfLogger = new LoggerConfiguration()
                .WriteTo.File(path: Environment.GetEnvironmentVariable("LOGFILE_PERF"))
                .WriteTo.Elasticsearch("http://localhost:9200",
                    indexFormat: "perf-{0:yyyy.MM.dd}",
                    inlineFields: true)
                .CreateLogger();

            _usageLogger = new LoggerConfiguration()
                 .WriteTo.File(path: Environment.GetEnvironmentVariable("LOGFILE_USAGE"))
                 .WriteTo.Elasticsearch("http://localhost:9200",
                     indexFormat: "usage-{0:yyyy.MM.dd}",
                     inlineFields: true)
                 .CreateLogger();

            _errorLogger = new LoggerConfiguration()
                 .WriteTo.File(path: Environment.GetEnvironmentVariable("LOGFILE_ERROR"))
                 .WriteTo.Elasticsearch("http://localhost:9200",
                     indexFormat: "error-{0:yyyy.MM.dd}",
                     inlineFields: true)
                 .CreateLogger();

            _diagnosticLogger = new LoggerConfiguration()
                 .WriteTo.File(path: Environment.GetEnvironmentVariable("LOGFILE_DIAG"))
                 .WriteTo.Elasticsearch("http://localhost:9200",
                     indexFormat: "diag-{0:yyyy.MM.dd}",
                     inlineFields: true)
                .CreateLogger();
        }

        public static void WritePerf(LogDetail infoToLog)
        {
            _perfLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);

            //_perfLogger.Write(LogEventLevel.Information,
            //    "{Timestamp}{Message}{Layer}{Location}{Product}" +
            //    "{CustomException}{ElapsedMilliseconds}{Exception}{Hostname}" +
            //    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
            //    infoToLog.Timestamp, infoToLog.Message,
            //    infoToLog.Layer, infoToLog.Location,
            //    infoToLog.Product, infoToLog.CustomException,
            //    infoToLog.ElapsedMilliseconds, infoToLog.Exception?.ToBetterString(),
            //    infoToLog.Hostname, infoToLog.UserId,
            //    infoToLog.UserName, infoToLog.CorrelationId,
            //    infoToLog.AdditionalInfo
            //);
        }
        public static void WriteUsage(LogDetail infoToLog)
        {

            _usageLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);

            //_usageLogger.Write(LogEventLevel.Information,
            //    "{Timestamp}{Message}{Layer}{Location}{Product}" +
            //    "{CustomException}{ElapsedMilliseconds}{Exception}{Hostname}" +
            //    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
            //    infoToLog.Timestamp, infoToLog.Message,
            //    infoToLog.Layer, infoToLog.Location,
            //    infoToLog.Product, infoToLog.CustomException,
            //    infoToLog.ElapsedMilliseconds, infoToLog.Exception?.ToBetterString(),
            //    infoToLog.Hostname, infoToLog.UserId,
            //    infoToLog.UserName, infoToLog.CorrelationId,
            //    infoToLog.AdditionalInfo
            //);

        }

        public static void WriteError(LogDetail infoToLog)
        {
            if (infoToLog.Exception != null)
            {
                var procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName) ? infoToLog.Location : procName;
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }
            _errorLogger.Write(LogEventLevel.Error, "{@LogDetail}", infoToLog);
            //_errorLogger.Write(LogEventLevel.Information,
            //    "{Timestamp}{Message}{Layer}{Location}{Product}" +
            //    "{CustomException}{ElapsedMilliseconds}{Exception}{Hostname}" +
            //    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
            //    infoToLog.Timestamp, infoToLog.Message,
            //    infoToLog.Layer, infoToLog.Location,
            //    infoToLog.Product, infoToLog.CustomException,
            //    infoToLog.ElapsedMilliseconds, infoToLog.Exception?.ToBetterString(),
            //    infoToLog.Hostname, infoToLog.UserId,
            //    infoToLog.UserName, infoToLog.CorrelationId,
            //    infoToLog.AdditionalInfo
            //);
        }
        public static void WriteDiagnostic(LogDetail infoToLog)
        {
            var writeDiagnostics =
                Convert.ToBoolean(Environment.GetEnvironmentVariable("DIAGNOSTICS_ON"));
            if (!writeDiagnostics)
                return;

            _diagnosticLogger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);

            //_diagnosticLogger.Write(LogEventLevel.Information,
            //    "{Timestamp}{Message}{Layer}{Location}{Product}" +
            //    "{CustomException}{ElapsedMilliseconds}{Exception}{Hostname}" +
            //    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
            //    infoToLog.Timestamp, infoToLog.Message,
            //    infoToLog.Layer, infoToLog.Location,
            //    infoToLog.Product, infoToLog.CustomException,
            //    infoToLog.ElapsedMilliseconds, infoToLog.Exception?.ToBetterString(),
            //    infoToLog.Hostname, infoToLog.UserId,
            //    infoToLog.UserName, infoToLog.CorrelationId,
            //    infoToLog.AdditionalInfo
            //);


        }
        private static string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }
        private static string FindProcName(Exception ex)
        {
            //var sqlEx = ex as SqlException;
            //if (sqlEx != null)
            //{
            //    var procName = sqlEx.Procedure;
            //    if (!string.IsNullOrEmpty(procName))
            //        return procName;
            //}

            //if (!string.IsNullOrEmpty((string)ex.Data["Procedure"]))
            //{
            //    return (string)ex.Data["Procedure"];
            //}

            if (ex.InnerException != null)
                return FindProcName(ex.InnerException);

            return null;
        }
    }
}
