using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
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
        /// Создать тест по конкретной части
        /// </summary>
        /// <returns></returns>
        public Test CreateTest()
        {
            var kind = TestsManager.Single.CurrentTestKind;
            var questions = new List<TestQuestion>();
            foreach (var (eng, rus) in Pairs.RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? eng : rus;
                var answer = kind == TestKind.WordIsEnglish ? rus : eng;
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
            return new Test(Caption, questions.ToArray());
        }

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
    /// Одна книга исходных данных
    /// Книга = класс (наверное, всегда)
    /// </summary>
    internal class TestBook
    {
        /// <summary>
        /// Текущая глава
        /// </summary>
        [NotNull]
        private TestChapter _currentChapter;

        /// <summary>
        /// Текущая глава
        /// </summary>
        [NotNull]
        public TestChapter CurrentChapter => _currentChapter;

        /// <summary>
        /// Установить текущую главу по наименованию
        /// </summary>
        /// <param name="chapterCaption"></param>
        /// <param name="saveOnDisk"></param>
        public void SetCurrentChapterByCaption([NotNull]string chapterCaption, bool saveOnDisk)
        {
            RuntimeEnvironment.ChapterSelected = true;
            _currentChapter = Chapters.FirstOrDefault(x => x.Caption == chapterCaption) ?? Chapters.First();
            if (saveOnDisk)
                Helpers.WriteDiscContent(TestsManager.CURRENT_CHAPTER_FILE, chapterCaption);
        }

        /// <summary>
        /// Наименование книги
        /// </summary>
        [NotNull]
        public readonly string Caption;

        /// <summary>
        /// Главы
        /// </summary>
        [NotNull, ItemNotNull]
        public readonly TestChapter[] Chapters;

        /// <summary>
        /// Создать тест по всем частям
        /// </summary>
        /// <returns></returns>
        public Test CreateTestAllChapters()
        {
            var kind = TestsManager.Single.CurrentTestKind;
            var questions = new List<TestQuestion>();
            var chaptersToUse = Chapters
                .Reverse()
                .SkipWhile(x => x != _currentChapter);
            foreach (var (eng, rus) in chaptersToUse.SelectMany(x => x.Pairs).RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? eng : rus;
                var answer = kind == TestKind.WordIsEnglish ? rus : eng;
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
        /// <param name="count"></param>
        /// <returns></returns>
        public Test CreateTestLastChapters(int count)
        {
            var kind = TestsManager.Single.CurrentTestKind;
            var questions = new HashSet<TestQuestion>();
            var chaptersToUse = Chapters
                .Reverse()
                .SkipWhile(x => x != _currentChapter)
                .Take(count)
                .ToArray();
            foreach (var (eng, rus) in chaptersToUse.SelectMany(x => x.Pairs).RandomOrder())
            {
                var word = kind == TestKind.WordIsEnglish ? eng : rus;
                var answer = kind == TestKind.WordIsEnglish ? rus : eng;
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
            // вопросов может оказаться меньше
            return new Test($"Последние {chaptersToUse.Length} главы", questions.ToArray());
        }

        /// <summary>
        /// Создать тест по случайным частям
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Test CreateTestRandomWords(int count)
        {
            var kind = TestsManager.Single.CurrentTestKind;
            var questions = new HashSet<TestQuestion>();
            var chaptersToUse = Chapters
                .Reverse()
                .SkipWhile(x => x != _currentChapter);
            foreach (var (eng, rus) in chaptersToUse.SelectMany(x => x.Pairs).RandomOrder().Take(count))
            {
                var word = kind == TestKind.WordIsEnglish ? eng : rus;
                var answer = kind == TestKind.WordIsEnglish ? rus : eng;
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
            // вопросов может оказаться меньше
            return new Test($"Случайные {questions.Count} слов", questions.ToArray());
        }

        /// <summary>
        /// Все пары слов
        /// </summary>
        private readonly Lazy<(string eng, string rus)[]> _allPairs;

        /// <summary>
        /// Получить все пары слов
        /// </summary>
        public (string eng, string rus)[] AllPairs => _allPairs.Value;

        /// <summary>
        /// Создание
        /// </summary>
        public TestBook([NotNull]string caption, [NotNull, ItemNotNull]TestChapter[] chapters)
        {
            Caption = caption;
            Chapters = chapters;
            _currentChapter = chapters.First();
            _allPairs = new Lazy<(string eng, string rus)[]>(() =>
            {
                var result = new List<(string eng, string rus)>();
                foreach (var chapter in chapters)
                    result.AddRange(chapter.Pairs);
                return result.ToArray();
            });
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
        /// Название файла текущей книги
        /// </summary>
        [NotNull]
        private const string CURRENT_BOOK_FILE = "currentBook.txt";

        /// <summary>
        /// Текущая глава
        /// </summary>
        [NotNull]
        public const string CURRENT_CHAPTER_FILE = "currentChapter.txt";

        /// <summary>
        /// Текущая глава
        /// </summary>
        [NotNull]
        public const string CURRENT_TEST_KIND_FILE = "currentTestKind.txt";

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
        public static void Initialize([NotNull]Activity activity)
        {
            if (_single == null)
                _single = new TestsManager(activity);
            var currentBook = Helpers.ReadDiscContent(CURRENT_BOOK_FILE);
            if (currentBook != null)
            {
                _single.SetCurrentBookByCaption(currentBook, false);
                var currentChapter = Helpers.ReadDiscContent(CURRENT_CHAPTER_FILE);
                if (currentChapter != null)
                    _single.CurrentBook.SetCurrentChapterByCaption(currentChapter, false);
            }
            _single.SetCurrentTestKind(int.TryParse(Helpers.ReadDiscContent(CURRENT_TEST_KIND_FILE), out var testKindValue) ?
                (TestKind)testKindValue : default(TestKind), false);
        }

        /// <summary>
        /// Текущая выбранная пользователем книга (по сути, класс)
        /// </summary>
        [NotNull]
        private TestBook _currentBook;

        /// <summary>
        /// Тип выполняемого теста
        /// </summary>
        private TestKind _currentTestKind;

        /// <summary>
        /// Все книги
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly TestBook[] _allBooks;

        /// <summary>
        /// Все главы
        /// </summary>
        [NotNull, ItemNotNull]
        public TestBook[] AllBooks => _allBooks;

        /// <summary>
        /// Получить текущую книгу
        /// </summary>
        [NotNull]
        public TestBook CurrentBook => _currentBook;

        /// <summary>
        /// Тип выполняемого теста
        /// </summary>
        public TestKind CurrentTestKind => _currentTestKind;

        /// <summary>
        /// Установить текущую книгу
        /// </summary>
        /// <param name="bookCaption"></param>
        /// <param name="saveOnDisk"></param>
        public void SetCurrentBookByCaption([NotNull]string bookCaption, bool saveOnDisk)
        {
            RuntimeEnvironment.BookSelected = true;
            _currentBook = _allBooks.FirstOrDefault(x => x.Caption == bookCaption) ?? _allBooks.First();
            if (saveOnDisk)
                Helpers.WriteDiscContent(CURRENT_BOOK_FILE, bookCaption);
        }

        /// <summary>
        /// Установить текущее направление перевода
        /// </summary>
        /// <param name="value"></param>
        /// <param name="saveOnDisk"></param>
        public void SetCurrentTestKind(TestKind value, bool saveOnDisk)
        {
            _currentTestKind = value;
            if (saveOnDisk)
                Helpers.WriteDiscContent(CURRENT_TEST_KIND_FILE, ((int)value).ToString());
        }

        /// <summary>
        /// Конструктор закрыт, т.к. явное создание этого объекта запрещено
        /// </summary>
        private TestsManager([NotNull]Activity activity)
        {
            _allBooks = ChaptersContent.LoadAllBooks(activity).ToArray();
            _currentBook = _allBooks.First();
        }
    }
}