using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.controller
{
    public class ControllerUtils
    {
        //public const string EMPTY_STRING = string.Empty;


        public static bool isEmpty(Object value)
        {
            if (value == null)
            {
                return true;
            }
            else if (value is ICollection)
            {
                return ((ICollection)value).Count < 1;
            }
            else if (value is IDictionary)
            {
                return ((IDictionary)value).Count < 1;
            }
            else if (value is string)
            {
                return string.Empty.Equals((string)value.ToString());
            }
            return false;
        }

        public static bool isNotEmpty(Object value)
        {
            return !isEmpty(value);
        }

        public static bool showException(string exceptionMessage)
        {
            return isNotEmpty(exceptionMessage);
        }
    }
}
