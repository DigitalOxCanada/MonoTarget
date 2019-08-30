using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTarget
{
    /*
 Layers from back to front
 1 - Background Layer 1
 2 - Background Layer 2
 3 - Objects Layer 1
 4 - Objects Layer 2
 5 - Objects Layer 3
 6 - Effects Layer 1
 7 - Effects Layer 2
 8 - HUD Layer 1
 9 - HUD Layer 2
 10 - Menu Layer 1
 */
    public static class VisualLayerID
    {
        public const int BG = 0;
        public const int BG1 = 1;
        public const int Objects = 2;
        public const int HUD = 7;
        public const int Menu = 9;
    }

    //public enum VisualLayerID : int
    //{
    //    BG = 0,
    //    Objects = 2,
    //    HUD = 7,
    //    Menu = 9
    //}
}
