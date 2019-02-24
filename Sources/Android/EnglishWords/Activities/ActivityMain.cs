using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using Environment = System.Environment;

namespace EnglishWords.Activities
{
    [Activity(Label = "ActivityMain")]
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
            LinearLayout.LayoutParams layoutParams =
                new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    LeftMargin = 20,
                    RightMargin = 20
                };
            var btnCurrentChapter = FindViewById<Button>(Resource.Id.buttonCurrentChapter);
            btnCurrentChapter.Click += (sender, args) =>
                StartTest(TestsManager.Single.CurrentBook.CurrentChapter.CreateTest());
            btnCurrentChapter.LayoutParameters = layoutParams;
            var btnLastChapters = FindViewById<Button>(Resource.Id.buttonLastChapters);
            btnLastChapters.Click += (sender, args) =>
                StartTest(TestsManager.Single.CurrentBook.CreateTestLastChapters(3));
            btnLastChapters.LayoutParameters = layoutParams;
            var btn20Words = FindViewById<Button>(Resource.Id.button20Words);
            btn20Words.Click += (sender, args) =>
                StartTest(TestsManager.Single.CurrentBook.CreateTestRandomWords(20));
            btn20Words.LayoutParameters = layoutParams;
            var btnAllWords = FindViewById<Button>(Resource.Id.buttonAllWords);
            btnAllWords.Click += (sender, args) =>
                StartTest(TestsManager.Single.CurrentBook.CreateTestAllChapters());
            btnAllWords.LayoutParameters = layoutParams;
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