using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace life
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Включение двойной буферизации для элемента управления.
        /// </summary>
        /// <typeparam name="T">Тип элемента управления.</typeparam>
        /// <param name="t">Ссылка на элемент управления.</param>
        /// <param name="setting">Устанавливаемое значение.</param>
        public static void DoubleBuffered<T>(this T t, bool setting) where T:class
        {
            Type TType = t.GetType();
            PropertyInfo pi = TType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            if (pi != null)
            {
                pi.SetValue(t, setting, null);
            }
        }
    }
}
