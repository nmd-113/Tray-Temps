using System;
using System.ServiceProcess;
using System.Threading.Tasks;

public static class ServiceManager
{
    public static async Task<bool> StopServiceAsync(string serviceName, int timeoutSeconds = 30)
    {
        try
        {
            await Task.Run(() =>
            {
                using (var service = new ServiceController(serviceName))
                {
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        return;
                    }
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(timeoutSeconds));
                }
            });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}