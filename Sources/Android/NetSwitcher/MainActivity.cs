using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using JetBrains.Annotations;

namespace NetSwitcher
{
    [Activity(Label = "Выключатель интернета", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        /// <summary>
        /// Описание узла сети
        /// </summary>
        private class NetNode
        {
            /// <summary>
            /// Mac адрес или IP
            /// </summary>
            [NotNull]
            public readonly string MacOrIp;

            /// <summary>
            /// Картинка на выключенное состояние
            /// </summary>
            public readonly int OffPicture;

            /// <summary>
            /// Картинка на включенное состояние
            /// </summary>
            public readonly int OnPicture;

            /// <summary>
            /// Назначенная картинка
            /// </summary>
            public ImageView Image { get; set; }

            /// <summary>
            /// Назначенный переключатель
            /// </summary>
            public Switch Switch { get; set; }

            /// <summary>
            /// Состояние
            /// </summary>
            public bool State { get; set; }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="macOrIp"></param>
            /// <param name="offPicture"></param>
            /// <param name="onPicture"></param>
            public NetNode([NotNull] string macOrIp, int offPicture, int? onPicture = null)
            {
                MacOrIp = macOrIp;
                OffPicture = offPicture;
                OnPicture = onPicture ?? offPicture;
            }

            /// <summary>
            /// Утилита создания телека
            /// </summary>
            /// <param name="macOrIp"></param>
            /// <returns></returns>
            public static NetNode TvNode([NotNull] string macOrIp)
            {
                return new NetNode(macOrIp, Resource.Drawable.tvOff, Resource.Drawable.tvOn);
            }
        }
        
        /// <summary>
        /// Маки
        /// </summary>
        [NotNull, ItemNotNull]
        private static readonly NetNode[] _nodes =
        {
            NetNode.TvNode("90:F1:AA:1B:BA:98"),
            NetNode.TvNode("84:A4:66:4C:AB:62"),
            NetNode.TvNode("B8:BB:AF:2F:8E:04"),
            new NetNode("90", Resource.Drawable.nas)
        };

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
            foreach (var netNode in _nodes)
            {
                netNode.Switch.Enabled = !_pending;
                if (!_pending)
                {
                    netNode.Switch.Checked = netNode.State;
                    netNode.Image.SetImageResource(netNode.State ? netNode.OnPicture : netNode.OffPicture);
                }
            }
            FindViewById<Button>(Resource.Id.buttonRefresh).AssertNull().Enabled = !_pending;
            FindViewById<LinearLayout>(Resource.Id.linearLayout1).AssertNull().RefreshDrawableState();
        }

        /// <summary>
        /// Переключить состояние узла
        /// </summary>
        /// <param name="node"></param>
        private void Toggle([NotNull]NetNode node)
        {
            _errorDetected = false;
            try
            {
                _pending = true;
                UpdateControls();
                try
                {
                    var arrsToBlock = new List<string>();
                    foreach (var netNode in _nodes)
                    {
                        var nodeMustBelocked = netNode == node ? netNode.State : !netNode.State;
                        if (nodeMustBelocked)
                            arrsToBlock.Add(netNode.MacOrIp);
                    }
                    RouterHelpers.WriteLockedMacsOrIps(arrsToBlock.ToArray());
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
            var macsAndIps = new HashSet<string>(RouterHelpers.ReadLockedMacsAndIps());
            foreach (var netNode in _nodes)
            {
                netNode.State = !macsAndIps.Contains(netNode.MacOrIp);
            }
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
        /// Получить идентификатор ресурса по его имени
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private static int GetIdByResourceName([NotNull] string resourceName)
        {
            return (int) typeof(Resource.Id).GetField(resourceName).GetValue(null);
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
            var index = 1;
            foreach (var netNode in _nodes)
            {
                netNode.Image = FindViewById<ImageView>(GetIdByResourceName($"imageView{index}"));
                netNode.Switch = FindViewById<Switch>(GetIdByResourceName($"switch{index}"));
                netNode.Image.Click += (x, y) => Toggle(netNode);
                netNode.Switch.Click += (x, y) => Toggle(netNode);
                index++;
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

