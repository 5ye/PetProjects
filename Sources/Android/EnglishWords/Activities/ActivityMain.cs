using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;

namespace EnglishWords.Activities
{
    [Activity(Label = "Учим английский")]
    public class ActivityMain : Activity
    {
        /// <summary>
        /// Запустить тест на исполнение
        /// </summary>
        /// <param name="test"></param>
        private void StartTest([NotNull] Test test)
        {
            RuntimeEnvironment.ActiveTest = test;
            StartActivity(typeof(ActivityQuestions));
        }

        /// <summary>
        /// Изменить направление перевода
        /// </summary>
        private void InvertTestKind()
        {
            var newDirection = TestsManager.Single.CurrentTestKind == TestKind.WordIsEnglish ? 
                TestKind.WordIsRussian : 
                TestKind.WordIsEnglish;
            TestsManager.Single.SetCurrentTestKind(newDirection, true);
            var imgSetDirection = FindViewById<ImageView>(Resource.Id.imageViewDirect);
            imgSetDirection.SetImageResource(TestsManager.Single.CurrentTestKind == TestKind.WordIsEnglish ?
                Resource.Drawable.engToRus :
                Resource.Drawable.rusToEng);
        }

        /// <summary>
        /// При создании Activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutMain);
            if (!RuntimeEnvironment.BookSelected)
                StartActivity(typeof(ActivitySelectBook));
            else if (!RuntimeEnvironment.ChapterSelected)
                StartActivity(typeof(ActivitySelectChapter));
            var imgSetDirection = FindViewById<ImageView>(Resource.Id.imageViewDirect);
            imgSetDirection.SetImageResource(TestsManager.Single.CurrentTestKind == TestKind.WordIsEnglish ?
                Resource.Drawable.engToRus :
                Resource.Drawable.rusToEng);
            imgSetDirection.Click += (sender, args) => InvertTestKind();
            var btnSelectBook = FindViewById<Button>(Resource.Id.buttonSelectBook);
            btnSelectBook.Click += (sender, args) => StartActivity(typeof(ActivitySelectBook));
            if (RuntimeEnvironment.BookSelected)
                btnSelectBook.Text = TestsManager.Single.CurrentBook.Caption;
            var btnSelectChapter = FindViewById<Button>(Resource.Id.buttonSelectChapter);
            btnSelectChapter.Click += (sender, args) => StartActivity(typeof(ActivitySelectChapter));
            if (RuntimeEnvironment.ChapterSelected)
                btnSelectChapter.Text = TestsManager.Single.CurrentBook.CurrentChapter.Caption;
            var btnCurrentChapter = FindViewById<Button>(Resource.Id.buttonCurrentChapter);
            btnCurrentChapter.Click += (sender, args) =>
                StartTest(TestsManager.Single.CurrentBook.CurrentChapter.CreateTest());
            var btnLastChapters = FindViewById<Button>(Resource.Id.buttonLastChapters);
            btnLastChapters.Click += (sender, args) =>
                StartTest(TestsManager.Single.CurrentBook.CreateTestLastChapters(3));
            var btn20Words = FindViewById<Button>(Resource.Id.button20Words);
            btn20Words.Click += (sender, args) =>
                StartTest(TestsManager.Single.CurrentBook.CreateTestRandomWords(20));
            var btnAllWords = FindViewById<Button>(Resource.Id.buttonAllWords);
            btnAllWords.Click += (sender, args) =>
                StartTest(TestsManager.Single.CurrentBook.CreateTestAllChapters());
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