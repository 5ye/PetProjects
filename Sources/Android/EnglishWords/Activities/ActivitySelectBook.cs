﻿using System;
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

namespace EnglishWords.Activities
{
    /// <summary>
    /// Activity выбора текущей книги (класса)
    /// </summary>
    [Activity(Label = "Выбор книги", NoHistory = true)]
    public class ActivitySelectBook : Activity
    {
        /// <summary>
        /// Запустить тест на исполнение
        /// </summary>
        /// <param name="sender"></param>
        private void SelectCurrentBook([NotNull]Button sender)
        {
            TestsManager.Single.SetCurrentBookByCaption(sender.Text, true);
            StartActivity(typeof(ActivitySplash));
        }

        /// <summary>
        /// При создании Activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutSelectBook);
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 20,
                RightMargin = 20
            };
            var buttonsLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutSelectBook);
            foreach (var book in TestsManager.Single.AllBooks)
            {
                var button = new Button(this);
                button.Text = book.Caption;
                button.LayoutParameters = layoutParams;
                button.Click += (sender, args) => SelectCurrentBook(button);
                buttonsLayout.AddView(button);
            }
        }
    }
}