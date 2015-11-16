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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PieChartExamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Dictionary<int, string> pieChartPartNames = new Dictionary<int, string>();
        private Dictionary<int,Byte[]> pieChartPartColor = new Dictionary<int, byte[]>();
        private Dictionary<int, int> pieChartPartData = new Dictionary<int, int>();
        private Dictionary<int, double> pieChartPercent = new Dictionary<int, double>();
        private Dictionary<int, double> pieChartDegrees = new Dictionary<int, double>();

        public MainWindow()
        {
            InitializeComponent();

            CreatePieData(); // reads in the data

            ConvertPieData(); // converts data to proper percentages

            DrawPie(375); // draw the pie
   
        }

        private void CreatePieData()
        {
            // example data :

            // a survey was held under many imaginary people , they could vote for their favorite colors. These are the results:

            // the colors they could vote for

            pieChartPartNames.Add(0, "Red");
            pieChartPartNames.Add(1, "Green");
            pieChartPartNames.Add(2, "Blue");
            pieChartPartNames.Add(3, "Cyan");
            pieChartPartNames.Add(4, "Magenta");
            pieChartPartNames.Add(5, "Yellow");
            pieChartPartNames.Add(6, "Grey");
            pieChartPartNames.Add(7, "Orange");
            pieChartPartNames.Add(8, "Purple");
            pieChartPartNames.Add(9, "Pink");

            // number of votes per color.

            pieChartPartData.Add(0, 23);
            pieChartPartData.Add(1, 34);
            pieChartPartData.Add(2, 76);
            pieChartPartData.Add(3, 12);
            pieChartPartData.Add(4, 43);
            pieChartPartData.Add(5, 57);
            pieChartPartData.Add(6, 29);
            pieChartPartData.Add(7, 67);
            pieChartPartData.Add(8, 83);
            pieChartPartData.Add(9, 65);

            // the actual color codes

            pieChartPartColor.Add(0, new byte[] {0xFF, 0xC0, 0x40, 0x20}); // red 
            pieChartPartColor.Add(1, new byte[] {0xFF, 0x40, 0xC0, 0x20}); // green
            pieChartPartColor.Add(2, new byte[] {0xFF, 0x20, 0x40, 0xE0}); // blue
            pieChartPartColor.Add(3, new byte[] {0xFF, 0x20, 0xE0, 0xE0}); // cyan
            pieChartPartColor.Add(4, new byte[] {0xFF, 0xE0, 0x20, 0x90}); // magenta
            pieChartPartColor.Add(5, new byte[] {0xFF, 0xE0, 0xE0, 0x20}); // yellow
            pieChartPartColor.Add(6, new byte[] {0xFF, 0x90, 0x90, 0x90}); // grey
            pieChartPartColor.Add(7, new byte[] {0xFF, 0xE0, 0x70, 0x20}); // orange
            pieChartPartColor.Add(8, new byte[] {0xFF, 0xE0, 0x20, 0xE0}); // purple
            pieChartPartColor.Add(9, new byte[] {0xFF, 0xE0, 0x90, 0x90}); // pink

        }

        private void ConvertPieData()
        { 

        // convert votes to percentages:

            // counting all votes 

            int countVotes = 0;

            foreach (KeyValuePair<int,int> votes in pieChartPartData)
            {
                countVotes += votes.Value;
            }

            // calculating percentages

            double totalDegrees = 0;

            foreach (KeyValuePair<int, int> votes in pieChartPartData)
            {

                double percentage = ((double)votes.Value/countVotes)*100;

                pieChartPercent.Add(votes.Key,percentage);

                // converting percentage to degree

                // since percentages are relative we need to add them to the total

                totalDegrees += (percentage*3.6);

                pieChartDegrees.Add(votes.Key,totalDegrees);

            }

        }

        private void DrawPie(double radius)
        {
      
            foreach (KeyValuePair<int,double> piePart in pieChartDegrees)
            {
                DrawPiePart(piePart.Key,radius);
            }

        }

        private void DrawPiePart(int currentPart,double radius)
        {

            // defining out values 

            byte[] col;
            double startDegree=0; // first part of the pie needs to start at 0 degrees
            double endDegree=0;

            pieChartPartColor.TryGetValue(currentPart, out col); // reading colors from dictionary

            byte a = col[0];
            byte r = col[1];
            byte g = col[2];
            byte b = col[3];

            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(a, r, g, b));

            pieChartDegrees.TryGetValue(currentPart, out endDegree); // reading degrees from dictionary

            // currentpart - 1 has to be 0 or greater

            if (currentPart>0)  pieChartDegrees.TryGetValue(currentPart - 1, out startDegree);
            
            // defining polygon

            Polygon piePart = new Polygon();
            
            // defining polygon point collection

            PointCollection pointsOnPiePart = new PointCollection();

            // defining keypoints to be drawn

            Point startPoint=new Point();
            Point edgeStartPoint = new Point();
            Point edgeEndPoint = new Point();
            
            // defining center of the pie

            startPoint.X = 0;
            startPoint.Y = 0;

            // calculating start and end point of the edges

            edgeStartPoint = CalculateCirclePoint(radius, startDegree);
            edgeEndPoint = CalculateCirclePoint(radius, endDegree);

            // adding points to the polygon point collection

            pointsOnPiePart.Add(startPoint);
            pointsOnPiePart.Add(edgeStartPoint);

            // calculating the arc points

            for (double degree = startDegree; degree < endDegree; degree +=0.1)
            {
                Point arcPoint = CalculateCirclePoint(radius, degree);
                pointsOnPiePart.Add(arcPoint);
            }

            // adding the last point of the arc 

            pointsOnPiePart.Add(edgeEndPoint);
            
            // link the pointcollection to the polygon

            piePart.Points = pointsOnPiePart;

            // draw the polygon

            piePart.Stroke=brush;
            piePart.Fill=brush;
            piePart.Margin=new Thickness(stage.Width/2,stage.Height/2,0,0);

            stage.Children.Add(piePart);

            // adding the percentage labels

            Label colorLabel= new Label();

            string colorName=pieChartPercent[currentPart].ToString("##.##");

            colorLabel.Content = colorName;

            // center them inside the piepart

            double centerDegree = (startDegree + endDegree)/2;
            Point labelPoint = CalculateCirclePoint(radius - 50, centerDegree);

            // calculating the position

            colorLabel.Margin=new Thickness((stage.Width/2)+labelPoint.X,(stage.Height/2)+labelPoint.Y,0,0);
            colorLabel.Foreground=new SolidColorBrush(Colors.White);
            stage.Children.Add(colorLabel);
        }

        private Point CalculateCirclePoint(double radius, double degree)
        {
            double rad = 360/(2*Math.PI); // standard function to convert degrees to radians

            degree -= 180; // start at the top

            degree /= rad;

            Point circlePoint = new Point();

            circlePoint.X = radius*Math.Sin(-degree); // -degree reverses the anti clockwise drawing
            circlePoint.Y = radius*Math.Cos(-degree);

            return circlePoint;
        }
    }
}
