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

namespace EnglishWords.Activities
{
    /// <summary>
    /// Выбор уровня
    /// </summary>
    [Activity]
    public class ActivitySelectLevel : Activity
    {
        /// <summary>
        /// Переключить главу
        /// </summary>
        private void SwitchChapter(bool forward)
        {

        }

        /// <summary>
        /// Тип запускаемого теста
        /// </summary>
        private enum StartKind
        {
            /// <summary>
            /// Текущий
            /// </summary>
            Current,

            /// <summary>
            /// Последние N глав
            /// </summary>
            LastChapters,

            /// <summary>
            /// Случайные вопросы
            /// </summary>
            RandomQuestions,

            /// <summary>
            /// Все вопросы
            /// </summary>
            AllQuestions
        }

        /// <summary>
        /// Запустить тест
        /// </summary>
        /// <param name="kind"></param>
        private void StartTest(StartKind kind)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutSelectLevel);

            var layoutTop = FindViewById<LinearLayout>(Resource.Id.llLevelTop);
            layoutTop.SetBoundsByHeight(14 / 2);

            var scale = this.GetViewScale();
            var btnRealWidth = scale.scaleY * 40;
            var leftOrigin = (scale.screenWidth - btnRealWidth) / 2 - 6 * scale.scaleY;

            var imgPrevLevel = FindViewById<ImageView>(Resource.Id.imgPrevLevel);
            imgPrevLevel.Click += (sender, args) => SwitchChapter(false);
            imgPrevLevel.SetSvgBoundsWithDirectMargin("activitySelectLevelPrev", 6, 6, leftOrigin);

            var imgLevel = FindViewById<ImageView>(Resource.Id.imgLevel);
            imgLevel.Click += (sender, args) => StartTest(StartKind.Current);
            imgLevel.SetSvgBoundsWithDirectMargin("activitySelectLevelCurrent", 40, 20, 0);

            var imgNextLevel = FindViewById<ImageView>(Resource.Id.imgNextLevel);
            imgNextLevel.Click += (sender, args) => SwitchChapter(true);
            imgNextLevel.SetSvgBoundsWithDirectMargin("activitySelectLevelNext", 6, 6, 0);

            var imgLastLevels = FindViewById<ImageView>(Resource.Id.imgLastLevels);
            imgLastLevels.Click += (sender, args) => StartTest(StartKind.LastChapters);
            imgLastLevels.SetSvgBoundHorCentered("activitySelectLevelLastLevels", 40, 20, topMargin: 2);

            var imgRandom = FindViewById<ImageView>(Resource.Id.imgRandom);
            imgRandom.Click += (sender, args) => StartTest(StartKind.RandomQuestions);
            imgRandom.SetSvgBoundHorCentered("activitySelectLevelRandom", 40, 20, topMargin: 2);

            var imgAllQuestions = FindViewById<ImageView>(Resource.Id.imgAllQuestions);
            imgAllQuestions.Click += (sender, args) => StartTest(StartKind.AllQuestions);
            imgAllQuestions.SetSvgBoundHorCentered("activitySelectLevelAll", 40, 20, topMargin: 2);
        }
    }
}