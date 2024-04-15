using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sview
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:sview"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:sview;assembly=sview"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:SChart/>
    ///
    /// </summary>
    public class SChart : FrameworkElement
    {
        private List<Visual> visuals = new();
        private DrawingVisual layer;
        private List<double>? listPoints;
        private double y_scale;
        private double x_scale;

        private static int Top_Val_Max = 100;
        private static int Top_Val_Min = 0;
        private static int X_sex = 5;
        private static int Y_sex = 10;
        private static int xAsixMax = 26;
        private static int xAsixMin = 0;
        private static int bottom = 30;
        private static int left = 30;

        Pen pen = new Pen(Brushes.Green, 1);
        Pen primaryGrid_Pen = new Pen(Brushes.Black, 1);
        Pen secondGrid_Pen = new Pen()
        {
            DashStyle = new DashStyle()
            {
                Dashes = new DoubleCollection() { 5, 5 }
            },
            Thickness = 0.5,
            Brush = Brushes.Gray,
        };

        public SChart()
        {
            pen.Freeze();
            primaryGrid_Pen.Freeze();
            secondGrid_Pen.Freeze();

            layer = new DrawingVisual();

            visuals.Add(layer);
        }

        public void SetupData(List<double> points)
        {
            listPoints = points;
            DrawContent();
        }

        private void DrawContent()
        {
            var dc = layer.RenderOpen();
            y_scale = (RenderSize.Height - bottom ) / ( Top_Val_Max - Top_Val_Min );
            x_scale = (RenderSize.Width - left ) / ( xAsixMax - xAsixMin );

            //The Y-axis flips up and down
            var mat = new Matrix();
            mat.ScaleAt(1, -1, 0, RenderSize.Height / 2);
            dc.PushTransform(new MatrixTransform(mat));

            //draw Horizontal line of the chart
            for (int y = 0; y <= Top_Val_Max - Top_Val_Min; y += 2)
            {
                Point point1 = new Point(left, y * y_scale + bottom);
                Point point2 = new Point((xAsixMax - xAsixMin) * x_scale + left, y * y_scale + bottom);

                if (y % Y_sex == 0)
                {
                    dc.DrawLine(primaryGrid_Pen, point1, point2);

                    //draw the text of Y asix
                    FormattedText text = new FormattedText(y.ToString(), CultureInfo.GetCultureInfo("en-US"), FlowDirection.LeftToRight, new Typeface("Verdana"), 13, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    var mat3 = new Matrix();
                    mat3.ScaleAt(1, -1,8 - text.Width /2 ,y * y_scale + bottom + text.Height / 2);
                    dc.PushTransform(new MatrixTransform(mat3));
                    dc.DrawText(text, new Point(8 - text.Width/2 , y * y_scale + bottom + text.Height / 2));
                    dc.Pop();
                }
                else
                {
                    dc.DrawLine(secondGrid_Pen, point1, point2);
                }
            }

            //draw the vertical line of the chart
            for (int x = 0; x <= xAsixMax - xAsixMin; x++)
            {
                Point point1 = new Point(x * x_scale + left, bottom);
                Point point2 = new Point(x * x_scale + left, (Top_Val_Max-Top_Val_Min)*y_scale + bottom);


                if (x % X_sex == 0)
                {
                    dc.DrawLine(primaryGrid_Pen, point1, point2);

                    //draw the text of X asix
                    FormattedText text = new FormattedText(x.ToString(), CultureInfo.GetCultureInfo("en-US"), FlowDirection.LeftToRight, new Typeface("Verdana"), 13, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    var mat3 = new Matrix();
                    mat3.ScaleAt(1, -1, x * x_scale + left - text.Width / 2, 8 + text.Height / 2);
                    dc.PushTransform(new MatrixTransform(mat3));
                    dc.DrawText(text, new Point( x * x_scale + left - text.Width / 2, 8 + text.Height / 2));
                    dc.Pop();
                }
                else
                {
                    dc.DrawLine(secondGrid_Pen, point1, point2);
                }
            }


            if (listPoints is not null)
            {
                for (int i = 0; i < listPoints.Count - 1; i++)
                {
                    dc.DrawLine(pen, new Point(i * x_scale /100.0 + left, listPoints[i] * y_scale + bottom), new Point((i + 1)* x_scale / 100.0 + left , listPoints[i + 1] * y_scale + bottom));
                }
            }

            dc.Pop();
            dc.Close();
        }

        protected override int VisualChildrenCount => visuals.Count;
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            DrawContent();
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));
            base.OnRender(drawingContext);
        }
    }
}
