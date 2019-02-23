using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace EnglishWords
{
    /// <summary>
    /// Обработчик формы показа результата
    /// </summary>
    [Activity(Label = "", NoHistory = true)]
    public class ActivityResult : Activity
    {
        /// <summary>
        /// Выполняемый тест
        /// </summary>
        private Test _runningTest;

        /// <summary>
        /// Переход к новому вопросу
        /// </summary>
        private void ProcessGotoMain()
        {
            Finish();
        }

        /// <summary>
        /// Обработчик кнопки "Назад"
        /// </summary>
        public override void OnBackPressed()
        {
            ProcessGotoMain();
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
        /// Установить контент в соответствии с результатом
        /// </summary>
        private void SetTestContent()
        {
            var resultTextView = FindViewById<TextView>(Resource.Id.textViewTestResult);
            var goodCount = _runningTest.GoodCount;
            var totalCount = _runningTest.QuestionsCount;
            resultTextView.Text = $"{goodCount}/{totalCount}";
            var percents = ((double)goodCount) / totalCount * 100;
            if (percents < 50)
                resultTextView.SetTextColor(Android.Graphics.Color.Red);
            else if (percents < 80)
                resultTextView.SetTextColor(Android.Graphics.Color.Yellow);
            else
                resultTextView.SetTextColor(Android.Graphics.Color.Green);
        }

        /// <summary>
        /// Создание Activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutResult);
            _runningTest = ActivityExchanger.ActiveTest;
            FindViewById<Button>(Resource.Id.buttonGotoMain).Click +=
                (sender, e) => ProcessGotoMain();
            LinearLayout.LayoutParams layoutParams =
                new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    LeftMargin = 20,
                    RightMargin = 20,
                    TopMargin = 100,
                    BottomMargin = 100
                };
            FindViewById<TextView>(Resource.Id.textViewTestResult).LayoutParameters = layoutParams;
            var buttonLayout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 100,
                RightMargin = 100,
                TopMargin = 100,
                BottomMargin = 100
            };
            FindViewById<Button>(Resource.Id.buttonGotoMain).LayoutParameters = buttonLayout;
            SetTestContent();
        }
    }
}