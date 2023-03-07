using System;
using System.Diagnostics;

namespace Bloons_Mod_Manager.Lib.Web
{
    public static class WebHelper
    {
        public static void OpenURL(string url)
        {
            // try first method
            try
            {
                Process.Start(url);

                return;
            }
            catch (Exception) { }


            // try second if first fails
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);

                return;
            }
            catch (Exception) { }


            // try third if second fails
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"/c start {url}"
                };
                Process.Start(psi);

                return;
            }
            catch (Exception) { }


            // show error message if ALL failed.
            Logger.Log($"Failed to open url!\nURL: {url}");
        }
    }
}
