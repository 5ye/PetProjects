using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using JetBrains.Annotations;

namespace TvSwitcher
{
    [Activity(Label = "Выключатель интернета", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        /// <summary>
        /// Маки
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly string[] _macs =
        {
            "90:F1:AA:1B:BA:98",
            "84:A4:66:4C:AB:62",
            "B8:BB:AF:2F:8E:04"
        };

        /// <summary>
        /// Список картинок
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly ImageView[] _images = new ImageView[3];

        /// <summary>
        /// Переключатели
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly Switch[] _switches = new Switch[3];

        /// <summary>
        /// Прочитанные состояния
        /// </summary>
        [NotNull]
        private readonly bool[] _states = new bool[3];

        /// <summary>
        /// Идет чтение
        /// </summary>
        private bool _pending;

        /// <summary>
        /// Обнаружена ошибка связи
        /// </summary>
        private bool _errorDetected;

        /// <summary>
        /// Обновить элементы управления
        /// </summary>
        private void UpdateControls()
        {
            FindViewById<LinearLayout>(Resource.Id.linearLayoutError).AssertNull().Visibility =
                _errorDetected ? ViewStates.Visible : ViewStates.Invisible;
            foreach (var aSwitch in _switches)
            {
                aSwitch.Enabled = !_pending;
            }
            FindViewById<Button>(Resource.Id.buttonRefresh).AssertNull().Enabled = !_pending;
            if (!_pending)
            {
                for (int i = 0; i < 3; i++)
                {
                    _switches[i].AssertNull().Checked = _states[i];
                    _images[i].AssertNull().SetImageResource(_states[i] ? Resource.Drawable.tvsmall : Resource.Drawable.tvsmall2);
                }
            }
            FindViewById<LinearLayout>(Resource.Id.linearLayout1).AssertNull().RefreshDrawableState();
        }

        /// <summary>
        /// Переключить состояние телека
        /// </summary>
        /// <param name="index"></param>
        private void ToggleMac(int index)
        {
            _errorDetected = false;
            try
            {
                _pending = true;
                UpdateControls();
                try
                {
                    var newStates = new bool[3];
                    _states.CopyTo(newStates, 0);
                    newStates[index] = !newStates[index];
                    var macsToWrite = new string[3];
                    for (int i = 0; i < 3; i++)
                    {
                        macsToWrite[i] = newStates[i] ? null : _macs[i];
                    }
                    RouterHelpers.WriteLockedMacs(macsToWrite.Where(x => x != null));
                    ReReadStateInternal();
                }
                finally
                {
                    _pending = false;
                    UpdateControls();
                }
            }
            catch (Exception)
            {
                _errorDetected = true;
                _pending = false;
                UpdateControls();
            }
        }

        /// <summary>
        /// Перечитывание состояний без обновления экрана
        /// </summary>
        private void ReReadStateInternal()
        {
            var macs = new HashSet<string>(RouterHelpers.ReadLockedMacs());
            for (int i = 0; i < 3; i++)
                _states[i] = !macs.Contains(_macs[i]);
        }

        /// <summary>
        /// Обновить состояние
        /// </summary>
        private void ReReadState()
        {
            _errorDetected = false;
            try
            {
                _pending = true;
                UpdateControls();
                try
                {
                    ReReadStateInternal();
                }
                finally
                {
                    _pending = false;
                    UpdateControls();
                }
            }
            catch (Exception)
            {
                _errorDetected = true;
                _pending = false;
                UpdateControls();
            }
        }

        /// <summary>
        /// Создание
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView (Resource.Layout.Main);
            FindViewById<Button>(Resource.Id.buttonRefresh).AssertNull().Click += (x, y) => ReReadState();
            _images[0] = FindViewById<ImageView>(Resource.Id.imageView1);
            _images[1] = FindViewById<ImageView>(Resource.Id.imageView2);
            _images[2] = FindViewById<ImageView>(Resource.Id.imageView3);
            _switches[0] = FindViewById<Switch>(Resource.Id.switch1);
            _switches[1] = FindViewById<Switch>(Resource.Id.switch2);
            _switches[2] = FindViewById<Switch>(Resource.Id.switch3);
            for (int i = 0; i < 3; i++)
            {
                var local = i;
                _images[i].AssertNull().Click += (x, y) => ToggleMac(local);
                _switches[i].AssertNull().Click += (x, y) => ToggleMac(local);
            }
        }

        /// <summary>
        /// Вызывается при обновлении состояния
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            ReReadState();
        }
    }
}

