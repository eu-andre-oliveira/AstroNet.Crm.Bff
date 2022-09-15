using FluentValidation.Results;
using System;
using System.Runtime.CompilerServices;

namespace Infra.Crosscutting.Logs
{
    public interface ILogModel
    {
        void RecLog(LogType type, string message, [CallerMemberName] string methodName = "");
        void RecLog(LogType type, string message, object parameters, [CallerMemberName] string methodName = "");
        void RecLog(Exception ex, object parameters = null, [CallerMemberName] string methodName = "");
        void RecLog(ValidationResult validation, object parameters = null, [CallerMemberName] string methodName = "");
        void CloseAndFlush();
    }
}