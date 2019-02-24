using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EnglishWords
{
    /// <summary>
    /// Окружение приложения
    /// (переменные для обмена между всеми Activity)
    /// </summary>
    internal static class RuntimeEnvironment
    {
        /// <summary>
        /// Выполняемый тест
        /// </summary>
        public static Test ActiveTest { get; set; }
    }
}