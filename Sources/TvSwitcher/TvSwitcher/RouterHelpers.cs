using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace TvSwitcher
{
    /// <summary>
    /// Утилиты работы с роутером
    /// </summary>
    internal static class RouterHelpers
    {
        /// <summary>
        /// Параметры доступа на сервер
        /// </summary>
        private static readonly string _authHeaderValue = "Basic " + Convert.ToBase64String(
            System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("admin" + ":" + "260924"));

        /// <summary>
        /// POST-запрос на установку контекстта фильтра 2
        /// </summary>
        private const string SWITCH_TO_FILTER_2_POST_DATA =
            "submit_button=Filters&action=Apply&change_action=gozila_cgi&submit_type=&numfilters=4&blocked_service=&filter_web=&filter_policy=&filter_p2p=&f_status=&f_id=2&f_status1=enable&f_name=%D0%97%D0%B0%D0%BA%D1%80%D1%8B%D1%82%D1%8C+%D0%BA%D0%B0%D0%BC%D0%B5%D1%80%D1%83&f_status2=deny&day_all=1&time_all=1&allday=";

        /// <summary>
        /// Активировать фильтр 2 (с телевизорами)
        /// </summary>
        private static void ActivateFilter()
        {
            var request = new HttpWebRequest(new Uri("http://192.168.1.1/apply.cgi", UriKind.Absolute))
            {
                Method = "POST",
                Referer = "http://192.168.1.1/apply.cgi"
            };
            request.Headers.VoidAssertNull();
            request.Headers.Add("Authorization", _authHeaderValue);
            request.AppendPostData(SWITCH_TO_FILTER_2_POST_DATA);
            var submitResponse = request.GetResponse() as HttpWebResponse;
            if (submitResponse == null || submitResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception();
        }

        /// <summary>
        /// Запросить строкой текст с MAC-адресами
        /// </summary>
        /// <returns></returns>
        [NotNull]
        private static string RequestMacsPage()
        {
            var request = new HttpWebRequest(new Uri("http://192.168.1.1/FilterIPMAC.asp", UriKind.Absolute))
            {
                Referer = "http://192.168.1.1/apply.cgi"
            };
            request.Headers.VoidAssertNull();
            request.Headers.Add("Authorization", _authHeaderValue);
            using (var reader = new StreamReader(request.GetResponse().AssertNull().GetResponseStream().AssertNull()))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Паттерн поиска MAC через регулярки
        /// </summary>
        private const string MAC_REGEX_PATTERN = "([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})";

        /// <summary>
        /// Пустой ничего не значащий MAC-адрес
        /// </summary>
        private const string VOID_MAC = "00:00:00:00:00:00";

        /// <summary>
        /// Распарсить страницу с MAC-адресами
        /// </summary>
        /// <param name="macsPageContent"></param>
        /// <returns></returns>
        [NotNull, ItemNotNull]
        private static IEnumerable<string> ParseMacsOnPage([NotNull] string macsPageContent)
        {
            var regExp = new Regex(MAC_REGEX_PATTERN);
            var found = regExp.Match(macsPageContent);
            while (found.Success)
            {
                if (found.Value != VOID_MAC)
                    yield return found.Value;
                found = found.NextMatch();
            }
        }

        /// <summary>
        /// Запросить залоченнные MAC-адреса
        /// </summary>
        /// <returns></returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<string> ReadLockedMacs()
        {
            ActivateFilter();
            return ParseMacsOnPage(RequestMacsPage());
        }

        /// <summary>
        /// Post-запрос на установку фильтров
        /// </summary>
        private const string APPLY_FILTERS_POST_DATA_PATTERN =
            "submit_button=FilterIPMAC&action=ApplyTake&change_action=&submit_type=&filter_ip_value=&filter_mac_value={0}&ip0=0&ip1=0&ip2=0&ip3=0&ip4=0&ip5=0&ip_range0_0=0&ip_range0_1=0&ip_range0_2=0&ip_range0_3=0&ip_range0_4=0&ip_range0_5=0&ip_range0_6=0&ip_range0_7=0&ip_range1_0=0&ip_range1_1=0&ip_range1_2=0&ip_range1_3=0&ip_range1_4=0&ip_range1_5=0&ip_range1_6=0&ip_range1_7=0";

        /// <summary>
        /// Установить
        /// </summary>
        /// <param name="values"></param>
        public static void WriteLockedMacs([NotNull, ItemNotNull]IEnumerable<string> values)
        {
            var request = new HttpWebRequest(new Uri("http://192.168.1.1/apply.cgi", UriKind.Absolute))
            {
                Method = "POST",
                Referer = "http://192.168.1.1/apply.cgi"
            };
            var macFilterValues = new List<string>(values);
            while (macFilterValues.Count < 8)
                macFilterValues.Add(VOID_MAC);
            var postData = string.Format(APPLY_FILTERS_POST_DATA_PATTERN, string.Join(string.Empty,
                macFilterValues
                    .Where(x => x != null)
                    .Select((x, index) => $"&mac{index}={x.Replace(":", "%3A")}").AssertNull()));
            request.Headers.VoidAssertNull();
            request.Headers.Add("Authorization", _authHeaderValue);
            request.AppendPostData(postData);
            var submitResponse = request.GetResponse() as HttpWebResponse;
            if (submitResponse == null || submitResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception();
       }
    }
}