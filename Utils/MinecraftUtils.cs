using System;
using System.Text.RegularExpressions;

namespace siscode_bot.Utils {
    public static class MinecraftUtils {
        public static Regex Coords3Regex = new Regex(@"((-\d|\d)\d* ){3}|((-\d|\d)\d* ){2}(-\d|\d)\d*");
        public static Regex Coords2Regex = new Regex(@"((-\d|\d)\d* ){2}|((-\d|\d)\d* )(-\d|\d)\d*");

        public class Coordinate {
            public int X { get; internal set; }
            public int Y { get; internal set; }
            public int Z { get; internal set; }

            public Coordinate(int x, int y, int z) {
                X = x;
                Y = y;
                Z = z;
            }

            public Coordinate ConvertToNether() => new Coordinate((int)Math.Floor((double)X/8),Y,(int) Math.Floor((double)Z/8));
            public Coordinate ConvertToOverworld() => new Coordinate((int)Math.Floor((double)X*8),Y,(int) Math.Floor((double)Z*8));
            public override string ToString() => $"X:{X},Y:{Y},Z:{Z}";
        }
    }
}