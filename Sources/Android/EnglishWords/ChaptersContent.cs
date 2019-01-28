using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using JetBrains.Annotations;
using Environment = System.Environment;

namespace EnglishWords
{
    /// <summary>
    /// Контент глав
    /// </summary>
    internal static class ChaptersContent
    {
        /// <summary>
        /// Загрузить демоснтрационные главы
        /// </summary>
        /// <returns></returns>
        [NotNull, ItemNotNull]
        private static IEnumerable<TestChapter> LoadDemoChapters()
        {
            yield return new TestChapter("Первая глава", 
                ("girl", "чувиха"), 
                ("boy", "чувак"),
                ("blue", "синий"),
                ("alien", "чужой"),
                ("small", "маленький"));
            yield return new TestChapter("Вторая глава",
                ("girl1", "чувиха1"),
                ("boy1", "чувак1"),
                ("blue1", "синий1"),
                ("alien1", "чужой1"),
                ("small1", "маленький1"));
            yield return new TestChapter("Третья глава",
                ("girl1", "чувиха2"),
                ("boy1", "чувак2"),
                ("blue1", "синий2"),
                ("alien1", "чужой2"),
                ("small1", "маленький2"));
        }


        /// <summary>
        /// Попытаться загрузить контент с удаленного сервера
        /// </summary>
        /// <returns></returns>
        private static string TryLoadRemoteContent()
        {
            var request = new HttpWebRequest(new Uri("http://192.168.1.90:5000/MyWeb/Shares/English/words.txt"));
            try
            {
                using (var stream = request.GetResponse().GetResponseStream().AssertNull())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Заполучить итоговую строку контента
        /// </summary>
        /// <returns></returns>
        private static string GetContentString()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filename = Path.Combine(documents, "Words.txt");
            // Сначала пытаемся затянуть (и сохранить локально) удаленный контент
            var content = TryLoadRemoteContent();
            if (content != null)
            {
                File.WriteAllText(filename, content);
                return content;
            }
            // если не получилось его подтянуть - тащим локальный
            if (File.Exists(filename))
                return File.ReadAllText(filename);
            return null;
        }

        /// <summary>
        /// Распарсить контент, получив главы
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [NotNull, ItemNotNull]
        private static IEnumerable<TestChapter> ParseChapters([NotNull] string content)
        {
            var chapterCaption = string.Empty;
            var chapterWords = new List<(string eng, string rus)>();
            foreach (var line in content.Split('\n').Select(x => x.Trim()))
            {
                if (line.StartsWith(';'))
                    continue;
                if (string.IsNullOrEmpty(line))
                    continue;

                var sepIndex = line.IndexOf('=');
                if (sepIndex == -1)
                {
                    if (chapterWords.Any() && !string.IsNullOrEmpty(chapterCaption))
                    {
                        yield return new TestChapter(chapterCaption, chapterWords.ToArray());
                        chapterWords.Clear();
                    }
                    chapterCaption = line;
                }
                else
                {
                    var engWord = line.Substring(0, sepIndex).Trim();
                    var rusWord = line.Substring(sepIndex + 1).Trim();
                    if (!string.IsNullOrEmpty(engWord) && !string.IsNullOrEmpty(rusWord))
                        chapterWords.Add((engWord, rusWord));
                }
            }
            if (chapterWords.Any() && !string.IsNullOrEmpty(chapterCaption))
                yield return new TestChapter(chapterCaption, chapterWords.ToArray());
        }

        /// <summary>
        /// Загрузить все главы
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestChapter> LoadAllChapters()
        {
            var content = GetContentString();
            return content == null ? LoadDemoChapters() : ParseChapters(content);
        }
    }
}