using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIKUArcade.Entities;
using DIKUArcade.Math;
using DIKUArcade.Graphics;
using System.IO;
using DIKUArcade;

namespace SpaceTaxi_1
{
    internal class Program
    {
        public static void Main(string[] args)
        {    
            var game = new Game();
            game.GameLoop();
        }
    }
}
