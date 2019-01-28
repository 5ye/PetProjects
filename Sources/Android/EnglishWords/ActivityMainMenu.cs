using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;

namespace EnglishWords
{
    [Activity(Label = "Учи английский, Саша!")]
    public class ActivityMainMenu : Activity
    {
        /// <summary>
        /// Запустить тест на исполнение
        /// </summary>
        /// <param name="test"></param>
        private void StartTest([NotNull] Test test)
        {
            ActivityExchanger.ActiveTest = test;
            StartActivity(typeof(ActivityQuestions));
        }

        /// <summary>
        /// Выбранный пользователем вариант теста
        /// </summary>
        private TestKind Kind => FindViewById<RadioButton>(Resource.Id.radioButton1).Checked
            ? TestKind.WordIsEnglish
            : TestKind.WordIsRussian;
        
        /// <summary>
        /// При создании Activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutMainMenu);
            LinearLayout.LayoutParams layoutParams =
                new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    LeftMargin = 20,
                    RightMargin = 20
                };
            var btn20Words = FindViewById<Button>(Resource.Id.button20Words);
            btn20Words.Click += (sender, args) => 
                StartTest(TestsManager.Single.CreateTestRandomWords(Kind, 20));
            btn20Words.LayoutParameters = layoutParams;
            var btnAllWords = FindViewById<Button>(Resource.Id.buttonAllWords);
            btnAllWords.Click += (sender, args) =>
                StartTest(TestsManager.Single.CreateTestAllChapters(Kind));
            btnAllWords.LayoutParameters = layoutParams;
            var buttonsLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutMenuButtons);
            foreach (var chapter in TestsManager.Single.AllChapters)
            {
                Button button = new Button(this);
                button.Text = chapter.Caption;
                button.LayoutParameters = layoutParams;
                button.Click += (sender, args) => StartTest(TestsManager.Single.CreateTestByChapter(Kind, chapter));
                buttonsLayout.AddView(button);
            }
        }

        /// <summary>
        /// При закрытии Activity
        /// </summary>
        public override void OnBackPressed()
        {
            FinishAffinity();
            // JavaSystem.Exit(0);
        }
    }
}