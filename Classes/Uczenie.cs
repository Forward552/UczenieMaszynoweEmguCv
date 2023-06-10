using System.Text;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Rozpoznawanie_obiektów_na_zdjeciach.Classes
{
    public static class Uczenie
    {

        public static Bitmap useTrainedData(Mat srcImage)
        {
            //var srcImage = new Mat(@"C:\Users\mzdro\source\repos\OpenCVSharp-Samples\OpenCVSharpSample16\CarData\CarData\TestImages_Scale\test-1.pgm");
            //var srcImage = new Mat(@"C:\Users\mzdro\OneDrive\Dokumenty\Modele AI\8dc26b0b-88e1-4fbc-adbf-db07ab8e65ec.jpg");
            //Cv2.ImShow("Source", (Mat)picture;
            //Cv2.WaitKey(1); // do events
            var grayImage = new Mat();
            Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGRA2GRAY);
            Cv2.EqualizeHist(grayImage, grayImage);

            var cascade = new CascadeClassifier(@"C:\Users\mzdro\source\repos\OpenCVSharp-Samples\OpenCVSharpSample16\CarsInfo\data\cascade.xml");

            var cars = cascade.DetectMultiScale(
                image: grayImage,
                scaleFactor: 1.1,
            minNeighbors: 2,
            flags: HaarDetectionTypes.DoRoughSearch | HaarDetectionTypes.ScaleImage,
            minSize: new OpenCvSharp.Size(30, 30)
            );

            Console.WriteLine("Detected cars: {0}", cars.Length);

            var rnd = new Random();
            var count = 1;
            foreach (var carRect in cars)
            {
                var detectedFaceImage = new Mat(srcImage, carRect);
                Cv2.ImShow(string.Format("Car {0}", count), detectedFaceImage);
                Cv2.WaitKey(1); // do events

                var color = Scalar.FromRgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                Cv2.Rectangle(srcImage, carRect, color, 3);


                var detectedFaceGrayImage = new Mat();
                Cv2.CvtColor(detectedFaceImage, detectedFaceGrayImage, ColorConversionCodes.BGRA2GRAY);

                count++;
            }

            return srcImage.ToBitmap();

            Cv2.WaitKey(0);
            srcImage.Dispose();
            Cv2.WaitKey(1); // do events

        }

        public static void createCarImagesFile()
        {
            var sb = new StringBuilder();
            foreach (var file in new DirectoryInfo(@"C:\Users\mzdro\source\repos\OpenCVSharp-Samples\OpenCVSharpSample16\CarData\CarData\TrainImages").GetFiles("*pos-*.pgm"))
            {
                sb.AppendFormat("{0} {1} {2} {3} {4} {5}{6}", file.FullName, 1, 0, 0, 100, 40, Environment.NewLine);
            }
            File.WriteAllText(@"C:\Users\mzdro\source\repos\OpenCVSharp-Samples\OpenCVSharpSample16\CarsInfo\carImages.txt", sb.ToString());
        }

        public static void createNegativeImagesFile()
        {
            var sb = new StringBuilder();
            foreach (var file in new DirectoryInfo(@"C:\Users\mzdro\source\repos\OpenCVSharp-Samples\OpenCVSharpSample16\CarData\CarData\TrainImages").GetFiles("*neg-*.pgm"))
            {
                sb.AppendFormat("{0}{1}", file.FullName, Environment.NewLine);
            }
            File.WriteAllText(@"C:\Users\mzdro\source\repos\OpenCVSharp-Samples\OpenCVSharpSample16\CarsInfo\negativeImages.txt", sb.ToString());
        }
        
    }
}
