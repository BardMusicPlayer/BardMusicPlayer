using System;

namespace BardMusicPlayer.Grunt.ApiTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Grunt.Instance.Start();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
