using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;

namespace EnglishWords.Activities
{
    /// <summary>
    /// Выбор уровня
    /// </summary>
    [Activity]
    public class ActivitySelectLevel : Activity
    {
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
        /// Кнопка Влево
        /// </summary>
        private ImageView _imgPrevLevel;

        /// <summary>
        /// Кнопка уровня
        /// </summary>
        private ImageView _imgLevel;

        /// <summary>
        /// Кнопка Вправо
        /// </summary>
        private ImageView _imgNextLevel;

        /// <summary>
        /// Кнопка Последние 3 главы
        /// </summary>
        private ImageView _imgLastLevels;

        /// <summary>
        /// Кнопка Случайные вопросы
        /// </summary>
        private ImageView _imgRandom;

        /// <summary>
        /// Кнопка "Все вопросы"
        /// </summary>
        private ImageView _imgAllQuestions;

        /// <summary>
        /// Переключить главу
        /// </summary>
        private void SwitchChapter(bool forward)
        {
            locked = !locked;
            _imgLevel.UpdateSvg();
        }

        /// <summary>
        /// Запустить тест
        /// </summary>
        /// <param name="kind"></param>
        private void StartTest(StartKind kind)
        {

        }

        private static bool locked;

        /// <summary>
        /// Предобработать кнопочки
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="svg"></param>
        private static void PreprocessSwitchSvg(bool forward, [NotNull]XmlDocument svg)
        {
            svg.Get("path8").Css("fill", "#aaa");
        }

        /// <summary>
        /// Предобработать кнопочки
        /// </summary>
        /// <param name="svg"></param>
        private static void PreprocessCurrentChapterSvg([NotNull]XmlDocument svg)
        {
            var nodeToRemove = locked ? svg.Get("g10_play") : svg.Get("gLock");
            nodeToRemove.ParentNode.AssertNull().RemoveChild(nodeToRemove);
            if (locked)
                svg.Get("rect2").Css("fill", "#aaa");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutSelectLevel);

            _imgPrevLevel = FindViewById<ImageView>(Resource.Id.imgPrevLevel);
            _imgPrevLevel.Click += (sender, args) => SwitchChapter(false);
            _imgPrevLevel.LinkSvg("activitySelectLevelPrev", 6, 6, 
                preprocess: x => PreprocessSwitchSvg(false, x));

            _imgLevel = FindViewById<ImageView>(Resource.Id.imgLevel);
            _imgLevel.Click += (sender, args) => StartTest(StartKind.Current);
            _imgLevel.LinkSvg("activitySelectLevelCurrent", 40, 20, 1,
                preprocess: x => PreprocessCurrentChapterSvg(x));

            _imgNextLevel = FindViewById<ImageView>(Resource.Id.imgNextLevel);
            _imgNextLevel.Click += (sender, args) => SwitchChapter(true);
            _imgNextLevel.LinkSvg("activitySelectLevelNext", 6, 6, 1,
                preprocess: x => PreprocessSwitchSvg(true, x));

            _imgLastLevels = FindViewById<ImageView>(Resource.Id.imgLastLevels);
            _imgLastLevels.Click += (sender, args) => StartTest(StartKind.LastChapters);
            _imgLastLevels.LinkSvg("activitySelectLevelLastLevels", 40, 20, topMargin: 2);

            _imgRandom = FindViewById<ImageView>(Resource.Id.imgRandom);
            _imgRandom.Click += (sender, args) => StartTest(StartKind.RandomQuestions);
            _imgRandom.LinkSvg("activitySelectLevelRandom", 40, 20, topMargin: 2);

            _imgAllQuestions = FindViewById<ImageView>(Resource.Id.imgAllQuestions);
            _imgAllQuestions.Click += (sender, args) => StartTest(StartKind.AllQuestions);
            _imgAllQuestions.LinkSvg("activitySelectLevelAll", 40, 20, topMargin: 2);
        }
    }
}