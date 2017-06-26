using DotLiquid.Tests;
using NUnitLite;
using System.Reflection;

namespace Dotliquid.Console {
    public class Program {
        static void Main(string[] args) {
            new AutoRun(typeof(BlockTests).GetTypeInfo().Assembly).Execute(args);
        }
    }
}
