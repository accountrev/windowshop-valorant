using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windowshop.Utility;

namespace Windowshop.Helpers
{
    class AuthUpdater
    {
        private int authTimer;

        public async Task Start()
        {
            while (true)
            {
                await Task.Delay(30 * 60000);

                await WindowshopUtil.RefreshToken();
            }
        }
    }
}
