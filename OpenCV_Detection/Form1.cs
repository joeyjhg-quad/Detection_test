using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.XImgProc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using System.Diagnostics;
using OpenCvSharp.Features2D;


namespace OpenCV_Detection
{

    public partial class Form1 : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        ComparePoint comparePoint;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            log.Debug(MethodBase.GetCurrentMethod().Name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dig = new OpenFileDialog();
                if (dig.ShowDialog() == DialogResult.OK)
                {
                    // 해당 경로에서 이미지를 가지고와 Mat타입으로 변환
                    //Mat img_color = Cv2.ImRead(dig.FileName);
                    //Mat img_gray = new Mat();
                    //Cv2.CvtColor(img_color, img_gray, ColorConversionCodes.BGR2GRAY);

                    ////    //// grayImage 위에 텍스트 쓰기
                    ////    //string text = "Hello, OpenCvSharp!";
                    ////    //OpenCvSharp.Point textPosition = new OpenCvSharp.Point(10, 50); // 이미지 상의 텍스트 위치
                    ////    //HersheyFonts font = HersheyFonts.HersheySimplex; // 폰트 선택
                    ////    //double fontSize = 1.0; // 폰트 크기
                    ////    //Scalar textColor = new Scalar(0, 255, 0); // 색상(Green)
                    ////    //int textThickness = 2; // 텍스트 두께
                    ////    //Cv2.PutText(grayImage, text, textPosition, font, fontSize, textColor, textThickness);
                    //pictureBox1.Image = img_gray.ToBitmap();
                    HersheyFonts font = HersheyFonts.HersheySimplex;

                    Mat src = Cv2.ImRead(dig.FileName);
                    // ROI 비율
                    //float percentage = 60f;

                    //// 이미지의 크기 가져오기
                    //int width = src.Width;
                    //int height = src.Height;

                    //// ROI의 시작과 끝 좌표 계산
                    //int startX = (int)(width * (percentage / 100) / 2); // 30% 지점
                    //int endX = (int)(width - startX); // 70% 지점
                    //int startY = (int)(height * (percentage / 100) / 2); // 25% 지점
                    //int endY = (int)(height - startY); // 75% 지점

                    //// ROI 설정
                    //Rect roi = new Rect(startX, startY, width - startX * 2, height - startY * 2);
                    //Mat roiMat = new Mat(src, roi);
                    //src = new Mat(src, roi);
                    //Cv2.Resize(src, src, new OpenCvSharp.Size(500, 500));
                    Mat black = new Mat();
                    Mat dst = src.Clone();
                    Mat gray = new Mat();
                    Mat binary = new Mat();
                    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                    Cv2.Threshold(gray, binary, -1, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                    Cv2.Resize(binary, binary, new OpenCvSharp.Size(1000, 1000));
                    Cv2.ImShow("black", binary);
                    Cv2.WaitKey(0);

                    OpenCvSharp.Point[][] contours;
                    HierarchyIndex[] hierarchy;

                    Cv2.InRange(src, new Scalar(0, 0, 0), new Scalar(240, 240, 240), black);
                    //Mat element1 = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
                    //Cv2.Dilate(black, black, element1, null, 12);
                    //Cv2.Erode(black, black, element1, null, 12);
                    //Cv2.Dilate(black, black, new Mat(), null, 3);
                    //Cv2.Resize(black, black, new OpenCvSharp.Size(1000, 1000));
                    // Cv2.ImShow("black", black);
                    //Cv2.WaitKey(0);
                    //Cv2.FindContours(black, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);
                    Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

                    List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]>();
                    List<OpenCvSharp.Point> centers = new List<OpenCvSharp.Point>();
                    foreach (OpenCvSharp.Point[] p in contours)
                    {
                        double length = Cv2.ArcLength(p, true);
                        if (length > 0 && length < 300)
                        {
                            new_contours.Add(p);

                            Moments M = Cv2.Moments(p);
                            if (M.M00 != 0)
                            {
                                int cX = (int)(M.M10 / M.M00);
                                int cY = (int)(M.M01 / M.M00);
                                centers.Add(new OpenCvSharp.Point(cX, cY));
                            }
                        }
                    }
                    Cv2.DrawContours(dst, new_contours, -1, new Scalar(0, 0, 255), 5, LineTypes.Link4, null, 1);

                    double tolerance = 25; // y 좌표를 그룹화할 때 사용할 오차 범위
                    List<List<OpenCvSharp.Point>> rows = new List<List<OpenCvSharp.Point>>();

                    // centers를 y 오름차순으로 정렬
                    centers.Sort((p1, p2) => p1.Y.CompareTo(p2.Y));

                    // 첫 번째 행 초기화
                    List<OpenCvSharp.Point> currentRow = new List<OpenCvSharp.Point> { centers[0] };
                    double currentY = centers[0].Y;

                    for (int i = 1; i < centers.Count; i++)
                    {
                        if (Math.Abs(centers[i].Y - currentY) <= tolerance)
                        {
                            // 같은 행에 속하는 점이라면 추가
                            currentRow.Add(centers[i]);
                        }
                        else
                        {
                            // 새로운 행으로 시작
                            rows.Add(currentRow);
                            currentRow = new List<OpenCvSharp.Point> { centers[i] };
                            currentY = centers[i].Y;
                        }
                    }

                    // 마지막 행 추가
                    if (currentRow.Count > 0)
                    {
                        rows.Add(currentRow);
                    }

                    // 각 행을 x 좌표 기준으로 정렬
                    foreach (var row in rows)
                    {
                        row.Sort((p1, p2) => p1.X.CompareTo(p2.X));
                    }

                    // 행별로 선을 잇고 번호를 출력
                    int count = 1;
                    for (int i = 0; i < rows.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            for (int j = 0; j < rows[i].Count; j++)
                            {
                                // 번호 출력
                                Cv2.PutText(dst, $"{count}", rows[i][j], font, 0.5, new Scalar(255, 0, 0), 1);
                                Cv2.Circle(dst, rows[i][j], 2, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);

                                count++;

                                // 선 그리기 (다음 점이 있는 경우에만)
                                if (j < rows[i].Count - 1)
                                {
                                    Cv2.Line(dst, rows[i][j], rows[i][j + 1], new Scalar(255, 0, 0), 1, LineTypes.AntiAlias);
                                }
                            }

                            // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
                            if (i < rows.Count - 1)
                            {
                                Cv2.Line(dst, rows[i][rows[i].Count - 1], rows[i + 1][rows[i + 1].Count - 1], new Scalar(255, 0, 0), 1, LineTypes.AntiAlias);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < rows[i].Count; j++)
                            {
                                int reverse_j = rows[i].Count - j - 1;
                                // 번호 출력
                                Cv2.PutText(dst, $"{count}", rows[i][reverse_j], font, 0.5, new Scalar(255, 0, 0), 1);
                                Cv2.Circle(dst, rows[i][reverse_j], 2, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);
                                count++;

                                // 선 그리기 (다음 점이 있는 경우에만)
                                if (j < rows[i].Count - 1)
                                {
                                    Cv2.Line(dst, rows[i][reverse_j], rows[i][reverse_j - 1], new Scalar(255, 0, 0), 1, LineTypes.AntiAlias);
                                }
                            }

                            // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
                            if (i < rows.Count - 1)
                            {
                                Cv2.Line(dst, rows[i][0], rows[i + 1][0], new Scalar(255, 0, 0), 1, LineTypes.AntiAlias);
                            }
                        }

                    }

                    Cv2.Resize(dst, dst, new OpenCvSharp.Size(1000, 1000));
                    Cv2.ImShow("dst", dst);
                    Cv2.WaitKey(0);
                    // pictureBox1.Image = dst.ToBitmap();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            log.Debug(MethodBase.GetCurrentMethod().Name + "() Start");
            try
            {
                OpenFileDialog dig = new OpenFileDialog();
                if (dig.ShowDialog() == DialogResult.OK)
                {
                    log.Info("DialogResult.OK");
                    Mat src = Cv2.ImRead(dig.FileName);
                    log.Debug("dig.FileName = " + dig.FileName);
                    Mat gray = new Mat();
                    Mat binary = new Mat();
                    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                    //Cv2.Resize(gray, gray, new OpenCvSharp.Size(800, 800));

                    //Cv2.ImShow("gray", gray);

                    Cv2.NamedWindow("binary");
                    int a = 0;
                    Cv2.CreateTrackbar("threshold", "binary", ref a, 255);
                    Cv2.SetTrackbarPos("threshold", "binary", 253);

                    while (true)
                    {
                        int val = Cv2.GetTrackbarPos("threshold", "binary");
                        //Cv2.Threshold(gray, binary, -1, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                        Cv2.Threshold(gray, binary, a, 255, ThresholdTypes.Binary);
                        Cv2.ImShow("binary", binary);
                        if (Cv2.WaitKey(1) == 27)
                            break;
                    }
                    //Mat Thre_clone = binary.Clone();
                    Mat element1 = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
                    Cv2.Dilate(binary, binary, element1, null, 2);  //팽창
                    Cv2.Erode(binary, binary, element1, null, 10);   //침식
                    //Cv2.Resize(binary, binary, new OpenCvSharp.Size(800, 800));
                    //Cv2.ImShow("binary2", binary);


                    OpenCvSharp.Point[][] contours;
                    HierarchyIndex[] hierarchy;

                    Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
                    log.Info("FindContours.OK");

                    //Cv2.DrawContours(src, contours, -1, new Scalar(0, 0, 255), 5, LineTypes.Link8, null, 1);
                    //Cv2.Resize(src, src, new OpenCvSharp.Size(1200, 1200));
                    //Cv2.ImShow("contours", src);

                    //크기 비교 - 중간 네모 찾기
                    List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]>();
                    List<OpenCvSharp.Point> center = new List<OpenCvSharp.Point>();

                    List<int> list_area = new List<int>();
                    List<int> list_round = new List<int>();

                    foreach (OpenCvSharp.Point[] p in contours)
                    {
                        double area = Cv2.ContourArea(p);
                        double round = Cv2.ArcLength(p, true);
                        if (area > Constants_btn2.AREA_LOW && area < Constants_btn2.AREA_HIGH && round > Constants_btn2.ROUND_LOW && round < Constants_btn2.ROUND_HIGH)
                        {
                            new_contours.Add(p);
                            Moments M = Cv2.Moments(p);

                            list_area.Add((int)area);
                            list_round.Add((int)round);

                            int x = (int)(M.M10 / M.M00);
                            int y = (int)(M.M01 / M.M00);
                            center.Add(new OpenCvSharp.Point(x, y));
                        }

                    }
                    list_area.Sort();
                    list_round.Sort();
                    int areaMin = list_area[0];
                    int areaMax = list_area[list_area.Count - 1];
                    int roundMin = list_round[0];
                    int roundMax = list_round[list_round.Count - 1];
                    lb_areaMin.Text = $"{areaMin}";
                    lb_areaMax.Text = $"{areaMax}";
                    lb_roundMin.Text = $"{roundMin}";
                    lb_roundMax.Text = $"{roundMax}";
                    lb_count.Text = $"{list_area.Count}";


                    Cv2.DrawContours(src, new_contours, -1, new Scalar(0, 0, 255), 5, LineTypes.Link8, null, 1);
                    log.Info("DrawContours.OK");

                    //1. 센터값 정렬
                    center.Sort((p1, p2) => p1.Y.CompareTo(p2.Y));
                    //2. y값이 최대인 점 식별
                    OpenCvSharp.Point Y_max = center[0];
                    //3. 기울기 식별
                    double m = 0;
                    //3-1. 가장 가까운 점 2곳 식별
                    OpenCvSharp.Point close1 = new OpenCvSharp.Point();
                    OpenCvSharp.Point close2 = new OpenCvSharp.Point();
                    double dis_close1 = double.MaxValue; // 초기화: 최대값으로 설정
                    double dis_close2 = double.MaxValue;

                    foreach (OpenCvSharp.Point p in center)
                    {
                        if (p == Y_max) // 본인 제외
                            continue;

                        double currentDistance = Math.Sqrt(Math.Pow(p.X - Y_max.X, 2) + Math.Pow(p.Y - Y_max.Y, 2));

                        if (currentDistance < dis_close1)
                        {
                            // 기존 close1을 close2로 밀어냄
                            close2 = close1;
                            dis_close2 = dis_close1;

                            // close1 갱신
                            close1 = p;
                            dis_close1 = currentDistance;
                        }
                        else if (currentDistance < dis_close2)
                        {
                            // close2만 갱신
                            close2 = p;
                            dis_close2 = currentDistance;
                        }
                    }
                    //3-2. y값이 근사한 점을 찾아, dx/dy로 기울기를 구함
                    if (Math.Abs(Y_max.Y - close1.Y) < Math.Abs(Y_max.Y - close2.Y))
                        m = (double)(Y_max.Y - close1.Y) / (double)(Y_max.X - close1.X);
                    else
                        m = (double)(Y_max.Y - close2.Y) / (double)(Y_max.X - close2.X);

                    double Threshold = 0.05;
                    //6. 4,5를 반복해 다음 점들이 없을때 까지 반복
                    List<List<OpenCvSharp.Point>> rows = new List<List<OpenCvSharp.Point>>();
                    while (center.Count > 0)
                    {
                        List<OpenCvSharp.Point> removePoint = new List<OpenCvSharp.Point>();
                        List<OpenCvSharp.Point> currentRow = new List<OpenCvSharp.Point>();
                        foreach (OpenCvSharp.Point p in center)
                        {
                            if (Y_max == p)
                            {
                                currentRow.Add(p);
                                removePoint.Add(p);
                                continue;
                            }
                            double p_m = ((double)(Y_max.Y - p.Y) / (double)(Y_max.X - p.X));
                            if (p_m > m - Threshold && p_m < m + Threshold)
                            {
                                currentRow.Add(p);
                                removePoint.Add(p);
                            }
                        }

                        foreach (OpenCvSharp.Point p in removePoint)
                            center.Remove(p);
                        if (center.Count > 0)
                            Y_max = center[0];
                        rows.Add(currentRow);
                    }
                    //4. y값이 최대인 점에서 기울기에 근사한 점들을 모두 구함
                    //5. 다음 좌표는, 4번의 점들을 뺀 나머지중 y값이 최대인 점으로 함.
                    // 각 행을 x 좌표 기준으로 정렬
                    foreach (var row in rows)
                    {
                        row.Sort((p1, p2) => p1.X.CompareTo(p2.X));
                    }
                    log.Info("Sort.OK");

                    // 행별로 선을 잇고 번호를 출력
                    HersheyFonts font = HersheyFonts.HersheySimplex;
                    int count = 1;
                    for (int i = 0; i < rows.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            for (int j = 0; j < rows[i].Count; j++)
                            {
                                // 번호 출력
                                Cv2.PutText(src, $"{count}", rows[i][j], font, 3, new Scalar(255, 0, 0), 3);
                                Cv2.Circle(src, rows[i][j], 2, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);

                                count++;

                                // 선 그리기 (다음 점이 있는 경우에만)
                                if (j < rows[i].Count - 1)
                                {
                                    Cv2.Line(src, rows[i][j], rows[i][j + 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                                }
                            }

                            // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
                            if (i < rows.Count - 1)
                            {
                                Cv2.Line(src, rows[i][rows[i].Count - 1], rows[i + 1][rows[i + 1].Count - 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < rows[i].Count; j++)
                            {
                                int reverse_j = rows[i].Count - j - 1;
                                // 번호 출력
                                Cv2.PutText(src, $"{count}", rows[i][reverse_j], font, 3, new Scalar(255, 0, 0), 3);
                                Cv2.Circle(src, rows[i][reverse_j], 2, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);
                                count++;

                                // 선 그리기 (다음 점이 있는 경우에만)
                                if (j < rows[i].Count - 1)
                                {
                                    Cv2.Line(src, rows[i][reverse_j], rows[i][reverse_j - 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                                }
                            }

                            // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
                            if (i < rows.Count - 1)
                            {
                                Cv2.Line(src, rows[i][0], rows[i + 1][0], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                            }
                        }
                    }
                    Cv2.Resize(src, src, new OpenCvSharp.Size(800, 800));
                    Cv2.ImShow("contours", src);
                    log.Info("성공");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Warn(ex);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Stopwatch 객체 생성
            Stopwatch stopwatch = new Stopwatch();

            // 타이머 시작
            try
            {
                OpenFileDialog dig = new OpenFileDialog();
                if (dig.ShowDialog() == DialogResult.OK)
                {
                    stopwatch.Start();
                    log.Info("DialogResult.OK");
                    Mat src = Cv2.ImRead(dig.FileName);
                    log.Debug("dig.FileName = " + dig.FileName);
                    Mat gray = new Mat();
                    Mat binary = new Mat();
                    Mat dil_ero = new Mat();
                    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                    Cv2.NamedWindow("binary");
                    Cv2.NamedWindow("dil_ero");

                    int val = 0, val2 = 0, val3 = 0;

                    Cv2.CreateTrackbar("threshold", "binary", ref val, 255);
                    Cv2.CreateTrackbar("dilate", "dil_ero", ref val2, 20);
                    Cv2.CreateTrackbar("erode", "dil_ero", ref val3, 20);

                    Mat element1 = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));

                    while (true)
                    {
                        val = Cv2.GetTrackbarPos("threshold", "binary");
                        val2 = Cv2.GetTrackbarPos("dilate", "dil_ero");
                        val3 = Cv2.GetTrackbarPos("erode", "dil_ero");

                        Cv2.Threshold(gray, binary, val, 255, ThresholdTypes.Binary);
                        Cv2.ImShow("binary", binary);

                        Cv2.Dilate(binary, binary, element1, null, val2);
                        Cv2.Erode(binary, binary, element1, null, val3);

                        Cv2.ImShow("dil_ero", binary);

                        if (Cv2.WaitKey(30) == 27) // ESC 키
                            break;
                    }

                    //Mat Thre_clone = binary.Clone();
                    //Cv2.Dilate(binary, binary, element1, null, 10);  //팽창
                    //Cv2.Erode(binary, binary, element1, null, 7);   //침식
                    //Cv2.Resize(binary, binary, new OpenCvSharp.Size(800, 800));
                    //Cv2.ImShow("binary2", binary);


                    OpenCvSharp.Point[][] contours;
                    HierarchyIndex[] hierarchy;

                    Cv2.FindContours(binary, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
                    //log.Info("FindContours.OK");

                    //Cv2.DrawContours(src, contours, -1, new Scalar(0, 0, 255), 5, LineTypes.Link8, null, 1);
                    //Cv2.Resize(src, src, new OpenCvSharp.Size(800, 800));
                    //Cv2.ImShow("contours", src);
                    //크기 비교 - 중간 네모 찾기
                    List<OpenCvSharp.Point[]> new_contours = new List<OpenCvSharp.Point[]>();
                    List<OpenCvSharp.Point> center = new List<OpenCvSharp.Point>();

                    List<int> list_area = new List<int>();
                    List<int> list_round = new List<int>();

                    foreach (OpenCvSharp.Point[] p in contours)
                    {
                        double area = Cv2.ContourArea(p);
                        double round = Cv2.ArcLength(p, true);
                        if (area > Constants.AREA_LOW && area < Constants.AREA_HIGH && round > Constants.ROUND_LOW && round < Constants.ROUND_HIGH)
                        {
                            new_contours.Add(p);
                            Moments M = Cv2.Moments(p);

                            list_area.Add((int)area);
                            list_round.Add((int)round);

                            int x = (int)(M.M10 / M.M00);
                            int y = (int)(M.M01 / M.M00);
                            center.Add(new OpenCvSharp.Point(x, y));
                        }

                    }
                    list_area.Sort();
                    list_round.Sort();
                    int areaMin = list_area[0];
                    int areaMax = list_area[list_area.Count - 1];
                    int roundMin = list_round[0];
                    int roundMax = list_round[list_round.Count - 1];
                    lb_areaMin.Text = $"{areaMin}";
                    lb_areaMax.Text = $"{areaMax}";
                    lb_roundMin.Text = $"{roundMin}";
                    lb_roundMax.Text = $"{roundMax}";
                    lb_count.Text = $"{list_area.Count}";


                    Cv2.DrawContours(src, new_contours, -1, new Scalar(0, 0, 255), 5, LineTypes.Link8, null, 1);
                    log.Info("DrawContours.OK");

                    //1. 센터값 정렬
                    center.Sort((p1, p2) => p1.Y.CompareTo(p2.Y));
                    //2. y값이 최대인 점 식별
                    OpenCvSharp.Point Y_max = center[0];
                    //3. 기울기 식별
                    double m = 0;
                    //3-1. 가장 가까운 점 2곳 식별
                    OpenCvSharp.Point close1 = new OpenCvSharp.Point();
                    OpenCvSharp.Point close2 = new OpenCvSharp.Point();
                    double dis_close1 = double.MaxValue; // 초기화: 최대값으로 설정
                    double dis_close2 = double.MaxValue;

                    foreach (OpenCvSharp.Point p in center)
                    {
                        if (p == Y_max) // 본인 제외
                            continue;

                        double currentDistance = Math.Sqrt(Math.Pow(p.X - Y_max.X, 2) + Math.Pow(p.Y - Y_max.Y, 2));

                        if (currentDistance < dis_close1)
                        {
                            // 기존 close1을 close2로 밀어냄
                            close2 = close1;
                            dis_close2 = dis_close1;

                            // close1 갱신
                            close1 = p;
                            dis_close1 = currentDistance;
                        }
                        else if (currentDistance < dis_close2)
                        {
                            // close2만 갱신
                            close2 = p;
                            dis_close2 = currentDistance;
                        }
                    }
                    //3-2. y값이 근사한 점을 찾아, dx/dy로 기울기를 구함
                    if (Math.Abs(Y_max.Y - close1.Y) < Math.Abs(Y_max.Y - close2.Y))
                        m = (double)(Y_max.Y - close1.Y) / (double)(Y_max.X - close1.X);
                    else
                        m = (double)(Y_max.Y - close2.Y) / (double)(Y_max.X - close2.X);

                    double Threshold = 0.07;
                    //6. 4,5를 반복해 다음 점들이 없을때 까지 반복
                    List<List<OpenCvSharp.Point>> rows = new List<List<OpenCvSharp.Point>>();
                    while (center.Count > 0)
                    {
                        List<OpenCvSharp.Point> removePoint = new List<OpenCvSharp.Point>();
                        List<OpenCvSharp.Point> currentRow = new List<OpenCvSharp.Point>();
                        foreach (OpenCvSharp.Point p in center)
                        {
                            if (Y_max == p)
                            {
                                currentRow.Add(p);
                                removePoint.Add(p);
                                continue;
                            }
                            double p_m = ((double)(Y_max.Y - p.Y) / (double)(Y_max.X - p.X));
                            if (p_m > m - Threshold && p_m < m + Threshold)
                            {
                                currentRow.Add(p);
                                removePoint.Add(p);
                            }
                        }

                        foreach (OpenCvSharp.Point p in removePoint)
                            center.Remove(p);
                        if (center.Count > 0)
                            Y_max = center[0];
                        rows.Add(currentRow);
                    }
                    //4. y값이 최대인 점에서 기울기에 근사한 점들을 모두 구함
                    //5. 다음 좌표는, 4번의 점들을 뺀 나머지중 y값이 최대인 점으로 함.
                    // 각 행을 x 좌표 기준으로 정렬
                    foreach (var row in rows)
                    {
                        row.Sort((p1, p2) => p1.X.CompareTo(p2.X));
                    }
                    log.Info("Sort.OK");

                    // 행별로 선을 잇고 번호를 출력
                    HersheyFonts font = HersheyFonts.HersheySimplex;
                    int count = 1;
                    for (int i = 0; i < rows.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            for (int j = 0; j < rows[i].Count; j++)
                            {
                                // 번호 출력
                                //Cv2.PutText(src, $"{count}", rows[i][j], font, 3, new Scalar(255, 0, 0), 3);
                                Cv2.Circle(src, rows[i][j], 10, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);

                                count++;

                                // 선 그리기 (다음 점이 있는 경우에만)
                                if (j < rows[i].Count - 1)
                                {
                                    Cv2.Line(src, rows[i][j], rows[i][j + 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                                }
                            }

                            // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
                            if (i < rows.Count - 1)
                            {
                                Cv2.Line(src, rows[i][rows[i].Count - 1], rows[i + 1][rows[i + 1].Count - 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < rows[i].Count; j++)
                            {
                                int reverse_j = rows[i].Count - j - 1;
                                // 번호 출력
                                //Cv2.PutText(src, $"{count}", rows[i][reverse_j], font, 3, new Scalar(255, 0, 0), 3);
                                Cv2.Circle(src, rows[i][reverse_j], 10, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);
                                count++;

                                // 선 그리기 (다음 점이 있는 경우에만)
                                if (j < rows[i].Count - 1)
                                {
                                    Cv2.Line(src, rows[i][reverse_j], rows[i][reverse_j - 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                                }
                            }

                            // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
                            if (i < rows.Count - 1)
                            {
                                Cv2.Line(src, rows[i][0], rows[i + 1][0], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                            }
                        }
                    }
                    stopwatch.Stop();
                    label6.Text = stopwatch.ElapsedMilliseconds.ToString();
                    Cv2.Resize(src, src, new OpenCvSharp.Size(800, 800));
                    Cv2.ImShow("contours", src);
                    log.Info("성공");
                }
            }
            catch (Exception ex) { }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            test();
            //이미지 및 템플릿 로드
            //OpenFileDialog dig1 = new OpenFileDialog();
            //OpenFileDialog dig2 = new OpenFileDialog();

            //dig1.ShowDialog();
            //dig2.ShowDialog();
            //Mat image = Cv2.ImRead(dig1.FileName, ImreadModes.Color);
            //Mat templateImage = Cv2.ImRead(dig2.FileName, ImreadModes.Color);
            ////Cv2.Rectangle(image, new OpenCvSharp.Point(100, 100), new OpenCvSharp.Point(150, 150), Scalar.Black, -1); // -1: 채우기
            ////Cv2.ImShow("image", image);
            ////Cv2.WaitKey(0);
            //// 색 공간 변환 (BGR -> RGB)    

            //// 이미지를 흑백으로 변환 (그레이스케일)
            //Mat grayImage = new Mat();
            //Mat grayTemplate = new Mat();
            //Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
            //Cv2.CvtColor(templateImage, grayTemplate, ColorConversionCodes.BGR2GRAY);

            //// 오츠 알고리즘 적용 이진화
            //Mat binaryImage = new Mat();
            //Mat binaryTemplate = new Mat();
            //double otsuThreshImage = Cv2.Threshold(grayImage, binaryImage, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
            //double otsuThreshTemplate = Cv2.Threshold(grayTemplate, binaryTemplate, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            //// 이진화된 이미지 테스트용으로 표시
            //Mat resizedBinaryImage = binaryImage.Clone();
            //Mat resizedBinaryTemplate = binaryTemplate.Clone();
            //Cv2.Resize(resizedBinaryImage, resizedBinaryImage, new OpenCvSharp.Size(600, 600));
            //Cv2.Resize(resizedBinaryTemplate, resizedBinaryTemplate, new OpenCvSharp.Size(600, 600));
            //Cv2.ImShow("Binary Image (Input)", resizedBinaryImage);
            //Cv2.ImShow("Binary Template (Template)", resizedBinaryTemplate);

            //// 템플릿 매칭
            //Mat result = new Mat();
            //Cv2.MatchTemplate(image, templateImage, result, TemplateMatchModes.CCoeffNormed);

            //// 결과 이진화
            //Cv2.Threshold(result, result, 0.8, 1.0, ThresholdTypes.Binary);

            //Mat t0 = result.Clone();
            //Cv2.Resize(t0, t0, new OpenCvSharp.Size(1200, 1200));
            //Cv2.ImShow("t0", t0);
            //// 결과 정규화 (0~255 범위로)
            //Mat normResult = new Mat();
            //Cv2.Normalize(result, normResult, 0, 255, NormTypes.MinMax, MatType.CV_8U);

            //Mat t1 = normResult.Clone();
            //Cv2.Resize(t1, t1, new OpenCvSharp.Size(1200, 1200));
            //Cv2.ImShow("t1", t1);
            //// 결과 복사 (최종 이미지)
            //Mat finalImage = image.Clone();

            //// 윤곽선 탐색
            //OpenCvSharp.Point[][] contours;
            //HierarchyIndex[] hierarchy;
            //Cv2.FindContours(normResult, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            //Mat t2 = finalImage.Clone();
            //Cv2.Resize(t2, t2, new OpenCvSharp.Size(1200, 1200));
            //Cv2.DrawContours(t2, contours, -1, new Scalar(0, 0, 255), 5, LineTypes.Link8, null, 1);
            //Cv2.ImShow("t2", t2);

            //List<List<OpenCvSharp.Point>> rows = new List<List<OpenCvSharp.Point>>();
            //// 윤곽선을 순회하며 매칭된 영역에 사각형 그리기
            //foreach (var contour in contours)
            //{
            //    double contourArea = Cv2.ContourArea(contour);
            //    if (contourArea > 100) // 작은 영역만 처리
            //        continue;

            //    // 윤곽선의 첫 번째 점 가져오기
            //    OpenCvSharp.Point pt1 = contour[0];
            //    OpenCvSharp.Point pt2 = new OpenCvSharp.Point(pt1.X + templateImage.Width, pt1.Y + templateImage.Height);

            //    // 사각형 그리기
            //    Cv2.Rectangle(finalImage, pt1, pt2, Scalar.Red, 5);
            //    rows = draw(pt1, finalImage);
            //    break;
            //}
            //HersheyFonts font = HersheyFonts.HersheySimplex;
            //int count = 1;
            //for (int i = 0; i < rows.Count; i++)
            //{
            //    if (i % 2 == 0)
            //    {
            //        for (int j = 0; j < rows[i].Count; j++)
            //        {
            //            // 번호 출력
            //            Cv2.PutText(finalImage, $"{count}", rows[i][j], font, 3, new Scalar(255, 0, 0), 3);
            //            Cv2.Circle(finalImage, rows[i][j], 2, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);

            //            count++;

            //            // 선 그리기 (다음 점이 있는 경우에만)
            //            if (j < rows[i].Count - 1)
            //            {
            //                Cv2.Line(finalImage, rows[i][j], rows[i][j + 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
            //            }
            //        }

            //        // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
            //        if (i < rows.Count - 1)
            //        {
            //            Cv2.Line(finalImage, rows[i][rows[i].Count - 1], rows[i + 1][rows[i + 1].Count - 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
            //        }
            //    }
            //    else
            //    {
            //        for (int j = 0; j < rows[i].Count; j++)
            //        {
            //            int reverse_j = rows[i].Count - j - 1;
            //            // 번호 출력
            //            Cv2.PutText(finalImage, $"{count}", rows[i][reverse_j], font, 3, new Scalar(255, 0, 0), 3);
            //            Cv2.Circle(finalImage, rows[i][reverse_j], 2, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);
            //            count++;

            //            // 선 그리기 (다음 점이 있는 경우에만)
            //            if (j < rows[i].Count - 1)
            //            {
            //                Cv2.Line(finalImage, rows[i][reverse_j], rows[i][reverse_j - 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
            //            }
            //        }

            //        // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
            //        if (i < rows.Count - 1)
            //        {
            //            Cv2.Line(finalImage, rows[i][0], rows[i + 1][0], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
            //        }
            //    }
            //}
            //// 결과 이미지 출력
            //Cv2.Resize(finalImage, finalImage, new OpenCvSharp.Size(1200, 1200));
            //Cv2.ImShow("Final Image", finalImage);
            //Cv2.WaitKey(0);
            //Cv2.DestroyAllWindows();
        }

        public void test()
        {
            //OpenFileDialog dig = new OpenFileDialog();
            //dig.ShowDialog();
            //Mat src = Cv2.ImRead(dig.FileName);
            //Mat baseImage = new Mat();
            //Cv2.CvtColor(src, baseImage, ColorConversionCodes.BGR2GRAY);

            //OpenFileDialog dig2 = new OpenFileDialog();
            //dig2.ShowDialog();
            //Mat input = Cv2.ImRead(dig2.FileName);
            //Mat inputgray = new Mat();
            //Cv2.CvtColor(input, inputgray, ColorConversionCodes.BGR2GRAY);
            //Cv2.Threshold(inputgray, inputgray, 204, 255, ThresholdTypes.Binary);

            //var orb = ORB.Create();
            //var keypointsBase = orb.Detect(baseImage);
            //var desBase = new Mat();
            //orb.Compute(baseImage, ref keypointsBase, desBase);

            //var keypointsInput = orb.Detect(inputgray);
            //var desInput = new Mat();
            //orb.Compute(inputgray, ref keypointsInput, desInput);

            //// 2. 매칭 수행 (Hamming 거리 사용, cross-check 활성화)
            //var bfMatcher = new BFMatcher(NormTypes.Hamming, crossCheck: true);
            //var matches = bfMatcher.Match(desBase, desInput);

            //// 3. 매칭 결과를 정렬 (매칭 품질 순서)
            //var sortedMatches = matches.OrderBy(m => m.Distance).ToArray();

            //// 4. 결과 시각화
            //Mat resultImage = new Mat();
            //Cv2.DrawMatches(baseImage, keypointsBase, inputgray, keypointsInput, sortedMatches, resultImage);
            //// 매칭된 점에 원 그리기
            //foreach (var match in sortedMatches)
            //{
            //    // 매칭된 특징점의 위치 가져오기 (Point2f -> Point 변환)
            //    var pointBase = new OpenCvSharp.Point((int)keypointsBase[match.QueryIdx].Pt.X, (int)keypointsBase[match.QueryIdx].Pt.Y);
            //    var pointInput = new OpenCvSharp.Point(
            //        (int)(keypointsInput[match.TrainIdx].Pt.X + baseImage.Cols), // 오프셋 적용
            //        (int)keypointsInput[match.TrainIdx].Pt.Y
            //    );

            //    // 원 그리기 (색상: 초록색, 반지름: 5)
            //    Cv2.Circle(resultImage, pointBase, 20, Scalar.Green, thickness: 2);
            //    Cv2.Circle(resultImage, pointInput, 20, Scalar.Green, thickness: 2);
            //}


            //// 결과 이미지 출력
            //Cv2.Resize(resultImage, resultImage, new OpenCvSharp.Size(2000, 800));
            //Cv2.ImShow("Matched Image", resultImage);
            //Cv2.WaitKey(0);
            //Cv2.DestroyAllWindows();

            //Mat baseImage = new Mat();
            //Cv2.CvtColor(src, baseImage, ColorConversionCodes.BGR2GRAY);

            OpenFileDialog dig = new OpenFileDialog();
            dig.ShowDialog();
            Mat src = Cv2.ImRead(dig.FileName);


            OpenFileDialog dig2 = new OpenFileDialog();
            dig2.ShowDialog();
            Mat input = Cv2.ImRead(dig2.FileName);

            //Mat inputgray = new Mat();
            //Cv2.CvtColor(input, inputgray, ColorConversionCodes.BGR2GRAY);
            //Cv2.Threshold(inputgray, inputgray, 204, 255, ThresholdTypes.Binary);
            //ComparePoint c = new ComparePoint(src, input);
            //ComparePoint c = new ComparePoint();
            
        }
        private List<List<OpenCvSharp.Point>> draw(OpenCvSharp.Point p1, Mat image, double angle)
        {
            int squareSize = 45; // 정사각형 한 변의 길이
            int spacing = 116;   // 정사각형 중심 간 거리
            int numSquares = 11; // 11x11 개의 정사각형
            OpenCvSharp.Point centerPoint = new OpenCvSharp.Point(image.Width / 2, image.Height / 2);
            OpenCvSharp.Point[,] centers = new OpenCvSharp.Point[numSquares, numSquares];
            for (int i = 0; i < numSquares; i++) // 행 루프
            {
                for (int j = 0; j < numSquares; j++) // 열 루프
                {
                    // 정사각형의 중심을 기준으로 좌상단 좌표 계산
                    int startX = p1.X + j * spacing;
                    int startY = p1.Y + i * spacing;

                    // 정사각형의 좌상단과 우하단 좌표
                    OpenCvSharp.Point topLeft = new OpenCvSharp.Point(startX, startY);
                    OpenCvSharp.Point bottomRight = new OpenCvSharp.Point(startX + squareSize, startY + squareSize);

                    // 중심점 계산
                    int centerX = (topLeft.X + bottomRight.X) / 2;
                    int centerY = (topLeft.Y + bottomRight.Y) / 2;
                    OpenCvSharp.Point center = new OpenCvSharp.Point(centerX, centerY);

                    double radians = angle * Math.PI / 180.0;
                    // 회전
                    OpenCvSharp.Point rotatedTopLeft = RotatePoint(topLeft, centerPoint, radians);
                    OpenCvSharp.Point rotatedBottomRight = RotatePoint(bottomRight, centerPoint, radians);
                    OpenCvSharp.Point rotatedCenter = RotatePoint(center, centerPoint, radians);

                    // 2차원 배열에 중심점 저장
                    centers[i, j] = rotatedCenter;
                    // 정사각형 그리기
                    Cv2.Rectangle(image, rotatedTopLeft, rotatedBottomRight, new Scalar(255, 0, 0), 2);
                }
            }
            List<List<OpenCvSharp.Point>> rows = new List<List<OpenCvSharp.Point>>();
            for (int i = 0; i < numSquares; i++)
            {
                if (rows.Count <= i)
                {
                    rows.Add(new List<OpenCvSharp.Point>());
                }
                for (int j = 0; j < numSquares; j++)
                {
                    switch (i)
                    {
                        case 0:
                        case 10:
                            if (j >= 3 && j <= 7)
                                rows[i].Add(centers[i, j]);
                            break;
                        case 1:
                        case 9:
                            if (j >= 2 && j <= 8)
                                rows[i].Add(centers[i, j]);
                            break;
                        case 2:
                        case 8:
                            if (j >= 1 && j <= 9)
                                rows[i].Add(centers[i, j]);
                            break;
                        default:
                            rows[i].Add(centers[i, j]);
                            break;
                    }
                }
            }



            // 결과 시각화
            //Cv2.ImShow("Grid of Squares", image);
            return rows;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //이미지 및 템플릿 로드
            OpenFileDialog dig1 = new OpenFileDialog();
            OpenFileDialog dig2 = new OpenFileDialog();

            dig1.ShowDialog();
            dig2.ShowDialog();
            Mat image = Cv2.ImRead(dig1.FileName, ImreadModes.Color);
            Mat templateImage = Cv2.ImRead(dig2.FileName, ImreadModes.Color);
            //Cv2.Rectangle(image, new OpenCvSharp.Point(100, 100), new OpenCvSharp.Point(150, 150), Scalar.Black, -1); // -1: 채우기
            //Cv2.ImShow("image", image);
            //Cv2.WaitKey(0);
            // 색 공간 변환 (BGR -> RGB)    

            // 이미지를 흑백으로 변환 (그레이스케일)
            Mat grayImage = new Mat();
            Mat grayTemplate = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
            Cv2.CvtColor(templateImage, grayTemplate, ColorConversionCodes.BGR2GRAY);

            // 오츠 알고리즘 적용 이진화
            Mat binaryImage = new Mat();
            Mat binaryTemplate = new Mat();
            //double otsuThreshImage = Cv2.Threshold(grayImage, binaryImage, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
            Cv2.Threshold(grayImage, binaryImage, 204, 255, ThresholdTypes.Binary);
            //double otsuThreshTemplate = Cv2.Threshold(grayTemplate, binaryTemplate, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
            Cv2.Threshold(grayTemplate, binaryTemplate, 204, 255, ThresholdTypes.Binary);

            // 이진화된 이미지 테스트용으로 표시
            Mat resizedBinaryImage = binaryImage.Clone();
            Mat resizedBinaryTemplate = binaryTemplate.Clone();
            Cv2.Resize(resizedBinaryImage, resizedBinaryImage, new OpenCvSharp.Size(600, 600));
            Cv2.Resize(resizedBinaryTemplate, resizedBinaryTemplate, new OpenCvSharp.Size(600, 600));
            Cv2.ImShow("Binary Image (Input)", resizedBinaryImage);
            Cv2.ImShow("Binary Template (Template)", resizedBinaryTemplate);

            double bestMatchValue = -1.0;
            int bestAngle = 0;
            int bestX = 0, bestY = 0;

            for (int angle = -30; angle <= 30; angle += 2)
            {
                Mat rotatedtemplate = RotateImage(binaryTemplate, angle);
                Mat result = new Mat();
                Cv2.MatchTemplate(binaryImage, rotatedtemplate, result, TemplateMatchModes.CCoeffNormed);

                double minVal, maxVal;
                OpenCvSharp.Point minLoc, maxLoc;
                Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc);

                if (maxVal > bestMatchValue)
                {
                    bestMatchValue = maxVal;
                    bestAngle = angle;
                    bestX = maxLoc.X;
                    bestY = maxLoc.Y;
                }
            }
            // 템플릿 매칭
            Mat rotatedtemplate2 = RotateImage(binaryTemplate, bestAngle);
            Mat result2 = new Mat();
            Cv2.MatchTemplate(binaryImage, rotatedtemplate2, result2, TemplateMatchModes.CCoeffNormed);

            // 결과 이진화
            Cv2.Threshold(result2, result2, 0.6, 1.0, ThresholdTypes.Binary);

            Mat t0 = result2.Clone();
            Cv2.Resize(t0, t0, new OpenCvSharp.Size(1200, 1200));
            Cv2.ImShow("t0", t0);
            // 결과 정규화 (0~255 범위로)
            Mat normResult = new Mat();
            Cv2.Normalize(result2, normResult, 0, 255, NormTypes.MinMax, MatType.CV_8U);

            Mat t1 = normResult.Clone();
            Cv2.Resize(t1, t1, new OpenCvSharp.Size(1200, 1200));
            Cv2.ImShow("t1", t1);
            // 결과 복사 (최종 이미지)
            Mat finalImage = image.Clone();

            // 윤곽선 탐색
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(normResult, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            Mat t2 = finalImage.Clone();
            Cv2.Resize(t2, t2, new OpenCvSharp.Size(1200, 1200));
            Cv2.DrawContours(t2, contours, -1, new Scalar(0, 0, 255), 5, LineTypes.Link8, null, 1);
            Cv2.ImShow("t2", t2);

            bestAngle *= -1;

            List<List<OpenCvSharp.Point>> rows = new List<List<OpenCvSharp.Point>>();
            // 윤곽선을 순회하며 매칭된 영역에 사각형 그리기
            foreach (var contour in contours)
            {
                double contourArea = Cv2.ContourArea(contour);
                if (contourArea > 100) // 작은 영역만 처리
                    continue;

                // 윤곽선의 첫 번째 점 가져오기
                OpenCvSharp.Point pt1 = contour[0];
                OpenCvSharp.Point pt2 = new OpenCvSharp.Point(pt1.X + templateImage.Width, pt1.Y + templateImage.Height);

                finalImage = Rotate_cal(image, pt1, pt2, bestAngle);
                //Cv2.Resize(t3, t3, new OpenCvSharp.Size(1200, 1200));
                //Cv2.ImShow("t3", t3);
                // 사각형 그리기
                //Cv2.Rectangle(finalImage, pt1, pt2, Scalar.Red, 5);
                rows = draw(pt1, finalImage, (double)bestAngle);
                break;
            }
            HersheyFonts font = HersheyFonts.HersheySimplex;
            int count = 1;
            for (int i = 0; i < rows.Count; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < rows[i].Count; j++)
                    {
                        // 번호 출력
                        Cv2.PutText(finalImage, $"{count}", rows[i][j], font, 3, new Scalar(255, 0, 0), 3);
                        Cv2.Circle(finalImage, rows[i][j], 2, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);

                        count++;

                        // 선 그리기 (다음 점이 있는 경우에만)
                        if (j < rows[i].Count - 1)
                        {
                            Cv2.Line(finalImage, rows[i][j], rows[i][j + 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                        }
                    }

                    // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
                    if (i < rows.Count - 1)
                    {
                        Cv2.Line(finalImage, rows[i][rows[i].Count - 1], rows[i + 1][rows[i + 1].Count - 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                    }
                }
                else
                {
                    for (int j = 0; j < rows[i].Count; j++)
                    {
                        int reverse_j = rows[i].Count - j - 1;
                        // 번호 출력
                        Cv2.PutText(finalImage, $"{count}", rows[i][reverse_j], font, 3, new Scalar(255, 0, 0), 3);
                        Cv2.Circle(finalImage, rows[i][reverse_j], 2, new Scalar(0, 255, 0), -1, LineTypes.AntiAlias);
                        count++;

                        // 선 그리기 (다음 점이 있는 경우에만)
                        if (j < rows[i].Count - 1)
                        {
                            Cv2.Line(finalImage, rows[i][reverse_j], rows[i][reverse_j - 1], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                        }
                    }

                    // 다음 행이 있는 경우, 행 끝과 다음 행의 시작을 연결
                    if (i < rows.Count - 1)
                    {
                        Cv2.Line(finalImage, rows[i][0], rows[i + 1][0], new Scalar(255, 0, 0), 10, LineTypes.AntiAlias);
                    }
                }
            }
            // 결과 이미지 출력
            Cv2.Resize(finalImage, finalImage, new OpenCvSharp.Size(1200, 1200));
            Cv2.ImShow("Final Image", finalImage);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
        public Mat RotateImage(Mat src, int angle)
        {
            OpenCvSharp.Point center = new OpenCvSharp.Point(src.Width / 2, src.Height / 2);
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, (double)angle, 1.0);
            Mat rotatedImage = new Mat();
            Cv2.WarpAffine(src, rotatedImage, rotationMatrix, src.Size());
            return rotatedImage;
        }

        public Mat Rotate_cal(Mat src, OpenCvSharp.Point p1, OpenCvSharp.Point p2, int angle)
        {
            var rotatedPoints = RotateRectangle(p1, p2, angle);
            Mat mat = src.Clone();
            Cv2.Polylines(mat, new OpenCvSharp.Point[][] { rotatedPoints }, true, Scalar.Red, 5);
            return mat;
        }
        public OpenCvSharp.Point[] RotateRectangle(OpenCvSharp.Point pt1, OpenCvSharp.Point pt2, double angle)
        {
            // 사각형의 중심점 계산
            OpenCvSharp.Point center = new OpenCvSharp.Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);

            // 회전 행렬 계산 (각도는 라디안으로 변환)
            double radians = angle * Math.PI / 180.0;
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, 1.0);

            // 회전된 좌표들을 저장할 배열
            OpenCvSharp.Point[] rectPoints = new OpenCvSharp.Point[4];

            // 사각형의 4개 점을 회전시킴
            rectPoints[0] = RotatePoint(pt1, center, radians);
            rectPoints[1] = RotatePoint(new OpenCvSharp.Point(pt1.X, pt2.Y), center, radians);
            rectPoints[2] = RotatePoint(pt2, center, radians);
            rectPoints[3] = RotatePoint(new OpenCvSharp.Point(pt2.X, pt1.Y), center, radians);

            return rectPoints;
        }

        public OpenCvSharp.Point RotatePoint(OpenCvSharp.Point point, OpenCvSharp.Point center, double radians)
        {
            // 회전 공식: 새로운 x = (x - cx) * cos(θ) - (y - cy) * sin(θ) + cx
            // 새로운 y = (x - cx) * sin(θ) + (y - cy) * cos(θ) + cy
            int x = (int)(Math.Cos(radians) * (point.X - center.X) - Math.Sin(radians) * (point.Y - center.Y) + center.X);
            int y = (int)(Math.Sin(radians) * (point.X - center.X) + Math.Cos(radians) * (point.Y - center.Y) + center.Y);

            return new OpenCvSharp.Point(x, y);
        }

        private void btn_baseImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            dig.ShowDialog();
            Mat baseimg = Cv2.ImRead(dig.FileName);

            comparePoint = new ComparePoint(baseimg);
        }

        private void btn_inputImg_Click(object sender, EventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            dig.ShowDialog();
            Mat inputimg = Cv2.ImRead(dig.FileName);

            if (comparePoint != null)
            {
                comparePoint.CompareImg(inputimg);
            }
            else
                MessageBox.Show("need base img");
        }

        private void btn_inputImg_R_Click(object sender, EventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();
            dig.ShowDialog();
            Mat inputimg = Cv2.ImRead(dig.FileName);

            if (comparePoint != null)
            {
                comparePoint.CompareImg_R(inputimg);
            }
            else
                MessageBox.Show("need base img");
        }
    }



    static class Constants
    {
        public const int AREA_LOW = 1000;
        public const int AREA_HIGH = 1600;
        public const int ROUND_LOW = 160;
        public const int ROUND_HIGH = 200;
    }

    static class Constants_btn2
    {
        public const int AREA_LOW = 2400;
        public const int AREA_HIGH = 3500;
        public const int ROUND_LOW = 200;
        public const int ROUND_HIGH = 230;
    }
}
