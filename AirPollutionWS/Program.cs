using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AirPollutionWS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

#if DEBUG

            ActionForDebug();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AirPollutionService1()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }


        static void ActionForDebug()
        {
            try
            {
                AirPollutionMain AP = new AirPollutionMain();
                AP.MainActivity();
                ServiceSchedule serviceSchedule = new ServiceSchedule();
                serviceSchedule.ScheduleService();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }
    }
}
