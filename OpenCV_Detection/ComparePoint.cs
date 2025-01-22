using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace OpenCV_Detection
{
    internal class ComparePoint
    {
        const bool BASE_IMAGE = true;
        const bool INPUT_IMAGE = false;
        List<List<OpenCvSharp.Point>> base_rows = new List<List<OpenCvSharp.Point>>();
        List<List<OpenCvSharp.Point>> input_rows = new List<List<OpenCvSharp.Point>>();
        Mat baseimg;
        Mat rotatedImage;
        Mat inputimg;
        Point2d shift;
        double baseimg_m;
        double inputimg_m;
        public ComparePoint()
        {
            base_rows.Clear();
            input_rows.Clear();
            baseimg = new Mat();
            rotatedImage = new Mat();
            inputimg = new Mat();
            //RotateImage(30);
        }
        public ComparePoint(Mat baseimg)
        {
            base_rows.Clear();
            input_rows.Clear();
            this.baseimg = baseimg;
            Detecting(baseimg, BASE_IMAGE);
        }
        public void CompareImg(Mat inputimg)
        {
            //base_rows.Clear();
            //input_rows.Clear();
            this.inputimg = inputimg;
            Process();
        }
        public void CompareImg_R(Mat inputimg)
        {
            this.inputimg = inputimg;
            Process_R();
        }
        public void Process()
        {
            Detecting(inputimg, INPUT_IMAGE);
            RotateImage_by_m();
            Cal_Shift();
            Cal_NewRows(true);
        }
        public void Process_R()
        {
            Detecting(inputimg, INPUT_IMAGE);
            RotateImage_by_m();
            Cal_Shift();
            Cal_NewRows(false);
        }
        public void Detecting(Mat img, bool isbase)
        {
            Mat inputGray = new Mat();
            Cv2.CvtColor(img, inputGray, ColorConversionCodes.BGR2GRAY);

            // 이진화
            Mat thresholded = new Mat();
            Cv2.Threshold(inputGray, thresholded, 104, 255, ThresholdTypes.Binary);

            var parameters = new SimpleBlobDetector.Params
            {
                MinThreshold = 10,
                MaxThreshold = 240,
                ThresholdStep = 5,
                FilterByArea = true,
                MinArea = 200,
                FilterByColor = true, // 컬러 필터 활성화
                BlobColor = 0         // 검정색(0)을 찾도록 설정
            };

            // 나머지 필터 비활성화
            parameters.FilterByConvexity = false;
            parameters.FilterByInertia = false;
            parameters.FilterByCircularity = false;

            // Blob 검출기 생성
            var detector = SimpleBlobDetector.Create(parameters);

            // 키포인트 검출
            var keypoints = detector.Detect(thresholded);

            // 'keypoints'에서 좌표를 추출하여 'center' 리스트에 추가
            List<OpenCvSharp.Point> center = new List<OpenCvSharp.Point>();

            // KeyPoint의 Pt (Point2f)에서 X, Y 값을 추출하여 OpenCvSharp.Point로 변환
            foreach (var kp in keypoints)
            {
                center.Add(new OpenCvSharp.Point((int)kp.Pt.X, (int)kp.Pt.Y));
            }
            if (isbase)
                base_rows = Check_Slope(center, BASE_IMAGE);
            else
                input_rows = Check_Slope(center, INPUT_IMAGE);


            // 키포인트 그리기
            Mat imgWithKeypoints = new Mat();
            Cv2.DrawKeypoints(img, keypoints, imgWithKeypoints, Scalar.Red, DrawMatchesFlags.DrawRichKeypoints);

            //결과 출력
            Cv2.Resize(imgWithKeypoints, imgWithKeypoints, new OpenCvSharp.Size(600, 600));
            if (isbase)
                Cv2.ImShow("isbase", imgWithKeypoints);
            else
                Cv2.ImShow("input", imgWithKeypoints);

            //Cv2.WaitKey(0);
            //Cv2.DestroyAllWindows();
        }
        private void RotateImage_by_m()
        {
            double angle1 = Math.Atan(baseimg_m) * (180 / Math.PI);  // m1에 해당하는 각도
            double angle2 = Math.Atan(inputimg_m) * (180 / Math.PI);  // m2에 해당하는 각도

            // 회전 각도는 angle1- angle2
            double rotationAngle = angle1 - angle2;

            // 이미지 읽기
            Mat image = baseimg;

            // 이미지의 중심 계산
            Point2f center = new Point2f(image.Cols / 2, image.Rows / 2);

            // 회전 행렬 계산
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, rotationAngle, 1.0);

            // 회전된 이미지 저장할 크기 (회전 후 이미지 크기 조정)
            Size newSize = new Size(image.Cols, image.Rows);

            // 회전된 이미지 생성 (회전 후 남은 부분은 하얀색으로 채움)
            rotatedImage = new Mat();
            Cv2.WarpAffine(image, rotatedImage, rotationMatrix, newSize, InterpolationFlags.Linear, BorderTypes.Constant, new Scalar(255, 255, 255));

        }
        private void Cal_Shift()
        {
            Mat rotated_img = rotatedImage.Clone();
            Cv2.CvtColor(rotated_img, rotated_img, ColorConversionCodes.BGR2GRAY);
            rotated_img.ConvertTo(rotated_img, MatType.CV_32F);

            Mat input_img = inputimg.Clone();
            Cv2.CvtColor(input_img, input_img, ColorConversionCodes.BGR2GRAY);
            input_img.ConvertTo(input_img, MatType.CV_32F);

            shift = CalculateShift(rotated_img, input_img);
        }

        private void Cal_NewRows(bool R) 
        {
            //회전각도 double angle이 있고, x,y이동할 Point2d shift가 있을때, 
            //List<List<OpenCvSharp.Point>> base_rows = new List<List<OpenCvSharp.Point>>(); 여기의 좌표를 중앙을 중심으로 angle만큼 회전하고, x,y만큼 이동해서, rect그리기
            // 중심점 계산 (이미지 중심 기준으로 회전)
            Point2f center = new Point2f(baseimg.Width / 2f, baseimg.Height / 2f);
            double angle1 = Math.Atan(baseimg_m) * (180 / Math.PI);  // m1에 해당하는 각도
            double angle2 = Math.Atan(inputimg_m) * (180 / Math.PI);  // m2에 해당하는 각도

            // 회전 각도는 angle1- angle2
            double angle = angle1 - angle2;
            // 회전 변환 행렬 생성
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, 1.0);

            // 변환된 좌표를 저장할 리스트
            List<List<Point>> transformedPoints = new List<List<Point>>();

            foreach (var row in base_rows)
            {
                List<Point> newRow = new List<Point>();
                foreach (var point in row)
                {
                    Point2f originalPoint = new Point2f(point.X, point.Y);
                    Point2f[] rotatedPoint = new Point2f[1];

                    using (Mat pointMat = new Mat(1, 1, MatType.CV_32FC2))
                    {
                        pointMat.Set(0, 0, originalPoint);
                        using (Mat rotatedMat = new Mat())
                        {
                            Cv2.Transform(pointMat, rotatedMat, rotationMatrix);
                            rotatedPoint[0] = rotatedMat.At<Point2f>(0, 0);
                        }
                    }
                    // 이동 적용
                    Point transformedPoint = new Point(
                        (int)(rotatedPoint[0].X - shift.X),
                        (int)(rotatedPoint[0].Y - shift.Y)
                    );

                    newRow.Add(transformedPoint);
                }
                transformedPoints.Add(newRow);
            }
            if(R)
                Draw_lines(transformedPoints);
            else
                Draw_lines_R(transformedPoints);
        }

        public void Draw_lines(List<List<Point>> transformedPoints)
        {
            for (int i = 0; i < transformedPoints.Count; i++)
            {
                if (i % 2 != 0) // i가 홀수일 때
                {
                    transformedPoints[i].Reverse(); // 리스트를 뒤집음
                }
            }

            int counter = 1; // 숫자 카운터 초기화

            foreach (var transformedRow in transformedPoints)
            {
                foreach (var point in transformedRow)
                {
                    // 파란색 원 그리기
                    Cv2.Circle(inputimg, point, 10, Scalar.Blue, -1);

                    // 숫자 그리기
                    Cv2.PutText(
                        inputimg,
                        counter.ToString(),
                        new Point(point.X + 10, point.Y), // 약간 오른쪽으로 이동시켜 텍스트가 원에 겹치지 않게 조정
                        HersheyFonts.HersheySimplex,
                        2.5, // 폰트 크기
                        Scalar.Red,
                        3 // 두께
                    );

                    counter++; // 숫자 증가
                }
            }


            // 결과 이미지 표시
            Cv2.Resize(inputimg, inputimg, new OpenCvSharp.Size(600, 600));
            Cv2.ImShow("Transformed Rectangles", inputimg);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
        public void Draw_lines_R(List<List<Point>> transformedPoints)
        {
            int center = transformedPoints.Count / 2;
            int SelectedX = center;
            int SelectedY = center;
            int counter = 1;
            HashSet<(int,int)> visited = new HashSet<(int, int)>();
            Point point = transformedPoints[SelectedX][SelectedY];
            int emptyConut = 2; // 임의. n*n크기 니들에서 (n-count[0])/2
            int[] xCorrCoeff = new int[transformedPoints.Count];
            for (int i = 0; i < xCorrCoeff.Length; i++)
            {
                if (i < emptyConut)
                {
                    xCorrCoeff[i] = i - emptyConut;
                    continue;
                }
                if (i >= xCorrCoeff.Length - emptyConut)
                {
                    xCorrCoeff[i] = xCorrCoeff.Length - 1 - i - emptyConut;
                    continue;
                }
            }
            Cv2.Circle(inputimg, point, 10, Scalar.Blue, -1);

            // 숫자 그리기
            Cv2.PutText(
            inputimg,
                counter.ToString(),
                new Point(point.X + 10, point.Y), // 약간 오른쪽으로 이동시켜 텍스트가 원에 겹치지 않게 조정
                HersheyFonts.HersheySimplex,
                2.5, // 폰트 크기
                Scalar.Red,
                3 // 두께
            );
            counter++; // 숫자 증가
            visited.Add((SelectedX, SelectedY));

            //왼쪽, 위쪽, 오른쪽, 아래쪽
            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, -1, 0, 1 };

            int d = 0;

            int total = transformedPoints.Sum(row => row.Count);
            int repeats = 1;
            int checkRepeat = 0;
            while (visited.Count <= total)
            {
                for (int repeat = 0; repeat < repeats; repeat++)
                {
                    SelectedX += dx[d];
                    SelectedY += dy[d];

                    if (xCorrCoeff.Length <= SelectedY)
                        continue;

                    int xCorrValue = SelectedX + xCorrCoeff[SelectedY];

                    if (xCorrValue < 0 || SelectedY < 0 || transformedPoints[SelectedY].Count <= xCorrValue)
                        continue;
                    if (visited.Contains((xCorrValue, SelectedY)))
                        continue;

                    Point SelectedPoint = transformedPoints[SelectedY][xCorrValue];
                    Cv2.Circle(inputimg, SelectedPoint, 10, Scalar.Blue, -1);

                    // 숫자 그리기
                    Cv2.PutText(
                    inputimg,
                        counter.ToString(),
                        new Point(SelectedPoint.X + 10, SelectedPoint.Y), // 약간 오른쪽으로 이동시켜 텍스트가 원에 겹치지 않게 조정
                        HersheyFonts.HersheySimplex,
                        2.5, // 폰트 크기
                        Scalar.Red,
                        3 // 두께
                    );
                    counter++; // 숫자 증가
                    visited.Add((xCorrValue, SelectedY));
                    if (visited.Count >= total) break;
                }
                if (visited.Count >= total) break;
                checkRepeat++;
                d = (d + 1) % 4;
                if (checkRepeat % 2 == 0)
                    repeats++;
            }
            // 결과 이미지 표시
            Cv2.Resize(inputimg, inputimg, new OpenCvSharp.Size(600, 600));
            Cv2.ImShow("Transformed Rectangles", inputimg);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }



        //public void Draw_lines_R(List<List<Point>> transformedPoints)
        //{
        //    // 중앙점 찾기
        //    int rows = transformedPoints.Count;
        //    int midRow = rows / 2; // 중앙 행의 인덱스
        //    int midCol = transformedPoints[midRow].Count / 2; // 중앙 열의 인덱스
        //    Point currentPoint = transformedPoints[midRow][midCol]; // 시작점

        //    // 방문 여부를 확인하기 위한 HashSet
        //    HashSet<(int, int)> visited = new HashSet<(int, int)>();

        //    // 방향 벡터: 오른쪽, 아래, 왼쪽, 위
        //    int[] dx = { 1, 0, -1, 0 };
        //    int[] dy = { 0, 1, 0, -1 };

        //    // 숫자 카운터 초기화
        //    int counter = 1;

        //    // OpenCV 초기 그리기
        //    Cv2.Circle(inputimg, currentPoint, 10, Scalar.Blue, -1); // 파란색 원
        //    Cv2.PutText(inputimg, counter.ToString(), new Point(currentPoint.X + 10, currentPoint.Y), HersheyFonts.HersheySimplex, 2.5, Scalar.Red, 3); // 숫자
        //    counter++;
        //    visited.Add((midRow, midCol)); // 방문 표시

        //    // 나선형 탐색 시작
        //    int direction = 0; // 현재 방향 (0: 오른쪽, 1: 아래, 2: 왼쪽, 3: 위)
        //    int steps = 1; // 현재 진행할 거리 (같은 거리 2번 수행 후 증가)
        //    int change = 0; // 방향 변경 횟수
        //    int currentX = midCol;
        //    int currentY = midRow;

        //    // 전체 점의 개수
        //    int totalPoints = transformedPoints.Sum(row => row.Count);

        //    while (visited.Count < totalPoints) // 모든 점 방문 시 종료
        //    {

        //        for (int step = 0; step < steps; step++)
        //        {
        //            if (counter == 30)
        //                counter = 30;
        //            // 다음 점 계산
        //            currentX += dx[direction];
        //            currentY += dy[direction];

        //            // 유효한 범위인지 확인
        //            if (currentY < 0 || currentY >= rows || currentX < 0 || currentX >= transformedPoints[currentY].Count)
        //                continue;

        //            // 이미 방문한 점인지 확인
        //            if (visited.Contains((currentY, currentX)))
        //                continue;

        //            // 점 선택
        //            Point nextPoint = transformedPoints[currentY][currentX];

        //            // OpenCV 그리기
        //            Cv2.Circle(inputimg, nextPoint, 10, Scalar.Blue, -1);
        //            Cv2.PutText(inputimg, counter.ToString(), new Point(nextPoint.X + 10, nextPoint.Y), HersheyFonts.HersheySimplex, 2.5, Scalar.Red, 3);
        //            counter++;
        //            visited.Add((currentY, currentX)); // 방문 기록

        //            // 중단 조건: 모든 점 방문 시 종료
        //            if (visited.Count >= totalPoints)
        //                break;
        //        }

        //        // 방향 변경
        //        direction = (direction + 1) % 4;
        //        change++;

        //        // 방향 2번 변경 후 이동 거리 증가
        //        if (change % 2 == 0) steps++;
        //    }

        //    // 결과 이미지 표시
        //    Cv2.Resize(inputimg, inputimg, new OpenCvSharp.Size(600, 600));
        //    Cv2.ImShow("Transformed Rectangles", inputimg);
        //    Cv2.WaitKey(0);
        //    Cv2.DestroyAllWindows();
        //}


        public List<List<OpenCvSharp.Point>> Check_Slope(List<OpenCvSharp.Point> center, bool is_base)
        {
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
            // 3-2. y값이 근사한 점을 찾아, dx/dy로 기울기를 구함
            if (Math.Abs(Y_max.Y - close1.Y) < Math.Abs(Y_max.Y - close2.Y))
            {
                if (Y_max.X != close1.X)  // 분모가 0인지 확인
                    m = (double)(Y_max.Y - close1.Y) / (double)(Y_max.X - close1.X);
                else
                    m = 0;  // 분모가 0이면 기울기를 0으로 설정
            }
            else
            {
                if (Y_max.X != close2.X)  // 분모가 0인지 확인
                    m = (double)(Y_max.Y - close2.Y) / (double)(Y_max.X - close2.X);
                else
                    m = 0;  // 분모가 0이면 기울기를 0으로 설정
            }

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
            //기울기 체크
            if (is_base)
            {
                baseimg_m = m;
            }
            else
            {
                inputimg_m = m;
            }

            return rows;
        }
        public void RotateImage(double angle)
        {
            OpenFileDialog dig = new OpenFileDialog();
            dig.ShowDialog();
            Mat image = Cv2.ImRead(dig.FileName);
            // 이미지 읽기

            // 이미지의 중심 계산
            Point2f center = new Point2f(image.Cols / 2, image.Rows / 2);

            // 회전 행렬 계산 (angle은 회전 각도)
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, 1.0);

            // 회전된 이미지를 저장할 크기 (회전 후 이미지 크기 조정)
            Size newSize = new Size(image.Cols, image.Rows);

            // 회전된 이미지 생성 (회전 후 남은 부분은 하얀색으로 채움)
            Mat rotatedImage = new Mat();
            Cv2.WarpAffine(image, rotatedImage, rotationMatrix, newSize, InterpolationFlags.Linear, BorderTypes.Constant, new Scalar(255, 255, 255));

            // 결과 이미지 저장
            Cv2.ImWrite("rotated_image.bmp", rotatedImage);

            // 결과 이미지 창으로 보기
            Cv2.ImShow("Rotated Image", rotatedImage);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        static Point2d CalculateShift(Mat img1, Mat img2)
        {
            // FFT 변환
            Mat dft1 = new Mat(), dft2 = new Mat();
            Cv2.Dft(img1, dft1, DftFlags.ComplexOutput);
            Cv2.Dft(img2, dft2, DftFlags.ComplexOutput);

            // Cross-power spectrum 계산
            Mat crossPowerSpectrum = new Mat();
            Cv2.MulSpectrums(dft1, dft2, crossPowerSpectrum, DftFlags.None, true);
            Cv2.Normalize(crossPowerSpectrum, crossPowerSpectrum);

            // 역 DFT 변환으로 상관관계 계산
            Mat crossCorrelation = new Mat();
            Cv2.Idft(crossPowerSpectrum, crossCorrelation);

            // 복소수 부분 분리 (실수 및 허수)
            Mat[] planes = crossCorrelation.Split();
            Mat realPart = planes[0]; // 실수 부분
            Mat imaginaryPart = planes[1]; // 허수 부분

            // 실수 부분의 절댓값 계산
            Mat magnitude = new Mat();
            Cv2.Magnitude(realPart, imaginaryPart, magnitude);

            // 최대값 위치 탐색
            Point maxLoc;
            Cv2.MinMaxLoc(magnitude, out _, out _, out _, out maxLoc);

            // 이동량 계산
            double shiftX = maxLoc.X <= img1.Cols / 2 ? maxLoc.X : maxLoc.X - img1.Cols;
            double shiftY = maxLoc.Y <= img1.Rows / 2 ? maxLoc.Y : maxLoc.Y - img1.Rows;

            return new Point2d(shiftX, shiftY);
        }

        public void test(int dx, int dy)
        {
            // 파일 선택 창 열기
            OpenFileDialog dig = new OpenFileDialog();
            dig.ShowDialog();
            Mat originalImage = Cv2.ImRead(dig.FileName);

            // 이미지 크기 및 이동 거리 설정
            dx = 100;  // X축 이동 거리
            dy = 250;   // Y축 이동 거리
            int rows = originalImage.Rows;
            int cols = originalImage.Cols;

            // 변환 행렬을 float 배열로 생성
            float[,] translationArray = {
            { 1, 0, dx }, // X축 이동
            { 0, 1, dy }  // Y축 이동
        };

            // 변환 행렬을 Mat로 변환
            Mat translationMatrix = Mat.FromArray(translationArray);

            // WarpAffine으로 이동 (남은 영역을 흰색으로 채우기)
            Mat shiftedImage = new Mat();
            Cv2.WarpAffine(
                originalImage,
                shiftedImage,
                translationMatrix,
                new Size(cols, rows), // 원본 크기 유지
                InterpolationFlags.Linear,
                BorderTypes.Constant,
                new Scalar(255, 255, 255) // 흰색으로 남은 영역 채우기
            );

            // 결과 이미지 저장 및 출력
            Cv2.ImWrite("shifted_image.bmp", shiftedImage);
            Cv2.ImShow("Original Image", originalImage);
            Cv2.ImShow("Shifted Image (White Border)", shiftedImage);

            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();

            Console.WriteLine("이미지가 성공적으로 이동되고 저장되었습니다.");
        }


    }
}
