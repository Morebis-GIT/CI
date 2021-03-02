using System;
using System.Collections.Generic;
using System.Text;

namespace xggameplan.AuditEvents.ValueConverter
{
    public class ExceptionToHTMLConverter : IValueConverter
    {
        public bool CanConvert(Type fromType, Type toType)
        {
            return fromType == typeof(ExceptionModel) && toType == typeof(String);
        }

        public object Convert(object value, Type fromType, Type toType)
        {
            if (fromType == toType)
            {
                return value;
            }
            StringBuilder html = new StringBuilder("");
            ExceptionModel currentException = (ExceptionModel)value;
            int level = 0;
            while (currentException != null)
            {
                level++;
                if (level > 1)
                {
                    _ = html.Append("<BR/>");
                }
                _ = html.Append(string.Format("<B>Exception {0}</B><BR/>" +
                    "<table>" +
                    "<tr><td>Message</td><td>{1}</td></tr>" +
                    "<tr><td>Source</td><td>{2}</td></tr>" +
                    "<tr><td>HResult</td><td>{3}</td></tr>" +
                    "<tr><td>Help</td><td>{4}</td></tr>" +
                    "<tr><td>Stack</td><td>{5}</td></tr>" +
                    "</table>", level, currentException.Message, currentException.Source, currentException.HResult, currentException.HelpLink, currentException.StackTrace));
                currentException = currentException.InnerException;
            }
            return html.ToString();
        }
    }

    public class MappingConverter<T1, T2> : IValueConverter
    {
        private Dictionary<T1, T2> _list = null;

        public MappingConverter(Dictionary<T1, T2> list)
        {
            _list = list;
        }

        public bool CanConvert(Type fromType, Type toType)
        {
            return fromType == typeof(int) && toType == typeof(String);
        }

        public object Convert(object value, Type fromType, Type toType)
        {
            if (fromType == toType)
            {
                return value;
            }
            if (_list.ContainsKey((T1)value))
            {
                return _list[(T1)value];
            }
            return default(T2);
        }
    }

}
