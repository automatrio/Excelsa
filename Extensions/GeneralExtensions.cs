using System;
using System.Text;

namespace Excelsa.Extensions
{
    public static class GeneralExtensions
    {
        public static event EventHandler<LogEventArgs> OnLogAppended;

        public static StringBuilder Break(this StringBuilder builder)
        {
            return builder.Append("<br>");
        }

        public static StringBuilder AppendSuccess(this StringBuilder builder, string info)
        {
            builder.Append("<small class='success-item'>");
            builder.Append(info);
            builder.Append("</small>").Break();
            return builder;
        }

        public static StringBuilder AppendError(this StringBuilder builder, string info)
        {
            OnLogAppended?.Invoke(null, new LogEventArgs()
            {
                Type = LogEntryType.Error
            });

            builder.Append("<small class='error-item'>");
            builder.Append(info);
            builder.Append("</small>").Break();
            return builder;
        }

        public static StringBuilder AppendInfo(this StringBuilder builder, string info)
        {
            builder.Append("<small style='color: rgb(16, 104, 145)'>");
            builder.Append(info);
            builder.Append("</small>").Break();
            return builder;
        }

        public static StringBuilder AppendWarning(this StringBuilder builder, string info)
        {
            OnLogAppended?.Invoke(null, new LogEventArgs()
            {
                Type = LogEntryType.Warning
            });

            builder.Append("<small class='warning-item'>");
            builder.Append(info);
            builder.Append("</small>").Break();
            return builder;
        }

        public static StringBuilder OpenList(this StringBuilder builder)
        {
            return builder.Append("<ul>");
        }

        public static StringBuilder CloseList(this StringBuilder builder)
        {
            return builder.Append("</ul>");
        }

        public static StringBuilder AppendListItem(this StringBuilder builder, string info, LogEntryType type)
        {
            builder.Append("<li>");
            switch (type)
            {
                case LogEntryType.Success:
                    builder.AppendSuccess(info);
                    break;
                case LogEntryType.Info:
                    builder.AppendInfo(info);
                    break;
                case LogEntryType.Error:
                    builder.AppendError(info);
                    break;
                case LogEntryType.Warning:
                    builder.AppendWarning(info);
                    break;
                default:
                    builder.Append(info);
                    break;
            }
            builder.Append("</li>");
            return builder;
        }

    }

    public enum LogEntryType
    {
        Success,
        Error,
        Info,
        Default,
        Warning
    }

    public class LogEventArgs : EventArgs
    {
        public LogEntryType Type { get; set; }
    }
}
