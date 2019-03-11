using Android.App;
using Android.OS;
using Android.Widget;

namespace EnglishWords.Activities
{
    /// <summary>
    /// Форма прохождения теста
    /// </summary>
    [Activity(Label = "")]
    public class ActivityQuestions : Activity
    {
        /// <summary>
        /// Выполняемый тест
        /// </summary>
        private Test _runningTest;

        /// <summary>
        /// Показать вопрос
        /// </summary>
        private void ShowQuestion()
        {
            if (_runningTest.Done)
                Finish();
            else
            {
                FindViewById<TextView>(Resource.Id.textViewWord).Text = _runningTest.CurrentQuestion.Word.ToUpperInvariant();
                var id = 1;
                foreach (var (value, _) in _runningTest.CurrentQuestion.Answers)
                    FindViewById<Button>(Helpers.GetIdByResourceName($"buttonAnswer{id++}")).Text = value;
                FindViewById<TextView>(Resource.Id.textViewBottomProgress).Text = $"Вопрос {_runningTest.QuestionIndex + 1} из {_runningTest.QuestionsCount}";
            }
        }

        /// <summary>
        /// Зарегистрировать ответ
        /// </summary>
        /// <param name="index"></param>
        private void RegisterAnswer(int index)
        {
            _runningTest.CurrentQuestion.RegisterAnswer(index);
            StartActivity(typeof(ActivityQuestionResult));
        }

        /// <summary>
        /// Требуется показ вопроса
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            ShowQuestion();
        }

        /// <summary>
        /// Вывод заголовка окна
        /// </summary>
        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Window.SetTitle(_runningTest.Caption);
        }

        /// <summary>
        /// Создание Activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutQuestion);
            _runningTest = RuntimeEnvironment.ActiveTest;
            for (int i = 1; i <= 4; i++)
            {
                var index = i;
                FindViewById<Button>(Helpers.GetIdByResourceName($"buttonAnswer{i}")).Click +=
                    (sender, e) => RegisterAnswer(index - 1);
            }
        }
    }
}