using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Java.Lang;
using JetBrains.Annotations;
using SkiaSharp;
using Exception = System.Exception;
using Math = System.Math;
using Path = System.IO.Path;

namespace EnglishWords
{
    /// <summary>
    /// Утилиты 
    /// </summary>
    internal static class Helpers
    {
        
        
        /// <summary>
        /// Средства получения случайного числа
        /// </summary>
        [NotNull]
        public static readonly Random Rnd = new Random();

        /// <summary>
        /// Получить произвольное слово по всем главам в целом
        /// </summary>
        /// <param name="testBook"></param>
        /// <param name="testKind"></param>
        /// <returns></returns>
        public static (string value, string valueTranslate) GetRandomWord([NotNull] this TestBook testBook, TestKind testKind)
        {
            var pair = testBook.AllPairs[Rnd.Next(testBook.AllPairs.Length)];
            return testKind == TestKind.WordIsEnglish ? (pair.rus, pair.eng) : (pair.eng, pair.rus);
        }

        /// <summary>
        /// Получить произвольное слово по всем главам в целом
        /// </summary>
        /// <param name="chapter"></param>
        /// <param name="testKind"></param>
        /// <returns></returns>
        public static (string value, string valueTranslate) GetRandomWord([NotNull] this TestChapter chapter, TestKind testKind)
        {
            var pair = chapter.Pairs[Rnd.Next(chapter.Pairs.Length)];
            return testKind == TestKind.WordIsEnglish ? (pair.rus, pair.eng) : (pair.eng, pair.rus);
        }

        /// <summary>
        /// Получить идентификатор ресурса по его имени
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        internal static int GetIdByResourceName([NotNull] string resourceName)
        {
            return (int)typeof(Resource.Id).GetField(resourceName).GetValue(null);
        }

        /// <summary>
        /// Перемешать порядок следования последовательности
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<T> RandomOrder<T>([NotNull]this IEnumerable<T> source)
        {
            return source.Select(x => (Rnd.Next(Int32.MaxValue), x))
                .OrderBy(x => x.Item1)
                .Select(x => x.Item2);
        }

        /// <summary>
        /// Преобразовать строку с контентом в версионированную строку с контентом
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static (int version, string content) StringContentToVersionedContent([NotNull]this string content)
        {
            var index = content.IndexOf('\n');
            if (index > -1 && int.TryParse(content.Substring(0, index).Trim(), out var version))
                return (version, content);
            return (default(int), content);
        }

        /// <summary>
        /// Прочитать контент или получить null, если контента не существует
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadDiscContent([NotNull]string fileName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullFileName = Path.Combine(documents, fileName);
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : null;
        }

        /// <summary>
        /// Записать контент на диск
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static void WriteDiscContent([NotNull]string fileName, [NotNull]string content)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fullFileName = Path.Combine(documents, fileName);
            File.WriteAllText(fullFileName, content);
        }

        /// <summary>
        /// Выставить положение картинки
        /// </summary>
        /// <param name="view"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="leftMargin"></param>
        /// <param name="topMargin"></param>
        /// <param name="rightMargin"></param>
        /// <param name="bottomMargin"></param>
        private static void SetBoundsInternal([NotNull]View view, double? width, double? height,
            double? leftMargin, double? topMargin, double? rightMargin, double? bottomMargin)
        {
            var layoutParams = view.LayoutParameters;
            if (width.HasValue)
                layoutParams.Width = (int)width.Value;
            if (height.HasValue)
                layoutParams.Height = (int)height.Value;
            if (leftMargin.HasValue || topMargin.HasValue || rightMargin.HasValue || bottomMargin.HasValue)
            {
                var linearLayoutParams = (LinearLayout.LayoutParams)layoutParams;
                if (leftMargin.HasValue)
                    linearLayoutParams.LeftMargin = (int)leftMargin.Value;
                if (topMargin.HasValue)
                    linearLayoutParams.TopMargin = (int)topMargin.Value;
                if (rightMargin.HasValue)
                    linearLayoutParams.RightMargin = (int)rightMargin.Value;
                if (bottomMargin.HasValue)
                    linearLayoutParams.BottomMargin = (int)bottomMargin.Value;
            }
            view.LayoutParameters = layoutParams;
        }

        /// <summary>
        /// Выставить положение картинки
        /// </summary>
        /// <param name="view"></param>
        /// <param name="width"></param>
        /// <param name="leftMargin"></param>
        /// <param name="rightMargin"></param>
        public static void SetBoundsByWidth([NotNull]this View view, int? width = null, 
            int? leftMargin = null, int? rightMargin = null)
        {
            var viewActivity = (Activity)view.Context;
            var scale = viewActivity.Resources.DisplayMetrics.WidthPixels / 100.0;
            SetBoundsInternal(view, width * scale, null, leftMargin * scale, null, rightMargin * scale, null);
        }

        /// <summary>
        /// Выставить положение картинки
        /// </summary>
        /// <param name="view"></param>
        /// <param name="height"></param>
        /// <param name="topMargin"></param>
        /// <param name="bottomMargin"></param>
        public static void SetBoundsByHeight([NotNull]this View view, int? height = null,
            int? topMargin = null, int? bottomMargin = null)
        {
            var viewActivity = (Activity)view.Context;
            var scale = viewActivity.Resources.DisplayMetrics.HeightPixels / 100.0;
            SetBoundsInternal(view, null, height * scale, null, topMargin * scale, null, bottomMargin * scale);
        }

        /// <summary>
        /// Преобразовать цвет в html-формат
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToHtmlValue(this Color value)
        {
            return "#" + value.ToArgb().ToString("X8").Substring(2, 6);
        }

        /// <summary>
        /// Назначить из Assets\svg SVG-картинку на ImageView
        /// </summary>
        /// <param name="imgView"></param>
        /// <param name="fileName"></param>
        /// <param name="wantedWidth"></param>
        /// <param name="wantedHeight"></param>
        private static void AssignSvg([NotNull]ImageView imgView, string fileName, int wantedWidth, int wantedHeight)
        {
            const int bestQuality = 100;
            var parent = (View)imgView.Parent;
            var parentColor = (parent.Background as ColorDrawable)?.Color;
            var viewActivity = (Activity)imgView.Context;
            using (var reader = new StreamReader(viewActivity.Assets.Open(Path.Combine("svg", Path.ChangeExtension(fileName, "svg").AssertNull()))))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);
                xmlDoc.DocumentElement.VoidAssertNull();
                xmlDoc.DocumentElement.SetAttribute("width", wantedWidth.ToString());
                xmlDoc.DocumentElement.SetAttribute("height", wantedHeight.ToString());
                if (parentColor.HasValue)
                {
                    var bkColorRect = xmlDoc.CreateElement("rect");
                    bkColorRect.SetAttribute("style", $"fill: {parentColor.Value.ToHtmlValue()}");
                    bkColorRect.SetAttribute("x", "0");
                    bkColorRect.SetAttribute("y", "0");
                    bkColorRect.SetAttribute("width", wantedWidth.ToString());
                    bkColorRect.SetAttribute("height", wantedHeight.ToString());
                    xmlDoc.DocumentElement.AssertNull().InsertAfter(bkColorRect, null);
                }
                using (var alterStream = new MemoryStream())
                {
                    xmlDoc.Save(alterStream);
                    alterStream.Seek(0, SeekOrigin.Begin);
                    var svg = new SkiaSharp.Extended.Svg.SKSvg(new SKSize(wantedWidth, wantedHeight));
                    svg.Load(alterStream);
                    using (var svgBitmap = new SKBitmap((int)svg.CanvasSize.Width, (int)svg.CanvasSize.Height))
                    {
                        using (var canvas = new SKCanvas(svgBitmap))
                        {
                            canvas.DrawPicture(svg.Picture);
                            canvas.Flush();
                            canvas.Save();
                        }
                        using (var image = SKImage.FromBitmap(svgBitmap))
                        using (var data = image.Encode(SKEncodedImageFormat.Png, bestQuality))
                        {
                            var bmpArray = data.ToArray();
                            imgView.SetImageBitmap(BitmapFactory.DecodeByteArray(bmpArray, 0, bmpArray.Length));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Нормализовать значение в соотвествии с масштабом экрана
        /// </summary>
        /// <param name="value"></param>
        /// <param name="scale"></param>
        /// <param name="sourceForSign"></param>
        /// <returns></returns>
        private static int ToScreen(this int value, (double scaleX, double scaleY) scale, int? sourceForSign = null)
        {
            var externalSign = sourceForSign.HasValue ? Math.Sign(sourceForSign.Value) : (int?)null;
            switch (externalSign ?? Math.Sign(value))
            {
                case -1: return (int)(externalSign.HasValue ? 
                    externalSign * Math.Abs(value * scale.scaleX) : 
                    - value * scale.scaleX);
                case 1: return (int)(externalSign.HasValue ?
                    value * scale.scaleX :
                    Math.Abs(value * scale.scaleY));
                default: throw new Exception("0 запрещен!");
            }
        }

        /// <summary>
        /// Нормализовать значение в соотвествии с масштабом экрана
        /// </summary>
        /// <param name="value"></param>
        /// <param name="scale"></param>
        /// <param name="sourceForSign"></param>
        /// <returns></returns>
        private static int? ToScreen(this int? value, (double scaleX, double scaleY) scale, int? sourceForSign = null)
        {
            return value == null ? (int?)null : value.Value.ToScreen(scale, sourceForSign);
        }

        /// <summary>
        /// Заполучить параметры масштабирования экрана
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static (double scaleX, double scaleY) GetViewScale([NotNull]this View view)
        {
            return (view.Resources.DisplayMetrics.WidthPixels / 100.0, view.Resources.DisplayMetrics.HeightPixels / 100.0);
        }

        /// <summary>
        /// Заполучить параметры масштабирования экрана
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static (double scaleX, double scaleY, int screenWidth, int screenHeight) GetViewScale([NotNull]this Activity activity)
        {
            return (activity.Resources.DisplayMetrics.WidthPixels / 100.0, 
                activity.Resources.DisplayMetrics.HeightPixels / 100.0, 
                activity.Resources.DisplayMetrics.WidthPixels,
                activity.Resources.DisplayMetrics.HeightPixels);
        }

        /// <summary>
        /// Выставить размеры ImageView и заодно подгрузить Svg
        /// </summary>
        /// <param name="imageView"></param>
        /// <param name="fileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="leftMargin"></param>
        /// <param name="topMargin"></param>
        public static void SetSvgBounds([NotNull]this ImageView imageView, string fileName, int width, int height,
            int? leftMargin = null, int? topMargin = null)
        {
            var scale = GetViewScale(imageView);
            var realWidth = width.ToScreen(scale);
            var realHeight = height.ToScreen(scale);
            SetBoundsInternal(imageView, realWidth, realHeight, leftMargin.ToScreen(scale), topMargin.ToScreen(scale), null, null);
            AssignSvg(imageView, fileName, realWidth, realHeight);
        }

        /// <summary>
        /// Выставить размеры ImageView и заодно подгрузить Svg
        /// </summary>
        /// <param name="imageView"></param>
        /// <param name="fileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="leftMargin"></param>
        /// <param name="topMargin"></param>
        public static void SetSvgBoundsWithDirectMargin([NotNull]this ImageView imageView, string fileName, int width, int height,
            double? leftMargin = null, double? topMargin = null)
        {
            var scale = GetViewScale(imageView);
            var realWidth = width.ToScreen(scale);
            var realHeight = height.ToScreen(scale);
            SetBoundsInternal(imageView, realWidth, realHeight, leftMargin, topMargin, null, null);
            AssignSvg(imageView, fileName, realWidth, realHeight);
        }

        /// <summary>
        /// Выставить размеры ImageView и заодно подгрузить Svg
        /// </summary>
        /// <param name="imageView"></param>
        /// <param name="fileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="leftMargin"></param>
        /// <param name="topMargin"></param>
        public static void SetSvgBoundHorCentered([NotNull]this ImageView imageView, string fileName, int width, int height,
            int? leftMargin = null, int? topMargin = null)
        {
            var scale = GetViewScale(imageView);
            var realWidth = width.ToScreen(scale);
            var realHeight = height.ToScreen(scale);
            var realLeftMargin = leftMargin.ToScreen(scale, width).GetValueOrDefault(0);
            var extendedLeftMargin = (imageView.Resources.DisplayMetrics.WidthPixels - realWidth - realLeftMargin) / 2;
            SetBoundsInternal(imageView, realWidth, realHeight, realLeftMargin + extendedLeftMargin, topMargin.ToScreen(scale, height), null, null);
            AssignSvg(imageView, fileName, realWidth, realHeight);
        }
    }
}