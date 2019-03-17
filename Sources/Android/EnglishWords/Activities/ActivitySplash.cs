using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using EnglishWords.Activities;

namespace EnglishWords.Activities
{
    [Activity(Label = "Учи английский, Саша!", MainLauncher = true, NoHistory = true)]
    public class ActivitySplash : Activity
    {
        /// <summary>
        /// При создании Activity
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutSplash);
        }

        /// <summary>
        /// При запуске Activity
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            new Task(() =>
            {
                System.Threading.Thread.Sleep(1000);
                TestsManager.Initialize(this);
                StartActivity(typeof(ActivityMain));
                Finish();
            }).Start();
        }
    }
}

