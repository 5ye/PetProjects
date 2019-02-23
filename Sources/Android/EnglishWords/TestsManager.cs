using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace EnglishWords
{
    /// <summary>
    /// Один вопрос теста
    /// </summary>
    internal class TestQuestion
    {
        /// <summary>
        /// Слово, которое подлежит переводу
        /// </summary>
        [NotNull]
        public readonly string Word;

        /// <summary>
        /// Ответы на слово
        /// </summary>
        [NotNull]
        public readonly (string value, string valueTranslate)[] Answers;

        /// <summary>
        /// Индекс правильного ответа
        /// </summary>
        private readonly int _rightIndex;

        /// <summary>
        /// Ответ
        /// </summary>
        private int? _answerIndex; 

        /// <summary>
        /// Зарегистировать ответ
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void RegisterAnswer(int value)
        {
            _answerIndex = value;
        }

        /// <summary>
        /// Ответ правильный
        /// </summary>
        public bool AnswerIsOk => _answerIndex == _rightIndex;

        /// <summary>
        /// Правильный ответ
        /// </summary>
        public string RightAnswer => Answers[_rightIndex].value;

        /// <summary>
        /// Выбранный ответ
        /// </summary>
        public (string value, string valueTranslate) SelectedAnswer
        {
            get
            {
                if (_answerIndex == null)
                    throw new Exception("Обращение к индексу ответа до его инициализации!");
                return Answers[_answerIndex.Value];
            }

        }

        /// <summary>
        /// Создание вопроса
        /// </summary>
        /// <param name="word"></param>
        /// <param name="answer"></param>
        /// <param name="trashAnswers"></param>
        public TestQuestion([NotNull] string word, [NotNull]string answer, [NotNull](string value, string valueTranslate)[] trashAnswers)
        {
            Word = word;
            Answers = new (string, string)[trashAnswers.Length + 1];
            _rightIndex = Helpers.Rnd.Next(Answers.Length);
            for (int i = 0; i < Answers.Length; i++)
            {
                if (i < _rightIndex)
                    Answers[i] = trashAnswers[i];
                if (i == _rightIndex)
                    Answers[i] = (answer, word);
                if (i > _rightIndex)
                    Answers[i] = trashAnswers[i - 1];
            }
        }
    }
    
    /// <summary>
    /// Одно тестирование
    /// </summary>
    internal class Test
    {
        /// <summary>
        /// Заголовок теста
        /// </summary>
        public readonly string Caption;
        
        /// <summary>
        /// Индекс текущего вопроса
        /// </summary>
        private int _questionIndex;
        
        /// <summary>
        /// Вопросы
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly TestQuestion[] _questions;

        /// <summary>
        /// Получить текущий вопрос
        /// </summary>
        [NotNull]
        public TestQuestion CurrentQuestion => _questions[_questionIndex];

        /// <summary>
        /// Число вопросов
        /// </summary>
        public int QuestionsCount => _questions.Length;

        /// <summary>
        /// Число правильных ответов
        /// </summary>
        public int GoodCount => _questions.Count(x => x.AnswerIsOk);

        /// <summary>
        /// Перейти к следующему вопросу
        /// </summary>
        /// <returns></returns>
        public void Next()
        {
            _questionIndex++;
        }

        /// <summary>
        /// Тест пройден
        /// </summary>
        public bool Done => _questionIndex > _questions.Length - 1;

        /// <summary>
        /// Индекс текущего вопроса
        /// </summary>
        public int QuestionIndex => _questionIndex;

        /// <summary>
        /// Создание
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="questions"></param>
        public Test([NotNull]string caption, [NotNull, ItemNotNull] TestQuestion[] questions)
        {
            Caption = caption;
            _questions = questions;
        }
    }

    /// <summary>
    /// Одна глава исходных данных
    /// </summary>
    internal class TestChapter
    {
        /// <summary>
        /// Наименование главы
        /// </summary>
        [NotNull]
        public readonly string Caption;

        /// <summary>
        /// Набор пар слово - перевод
        /// </summary>
        [NotNull]
        public readonly (string eng, string rus)[] Pairs;

        /// <summary>
        /// Создание 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="pairs"></param>
        public TestChapter([NotNull] string caption, params (string eng, string rus)[] pairs)
        {
            Caption = caption;
            Pairs = pairs.ToArray();
        }
    }

    /// <summary>
    /// Тип теста
    /// </summary>
    internal enum TestKind
    {
        /// <summary>
        /// Слово на аинглийском
        /// </summary>
        WordIsEnglish,

        /// <summary>
        /// Слово на русском
        /// </summary>
        WordIsRussian
    }
    
    /// <summary>
    /// Средства тестирования
    /// </summary>
    internal class TestsManager
    {
        /// <summary>
        /// Создать тест по всем частям
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public Test CreateTestAllChapters(TestKind kind)
        {
            var questions = new List<TestQuestion>();
            foreach (var pair in _allChapters.SelectMany(x => x.Pairs).RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? pair.eng : pair.rus;
                var answer = kind == TestKind.WordIsEnglish ? pair.rus : pair.eng;
                var randomAnswers = new HashSet<(string, string)>();
                while (randomAnswers.Count < 3)
                {
                    var newWord = this.GetRandomWord(kind);
                    if (newWord.value == answer)
                        continue;
                    randomAnswers.Add(newWord);
                }
                questions.Add(new TestQuestion(word, answer, randomAnswers.ToArray()));
            }
            return new Test("Все слова", questions.ToArray());
        }

        /// <summary>
        /// Создать тест по случайным частям
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Test CreateTestRandomWords(TestKind kind, int count)
        {
            var questions = new HashSet<TestQuestion>();
            foreach (var pair in _allChapters.SelectMany(x => x.Pairs).RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? pair.eng : pair.rus;
                var answer = kind == TestKind.WordIsEnglish ? pair.rus : pair.eng;
                var randomAnswers = new HashSet<(string, string)>();
                while (randomAnswers.Count < 3)
                {
                    var newWord = this.GetRandomWord(kind);
                    if (newWord.value == answer)
                        continue;
                    randomAnswers.Add(newWord);
                }
                questions.Add(new TestQuestion(word, answer, randomAnswers.ToArray()));
                if (questions.Count == count)
                    break;
            }
            // вопросов может оказаться меньше
            return new Test($"Случайные {count} слов", questions.ToArray());
        }

        /// <summary>
        /// Создать тест по конкретной части
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="chapter"></param>
        /// <returns></returns>
        public Test CreateTestByChapter(TestKind kind, [NotNull] TestChapter chapter)
        {
            var questions = new List<TestQuestion>();
            foreach (var pair in chapter.Pairs.RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? pair.eng : pair.rus;
                var answer = kind == TestKind.WordIsEnglish ? pair.rus : pair.eng;
                var randomAnswers = new HashSet<(string, string)>();
                while (randomAnswers.Count < 3)
                {
                    var newWord = chapter.GetRandomWord(kind);
                    if (newWord.value == answer)
                        continue;
                    randomAnswers.Add(newWord);
                }
                questions.Add(new TestQuestion(word, answer, randomAnswers.ToArray()));
            }
            return new Test(chapter.Caption, questions.ToArray());
        }

        /// <summary>
        /// Единственный экзеимпляр
        /// </summary>
        private static TestsManager _single;

        /// <summary>
        /// Единственный экзеимпляр
        /// </summary>
        [NotNull]
        public static TestsManager Single => _single;

        /// <summary>
        /// Выполнить начальную инициализацию
        /// </summary>
        public static void Initialize()
        {
            if (_single == null)
                _single = new TestsManager();
        }

        /// <summary>
        /// Все части
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly TestChapter[] _allChapters;

        /// <summary>
        /// Все пары слов
        /// </summary>
        private readonly Lazy<(string eng, string rus)[]> _allPairs;

        /// <summary>
        /// Получить все пары слов
        /// </summary>
        public (string eng, string rus)[] AllPairs => _allPairs.Value;

        /// <summary>
        /// Все главы
        /// </summary>
        [NotNull, ItemNotNull]
        public TestChapter[] AllChapters => _allChapters;

        /// <summary>
        /// Конструктор закрыт, т.к. явное создание этого объекта запрещено
        /// </summary>
        private TestsManager()
        {
            _allChapters = ChaptersContent.LoadAllChapters().ToArray();
            _allPairs = new Lazy<(string eng, string rus)[]>(() =>
            {
                var result = new List<(string eng, string rus)>();
                foreach (var chapter in _allChapters)
                    result.AddRange(chapter.Pairs);
                return result.ToArray();
            });
        }
    }
}