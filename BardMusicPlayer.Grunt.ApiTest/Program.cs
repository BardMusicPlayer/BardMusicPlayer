/*
 * Copyright(c) 2023 MoogleTroupe
 * Licensed under the GPL v3 license. See https://github.com/BardMusicPlayer/BardMusicPlayerApi/blob/develop/LICENSE for full license information.
 */

using System;
using System.Threading.Tasks;
using BardMusicPlayer.Quotidian.Structs;
using BardMusicPlayer.Pigeonhole;
using BardMusicPlayer.Seer;
using BardMusicPlayer.Seer.Events;

namespace BardMusicPlayer.Grunt.ApiTest
{
    internal class Program
    {
        private static void Main()
        {
            BmpPigeonhole.Initialize(AppContext.BaseDirectory + @"\Grunt.ApiTest.json");

            BmpSeer.Instance.GameStarted += SendADoot;

            BmpSeer.Instance.SetupFirewall("BardMusicPlayer.Grunt.ApiTest");

            Console.WriteLine("Hit enter to start Grunt");

            Console.ReadLine();

            while (true)
            {
                BmpSeer.Instance.Start();

                BmpGrunt.Instance.Start();

                Console.ReadLine();

                BmpGrunt.Instance.Stop();

                BmpSeer.Instance.Stop();

                Console.WriteLine("Grunt stopped. Hit enter to start it again.");

                Console.ReadLine();
            }
        }

        private static void SendADoot(GameStarted seerEvent)
        {
            var game = seerEvent.Game;

            Console.WriteLine("Detected game pid " + game.Pid + ", sleep a thread for 3000ms to allow Seer to parse the dat files.");

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                Console.WriteLine("Trying to doot on game pid " + game.Pid + ".");
                if (game != null && !await game.EquipInstrument(Instrument.Harp)) Console.WriteLine("Failed to tell game pid " + game.Pid + " to equip a harp :(");
                else
                {
                    if (game != null && !await game.SendNoteOn(60, 0)) Console.WriteLine("Failed to tell game pid " + game.Pid + " to push down on a note :(");
                    else
                    {
                        await Task.Delay(25);
                        if (game != null && !await game.SendNoteOff(60)) Console.WriteLine("Failed to tell game pid " + game.Pid + " to release a note :(");
                        else
                        {
                            await Task.Delay(3000);
                            if (game != null && !await game.EquipInstrument(Instrument.None)) Console.WriteLine("Failed to tell game pid " + game.Pid + " to de-equip a harp :(");

                            Console.WriteLine("Trying to sing on game pid " + seerEvent.Pid + ".");
                            if (game != null && !await game.SendLyricLine("/echo " + game.PlayerName + " is singing" + (game.IsDalamudHooked() ? " with Dalamud!" : " with copy paste!"))) Console.WriteLine("Failed to tell game pid " + game.Pid + " to sing a line :(");
                        }
                    }
                }
            });
        }
    }
}
