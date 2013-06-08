using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaconJam2013
{
    public static class EnumUtil
    {
        public static List<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList<T>();
        }
    }
}
