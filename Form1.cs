
using System;
using Emgu.CV;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace UczenieMaszynoweEmguCv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTakeScreenShot_Click(object sender, EventArgs e)
        {
            // Wczytujemy obraz z pliku
            Image<Bgr, byte> img = new Image<Bgr, byte>("example_02.jpg");

            // Wczytujemy model YOLO i jego parametry
            Net net = DnnInvoke.ReadNetFromDarknet("yolov3.cfg", "yolov3.weights");
            string[] classes = File.ReadAllLines("coco.names");

            // Zmieniamy rozmiar obrazu i przygotowujemy go do przekazania do sieci
            int width = img.Width;
            int height = img.Height;
            Mat blob = DnnInvoke.BlobFromImage(img, 1 / 255.0, new System.Drawing.Size(416, 416), new MCvScalar(0, 0, 0), true, false);
            net.SetInput(blob);

            // Wykonujemy obliczenia sieci i pobieramy wyniki
            string[] outputLayers = net.GetUnconnectedOutLayersNames();
            VectorOfMat outputs = new VectorOfMat();
            net.Forward(outputs, outputLayers);

            // Przechowujemy informacje o wykrytych obiektach
            List<int> classIds = new List<int>();
            List<float> confidences = new List<float>();
            List<Rectangle> boxes = new List<Rectangle>();

            // Przegl¹damy wyniki sieci i filtrujemy te z nisk¹ pewnoœci¹
            for (int i = 0; i < outputs.Size; i++)
            {
                Mat output = outputs[i];
                float[,] data = output.GetData() as float[,];
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    float confidence = data[j, 4];
                    if (confidence > 0.5)
                    {
                        // Obliczamy wspó³rzêdne prostok¹ta otaczaj¹cego obiekt
                        int centerX = (int)(data[j, 0] * width);
                        int centerY = (int)(data[j, 1] * height);
                        int w = (int)(data[j, 2] * width);
                        int h = (int)(data[j, 3] * height);
                        int x = centerX - w / 2;
                        int y = centerY - h / 2;

                        // Dodajemy informacje o obiekcie do list
                        classIds.Add(GetMaxIndex(data, j));
                        confidences.Add(confidence);
                        boxes.Add(new Rectangle(x, y, w, h));
                    }
                }
            }

            // Usuwamy zduplikowane prostok¹ty za pomoc¹ algorytmu NMS
            int[] indices = CvInvoke.NMSBoxes(boxes.ToArray(), confidences.ToArray(), 0.5f, 0.4f);

            // Rysujemy prostok¹ty i etykiety na obrazie
            MCvFont font = new MCvFont(Emgu.CV.CvEnum.FontFace.HersheySimplex, 1.0, 1.0);
            for (int i = 0; i < indices.Length; i++)
            {
                Rectangle box = boxes[indices[i]];
                string label = classes[classIds[indices[i]]];
                CvInvoke.Rectangle(img, box, new MCvScalar(0, 255, 0), 2);
                CvInvoke.PutText(img, label, new System.Drawing.Point(box.X, box.Y - 10), font.FontFace, font.Size, new Bgr(255, 255, 255).MCvScalar);
            }

            // Wyœwietlamy obraz z wykrytymi obiektami
            CvInvoke.Imshow("Image", img);
            CvInvoke.WaitKey(0);
            CvInvoke.DestroyAllWindows();

        }

        // Pomocnicza metoda do znalezienia indeksu klasy z najwiêkszym wynikiem
        private static int GetMaxIndex(float[,] data, int row)
        {
            int maxIndex = 5;
            float maxScore = data[row, 5];
            for (int i = 6; i < data.GetLength(1); i++)
            {
                float score = data[row, i];
                if (score > maxScore)
                {
                    maxScore = score;
                    maxIndex = i;
                }
            }
            return maxIndex - 5;
        }
    }
}
