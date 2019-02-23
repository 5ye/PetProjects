using System;
using System.Collections.Generic;
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
        /// <param name="testManager"></param>
        /// <param name="testKind"></param>
        /// <returns></returns>
        public static (string value, string valueTranslate) GetRandomWord([NotNull] this TestsManager testManager, TestKind testKind)
        {
            var pair = testManager.AllPairs[Rnd.Next(testManager.AllPairs.Length)];
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
    }

    /// <summary>
    /// Средства обмена между активностями
    /// </summary>
    internal static class ActivityExchanger
    {
        /// <summary>
        /// Выполняемый тест
        /// </summary>
        public static Test ActiveTest { get; set; }
    }
}