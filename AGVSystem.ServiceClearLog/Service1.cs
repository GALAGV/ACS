using AGVSystem.APP.LogClearService;
using System.ServiceProcess;

namespace AGVSystem.ServiceClearLog
{
    public partial class Service1 : ServiceBase
    {
       private  LogServe serve = new LogServe();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            serve.LogOnStart();
        }

        protected override void OnStop()
        {
            serve.LogOnStop();
        }
    }
}
