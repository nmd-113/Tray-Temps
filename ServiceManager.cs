using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace TrayTemps
{
    public static class ServiceManager
    {
        public static bool StopService(string serviceName, int timeoutSeconds = 5)
        {
            try
            {
                using (var service = new ServiceController(serviceName))
                {
                    if (service.Status == ServiceControllerStatus.Stopped)
                        return true;

                    if (service.Status == ServiceControllerStatus.StopPending)
                    {
                        service.WaitForStatus(
                            ServiceControllerStatus.Stopped,
                            TimeSpan.FromSeconds(timeoutSeconds));

                        return service.Status == ServiceControllerStatus.Stopped;
                    }

                    service.Stop();

                    service.WaitForStatus(
                        ServiceControllerStatus.Stopped,
                        TimeSpan.FromSeconds(timeoutSeconds));

                    return service.Status == ServiceControllerStatus.Stopped;
                }
            }
            catch
            {
                return false;
            }
        }

        public static Task<bool> StopServiceAsync(string serviceName, int timeoutSeconds = 5)
        {
            return Task.Run(() => StopService(serviceName, timeoutSeconds));
        }
    }
}