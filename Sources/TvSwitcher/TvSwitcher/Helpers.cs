using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;

namespace TvSwitcher
{
    /// <summary>
    /// Утилиты
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// Добавить данные в POST-запрос
        /// </summary>
        /// <param name="request"></param>
        /// <param name="postData"></param>
        public static void AppendPostData([NotNull]this HttpWebRequest request, [NotNull]string postData)
        {
            var postDataBytes = System.Text.Encoding.UTF8.GetBytes(postData);
            request.ContentLength = postDataBytes.Length;
            var stream = request.GetRequestStream().AssertNull();
            stream.Write(postDataBytes, 0, postDataBytes.Length);
            stream.Close();
        }
    }
}