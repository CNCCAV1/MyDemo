using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public static class EnumTypeExtention
    {

        public static T GetEnumByName<T>(this string name)
        {
            foreach (var memberInfo in typeof(T).GetMembers())
            {
                foreach (var attr in memberInfo.GetCustomAttributes(true))
                {
                    var test = attr as DisplayAttribute;

                    if (test == null) continue;

                    if (test.Name == name)
                    {
                        var result = (T)Enum.Parse(typeof(T), memberInfo.Name);

                        return result;
                    }
                }
            }

            return default(T);
        }

        public static string GetEnumName<T>(this T type, Enum enm) where T : Type
        {
            foreach (var memberInfo in type.GetMembers())
            {
                foreach (var attr in memberInfo.GetCustomAttributes(true))
                {
                    var test = attr as DisplayAttribute;

                    if (test == null) continue;

                    if (memberInfo.Name == enm.ToString())
                    {
                        return test.Name;
                    }
                }
            }

            return null;
        }

    }
}
