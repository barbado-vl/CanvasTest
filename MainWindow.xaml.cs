using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Drawing;


using Emgu;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CanvasTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image<Bgr, byte> inputImage;
        List<System.Windows.Point> listPoint = new List<System.Windows.Point>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClickOpenInputImage(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = @"F:\C#\CanvasTest";

                if (openFileDialog.ShowDialog() == true)
                {
                    inputImage = new Image<Bgr, byte>(openFileDialog.FileName);

                    brdrOne.Reset();

                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = new Uri(openFileDialog.FileName);
                    bi.EndInit();

                    ImageBrush ib = new ImageBrush();
                    ib.ImageSource = bi;

                    myCanvas.Background = ib;
                    myCanvas.Width = bi.Width;
                    myCanvas.Height = bi.Height;

                }
                else
                {
                    MessageBox.Show("Incorrect action, try again please", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClickFindContours(object sender, RoutedEventArgs e)
        {
            try
            {
                Image<Gray, byte> outputImage = inputImage.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255));
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat hierarchy = new Mat();
                CvInvoke.FindContours(outputImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.Tree,
                                      Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);

                for(int x = 1; x < contours.Size; x++)
                {
                    VectorOfPoint contour = contours[x];
                    System.Windows.Media.PointCollection myPointCollection = new System.Windows.Media.PointCollection(); 
                    for (int n = 0; n < contour.Size; n++)
                    {
                        System.Drawing.Point point = contour[n];
                        System.Windows.Point myP = new System.Windows.Point(point.X + 0.5, point.Y + 0.5);
                        myPointCollection.Add(myP);
                        myCanvas.PointFromScreen(myP);
                    }

                    Polygon myPolygon = new Polygon();
                    myPolygon.Stroke = System.Windows.Media.Brushes.Red;
                    myPolygon.StrokeThickness = 1;
                    myPolygon.Points = myPointCollection;
                    myPolygon.MouseLeftButtonDown += ClickChild;

                    myCanvas.Children.Add(myPolygon);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClickChild(object sender, RoutedEventArgs e)
        {
            Polygon element = (Polygon)sender;
            System.Windows.Point max = element.Points[0];
            for(int x=1; x < element.Points.Count(); x++)
            {
                if (element.Points[x].Y < max.Y) max.Y = element.Points[x].Y;
            }
            listPoint.Add(max);

            //int a = 0;
        }

        private void DrawLine(object sender, RoutedEventArgs e)
        {
            Line l = new Line();
            l.X1 = listPoint[0].X;
            l.Y1 = listPoint[0].Y + 1;
            l.X2 = listPoint[1].X;
            l.Y2 = listPoint[1].Y + 1;
            l.Stroke = System.Windows.Media.Brushes.Black;
            l.StrokeThickness = 1;

            myCanvas.Children.Add(l);
        }
    }
}
