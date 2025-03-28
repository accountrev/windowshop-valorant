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
