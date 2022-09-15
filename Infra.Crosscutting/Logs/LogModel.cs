using FluentValidation.Results;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Infra.Crosscutting.Logs
{
    [ExcludeFromCodeCoverage]
    public class LogModel : ILogModel
    {
        private string MethodName { get; set; }
        private string Message { get; set; }
        private LogType Type { get; set; }

        public LogModel()
        {
        }

        public void RecLog(LogType type, string message, object parameters, [CallerMemberName] string methodName = "")
        {
            MethodName = methodName;
            Message = message;
            if (parameters != null)
                Message += $" [Parameters: { JsonConvert.SerializeObject(parameters) }]";
            Type = type;
            RecLog();
        }

        public void RecLog(LogType type, string message, [CallerMemberName] string methodName = "")
        {
            MethodName = methodName;
            Message = message;
            Type = type;
            RecLog();
        }

        public void RecLog(ValidationResult validation, object parameters, [CallerMemberName] string methodName = "")
        {
            List<string> errorList = new List<string>();
            foreach (var error in validation.Errors)
            {
                errorList.Add(error.ErrorMessage);
            }

            MethodName = methodName;
            Message = string.Join(", ", errorList);
            if (parameters != null)
                Message += $" [Parameters: { JsonConvert.SerializeObject(parameters) }]";
            Type = LogType.LogError;
            RecLog();
        }

        public void RecLog(Exception ex, object parameters = null, [CallerMemberName] string methodName = "")
        {
            MethodName = methodName;
            Message = $"{ JsonConvert.SerializeObject(new { Exception = ex }) }";
            if (parameters != null)
                Message += $" [Parameters: { JsonConvert.SerializeObject(parameters) }]";
            Type = LogType.LogError;
            RecLog();
        }

        public void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }

        private void RecLog()
        {
            var template = "{MethodName} - {Message}";

            switch (Type)
            {
                case LogType.LogInformation:
                    Log.Information(template, MethodName, Message);
                    break;
                case LogType.LogDebug:
                    Log.Debug(template, MethodName, Message);
                    break;
                case LogType.LogError:
                    Log.Error(template, MethodName, Message);
                    break;
                case LogType.LogWarning:
                    Log.Warning(template, MethodName, Message);
                    break;
            }
        }
    }
}