using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using Accord.Math;
using Accord.Video.FFMPEG;
using ConsoleVideoPlayer.Native;

namespace ConsoleVideoPlayer.CVPCore
{
    public class CVP : IDisposable
    {
        public bool IsEnabled { get; private set; }

        public bool IsFinished
        {
            get
            {
                if (currentFrame >= totalFrames)
                {
                    IsEnabled = false;
                    return true;
                }

                return false;
            }
        }

        private Size consoleFontSize;

        private long     currentFrame = 0;
        private long     totalFrames;
        private Rational fr;
        private int      frRnd;

        private bool  shouldRescale = false;
        private float scale;

        private long ticksPerFrame;
        // private double deltaTime;

        private Size  vidSize;
        private Size  consoleSize;
        private Point vidPos;

        private ConcurrentQueue<Bitmap> preloadedBitmaps  = new ConcurrentQueue<Bitmap>();
        private long                    preloadedFrameCnt = 0;
        private bool                    isPreloadFinished = false;

        private Thread playThread;
        private Thread workThread;

        private readonly Graphics        g;
        private readonly VideoFileReader vReader;

        public CVP(string path, float scale = 1)
        {
            vReader = new VideoFileReader();
            vReader.Open(path);

            consoleFontSize = Util.GetConsoleFontSize();
            totalFrames = vReader.FrameCount;

            this.scale = scale;
            if (Math.Abs(scale - 1) > 1e-6)
                shouldRescale = true;
            if (shouldRescale)
                vidSize = new Size((int) (vReader.Width * scale), (int) (vReader.Height * scale));
            else
                vidSize = new Size(vReader.Width, vReader.Height);
            consoleSize = vidSize + new Size(consoleFontSize.Width * 2 - 1, consoleFontSize.Height * 4);

            Util.FixConsoleWindowSize();
            Util.AdjustConsoleWindowSize(consoleSize);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            vidPos = new Point(0, consoleFontSize.Height);
            fr = vReader.FrameRate;
            frRnd = (int) Math.Round(fr.Value);
            ticksPerFrame = (long) (1e7 / fr.Value);
            playThread = new Thread(InternalPlay);
            workThread = new Thread(PreloadFrames);

            g = Graphics.FromHwnd(WinApi.GetConsoleWindow());
        }

        private void InternalPlay()
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.SetCursorPosition(totalFrames.ToString().Length + 1, 0);
            // Console.Write($" / {totalFrames}, fr: {(int) fr}");
            var currentTime = DateTime.Now.Ticks;
            var startTime = DateTime.Now.Ticks;
            while (IsEnabled && !IsFinished)
            {
                PlayFrame();
                long delta;

                Console.SetCursorPosition(0, 0);
                Console.Write(
                    $"{currentFrame}, {preloadedFrameCnt} / {totalFrames}, fr: avg={currentFrame * 1e7 / (DateTime.Now.Ticks - startTime)}, real={fr.Value}");
                // ticksPerFrame refresh duration 
                while ((delta = DateTime.Now.Ticks - currentTime) < ticksPerFrame)
                    ;
                // deltaTime = delta / 1e7;
                currentTime = DateTime.Now.Ticks;
            }
        }

        private void PreloadFrames()
        {
            while (IsEnabled && !isPreloadFinished)
            {
                while (preloadedFrameCnt - currentFrame > frRnd * 2) Thread.Sleep(500);

                while (!(isPreloadFinished = preloadedFrameCnt >= totalFrames) &&
                       preloadedFrameCnt - currentFrame < frRnd * 2)
                {
                    preloadedFrameCnt++;
                    var bm = vReader.ReadVideoFrame();
                    if (shouldRescale)
                        bm = new Bitmap(bm, vidSize);
                    preloadedBitmaps.Enqueue(bm);
                }
            }
        }

        private void PlayFrame()
        {
            while (IsEnabled && !IsFinished && preloadedBitmaps.Count == 0) ;
            currentFrame++;
            // Console.SetCursorPosition(0, 0);
            // Console.Write($"{currentFrame}/ {totalFrames}, fr: {(int)fr}");

            // var currentBitmap = vReader.ReadVideoFrame();
            preloadedBitmaps.TryDequeue(out var a);
            g.DrawImage(a, vidPos);
            if (currentFrame % frRnd == 0) GC.Collect();
        }

        public void StartPlay(bool isAsync)
        {
            IsEnabled = true;
            workThread.Start();
            Thread.Sleep(shouldRescale ? 1000 : 100);
            if (isAsync)
                playThread.Start();
            else
                InternalPlay();
        }

        public void Dispose()
        {
            vReader?.Close();
            g?.Dispose();
        }
    }
}