using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;

namespace EnglishWords.Activities
{
    /// <summary>
    /// Activity выбора текущей главы
    /// </summary>
    [Activity(Label = "", NoHistory = true)]
    public class ActivitySelectChapter : Activity
    {
        /// <summary>
        /// Запустить тест на исполнение
        /// </summary>
        /// <param name="sender"></param>
        private void SelectCurrentChapter([NotNull]Button sender)
        {
            TestsManager.Single.CurrentBook.SetCurrentChapterByCaption(sender.Text, true);
            StartActivity(typeof(ActivityMain));
        }

        /// <summary>
        /// Вывод заголовка окна
        /// </summary>
        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Window.SetTitle(TestsManager.Single.CurrentBook.Caption);
        }

        /// <summary>
        /// При создании Activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutSelectChapter);
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent)
            {
                LeftMargin = 20,
                RightMargin = 20
            };
            var buttonsLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutSelectChapter);
            foreach (var chapter in TestsManager.Single.CurrentBook.Chapters)
            {
                var button = new Button(this)
                {
                    Text = chapter.Caption,
                    LayoutParameters = layoutParams
                };
                button.Click += (sender, args) => SelectCurrentChapter(button);
                buttonsLayout.AddView(button);
            }
        }
    }
}