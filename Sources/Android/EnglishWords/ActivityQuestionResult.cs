using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace EnglishWords
{
    [Activity(Label = "", NoHistory = true)]
    public class ActivityQuestionResult : Activity
    {
        /// <summary>
        /// Выполняемый тест
        /// </summary>
        private Test _runningTest;

        /// <summary>
        /// Переход к новому вопросу
        /// </summary>
        private void ProcessGotoNextQuestion()
        {
            _runningTest.Next();
            if (_runningTest.Done)
                StartActivity(typeof(ActivityResult));
            Finish();
        }

        /// <summary>
        /// Обработчик кнопки "Назад"
        /// </summary>
        public override void OnBackPressed()
        {
            ProcessGotoNextQuestion();
        }

        /// <summary>
        /// Установить контент в соответствии с результатом
        /// </summary>
        private void SetAnswerContent()
        {
            var resultTextView = FindViewById<TextView>(Resource.Id.textViewQuestResult);
            if (_runningTest.CurrentQuestion.AnswerIsOk)
            {
                resultTextView.Text = "ПРАВИЛЬНО!";
                resultTextView.SetTextColor(Android.Graphics.Color.Green);
            }
            else
            {
                resultTextView.Text = "НЕВЕРНО!";
                resultTextView.SetTextColor(Android.Graphics.Color.Red);
            }
        }

        /// <summary>
        /// Вывод заголовка окна
        /// </summary>
        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Window.SetTitle($"Ответ на вопрос {_runningTest.QuestionIndex + 1}");
        }

        /// <summary>
        /// При создании
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutQuestionResult);
            _runningTest = ActivityExchanger.ActiveTest;
            FindViewById<Button>(Resource.Id.buttonGotoNextQuestion).Click +=
                (sender, e) => ProcessGotoNextQuestion();
            LinearLayout.LayoutParams layoutParams =
                new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    LeftMargin = 20,
                    RightMargin = 20,
                    TopMargin = 100,
                    BottomMargin = 100
                };
            FindViewById<TextView>(Resource.Id.textViewQuestResult).LayoutParameters = layoutParams;
            SetAnswerContent();
        }
    }
}