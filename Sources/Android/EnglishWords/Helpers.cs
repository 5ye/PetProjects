using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace EnglishWords
{
    /// <summary>
    /// Утилиты 
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// Средства получения случайного числа
        /// </summary>
        [NotNull]
        public static readonly Random Rnd = new Random();

        /// <summary>
        /// Получить произвольное слово по всем главам в целом
        /// </summary>
        /// <param name="testBook"></param>
        /// <param name="testKind"></param>
        /// <returns></returns>
        public static (string value, string valueTranslate) GetRandomWord([NotNull] this TestBook testBook, TestKind testKind)
        {
            var pair = testBook.AllPairs[Rnd.Next(testBook.AllPairs.Length)];
            return testKind == TestKind.WordIsEnglish ? (pair.rus, pair.eng) : (pair.eng, pair.rus);
        }

        /// <summary>
        /// Получить произвольное слово по всем главам в целом
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="testKind"></param>
        /// <returns></returns>
        public static (string value, string valueTranslate) GetRandomWord([NotNull] this TestChapter chapter, TestKind testKind)
        {
            var pair = chapter.Pairs[Rnd.Next(chapter.Pairs.Length)];
            return testKind == TestKind.WordIsEnglish ? (pair.rus, pair.eng) : (pair.eng, pair.rus);
        }

        /// <summary>
        /// Получить идентификатор ресурса по его имени
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        internal static int GetIdByResourceName([NotNull] string resourceName)
        {
            return (int)typeof(Resource.Id).GetField(resourceName).GetValue(null);
        }

        /// <summary>
        /// Перемешать порядок следования последовательности
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<T> RandomOrder<T>([NotNull]this IEnumerable<T> source)
        {
            return source.Select(x => (Rnd.Next(Int32.MaxValue), x))
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2);
        }

        /// <summary>
        /// Преобразовать строку с контентом в версионированную строку с контентом
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static (int version, string content) StringContentToVersionedContent([NotNull]this string content)
        {
            var index = content.IndexOf('\n');
            if (index > -1 && int.TryParse(content.Substring(0, index).Trim(), out var version))
                return (version, content);
            return (default(int), content);
        }

        /// <summary>
        /// Прочитать контент или получить null, если контента не существует
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadDiscContent([NotNull]string fileName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullFileName = Path.Combine(documents, fileName);
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : null;
        }

        /// <summary>
        /// Записать контент на диск
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static void WriteDiscContent([NotNull]string fileName, [NotNull]string content)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullFileName = Path.Combine(documents, fileName);
            File.WriteAllText(fullFileName, content);
        }
    }
}