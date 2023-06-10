using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rozpoznawanie_obiektów_na_zdjeciach.Helpers
{
    public static class ConventerHelper
    {
        public static decimal GetSafeDecimal(this object value)
        {
            if (value == null)
                return 0;
            else
                return Convert.ToDecimal(value);
        }
        public static int GetSafeInt(this object value)
        {
            if (value == null)
                return 0;
            else
                return Convert.ToInt32(value);
        }
        public static bool GetSafeBool(this object value)
        {
            if (value == null)
                return false;
            else
                return Convert.ToBoolean(value);
        }
        public static short GetSafeShort(this object value)
        {
            if (value == null)
                return 0;
            else
                return Convert.ToInt16(value);
        }
    }
}
