using System;
using System.Drawing;
using System.Threading;
using ConsoleVideoPlayer.CVPCore;
using ConsoleVideoPlayer.Native;

namespace ConsoleVideoPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                args = new[] {Console.ReadLine(), Console.ReadLine()};
            }
            else if (args.Length == 1)
            {
                args = new[] {args[0], Console.ReadLine()};
            }

            Console.Clear();
            var player = new CVP(args[0], float.Parse(args[1]));
            player.StartPlay(false);
            player.Dispose();
        }
    }
}