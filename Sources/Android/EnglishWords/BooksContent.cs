using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Android.App;
using JetBrains.Annotations;
using Environment = System.Environment;

namespace EnglishWords
{
    /// <summary>
    /// Контент глав
    /// </summary>
    internal static class ChaptersContent
    {
        /*
        Переделать в удаленное обновление, возможно
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
        */

        /// <summary>
        /// Получить контент, вшитый непосредственно в программу
        /// </summary>
        /// <returns></returns>
        private static (int version, string content) GetHardcodeContentString([NotNull]Activity activity)
        {
            using (var reader = new StreamReader(activity.Assets.Open("BooksContent.txt")))
                return reader.ReadToEnd().StringContentToVersionedContent();
        }

        /// <summary>
        /// Заполучить итоговую строку контента
        /// </summary>
        /// <returns></returns>
        [NotNull]
        private static string GetContentString([NotNull]Activity activity)
        {
            var hardcodeContent = GetHardcodeContentString(activity);
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var savedContentFile = Path.Combine(documents, "Words.txt");
            if (File.Exists(savedContentFile))
            {
                var savedContent = File.ReadAllText(savedContentFile).StringContentToVersionedContent();
                if (savedContent.version >= hardcodeContent.version)
                    return savedContent.content;
                File.WriteAllText(savedContentFile, hardcodeContent.content);
            }
            return hardcodeContent.content;
        }

        /// <summary>
        /// Распарсить контент, получив главы
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [NotNull, ItemNotNull]
        private static IEnumerable<TestBook> ParseBooks([NotNull]string content)
        {
            const string bookSeparator = "---->";
            var bookCaption = string.Empty;
            var chapterCaption = string.Empty;
            var chapterWords = new List<(string eng, string rus)>();
            var bookChapters = new List<TestChapter>();
            foreach (var line in content.Split('\n').Select(x => x.Trim()))
            {
                if (line.StartsWith(';'))
                    continue;
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith(bookSeparator))
                {
                    yield return new TestBook(bookCaption, bookChapters.ToArray());
                    bookCaption = line.Substring(bookSeparator.Length).Trim();
                    bookChapters.Clear();
                }

                var sepIndex = line.IndexOf('=');
                if (sepIndex == -1)
                {
                    if (chapterWords.Any() && !string.IsNullOrEmpty(chapterCaption))
                    {
                        bookChapters.Add(new TestChapter(chapterCaption, chapterWords.ToArray()));
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
            {
                bookChapters.Add(new TestChapter(chapterCaption, chapterWords.ToArray()));
                yield return new TestBook(bookCaption, bookChapters.ToArray());
            }
        }

        /// <summary>
        /// Загрузить все главы
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestBook> LoadAllBooks([NotNull]Activity activity)
        {
            var content = GetContentString(activity);
            return ParseBooks(content);
        }
    }
}