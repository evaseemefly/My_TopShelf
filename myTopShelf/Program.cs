using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Topshelf;

namespace myTopShelf
{
    /// <summary>
    /// 继承自ServiceControl的模式
    /// </summary>
    public class MyServiceByServiceControl:ServiceControl
    {
        private Timer _timer = null;
        readonly ILog _log = LogManager.GetLogger(typeof(MyServiceByServiceControl));

        public MyServiceByServiceControl()
        {
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => _log.Info(DateTime.Now);
        }

        public bool Start(HostControl hostControl)
        {
            _log.Info("TopShelf用例已经启动");
            _timer.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _log.Info("TopShelf用例已经终止");
            _timer.Stop();
            return true;
        }
    }

    public class TownCrier
    {
        readonly Timer _timer=null;
        readonly ILog _log = LogManager.GetLogger(typeof(TownCrier));

        public TownCrier()
        {
            _timer = new Timer(1000) { AutoReset = true };
            //_timer.Elapsed += (sender, eventArgs) => Console.WriteLine("It is {0} and all is well", DateTime.Now);
            //改为向日志文件中写入
            _timer.Elapsed += (sender, eventArgs) => _log.Info(DateTime.Now);
        }
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }
    }

   
    class Program
    {
        static void Main(string[] args)
        {
            //方式一：常用模式
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
            HostFactory.Run(hc =>
            {
                hc.Service<TownCrier>(s =>
                {
                    s.ConstructUsing(name => new TownCrier());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                hc.RunAsLocalSystem();

                //服务的描述
                hc.SetDescription("使用topshelf创建的本地服务");
                //显示的服务名称
                hc.SetDisplayName("stuff");
                //在服务控制管理器中注册的名字
                hc.SetServiceName("stuff");
            });

            //方式二：
            //var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log4net.config");
            //XmlConfigurator.ConfigureAndWatch(logCfg);

            //HostFactory.Run(x =>
            //{
            //    x.Service<MyServiceByServiceControl>();
            //    x.RunAsLocalSystem();

            //    //服务的描述
            //    x.SetDescription("使用topshelf创建的本地服务");
            //    //显示的服务名称
            //    x.SetDisplayName("stuffByServiceControl");
            //    //在服务控制管理器中注册的名字
            //    x.SetServiceName("stuffByServiceControl");
            //});
        }
    }
}
