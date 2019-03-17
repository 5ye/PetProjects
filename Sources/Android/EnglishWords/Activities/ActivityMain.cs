using System.IO;
using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using SkiaSharp;
using Path = Android.Graphics.Path;

namespace EnglishWords.Activities
{
    [Activity]
    public class ActivityMain : Activity
    {
        /*
        /// <summary>
        /// Запустить тест на исполнение
        /// </summary>
        /// <param name="test"></param>
        private void StartTest([NotNull] Test test)
        {
            RuntimeEnvironment.ActiveTest = test;
            StartActivity(typeof(ActivityQuestions));
        }
        */

        /// <summary>
        /// При создании Activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutMain);

            var imgStart = FindViewById<ImageView>(Resource.Id.imgStart);
            imgStart.Click += (sender, args) => StartActivity(typeof(ActivitySelectLevel));
            imgStart.LinkSvg("activityMainBtnStart", 40, 20);

            var imgSettings = FindViewById<ImageView>(Resource.Id.imgSettings);
            imgSettings.Click += (sender, args) => StartActivity(typeof(ActivitySelectBook));
            imgSettings.LinkSvg("activityMainBtnSettings", 40, 20, topMargin: 2);
        }

        /// <summary>
        /// При закрытии Activity
        /// </summary>
        public override void OnBackPressed()
        {
            FinishAffinity();
        }
    }
}