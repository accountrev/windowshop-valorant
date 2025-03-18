using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windowshop
{
    internal class ItemViewerManager
    {
        public static void Refresh()
        {
            
        }

        public static void Reset()
        {
            WindowshopGlobals.levelSelected = 0;
            WindowshopGlobals.chromaSelected = 0;
        }
    }
}
