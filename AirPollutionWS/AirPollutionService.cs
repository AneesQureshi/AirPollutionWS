using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AirPollutionWS
{
    public partial class AirPollutionService1 : ServiceBase
    {
        public AirPollutionService1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
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

        protected override void OnStop()
        {

            try
            {
                ServiceSchedule serviceSchedule = new ServiceSchedule();
                serviceSchedule.Schedular.Dispose();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }


        }
    }
}
