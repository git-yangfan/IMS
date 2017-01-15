using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IMS.Web
{
    public partial class Startup
    {
        public static  void Register(ContainerBuilder builder)
        {
            List<Assembly> asmRepositoryImp = GetAllAssembly("IMS.Data.dll");
            builder.RegisterAssemblyTypes(asmRepositoryImp.ToArray()).Where(t => t.Name.EndsWith("Repository")).AsSelf().AsImplementedInterfaces().InstancePerHttpRequest();
        }


        #region 加载程序集
        private static List<Assembly> GetAllAssembly(string dllName)
        {
            List<string> pluginpath = FindPlugin(dllName);
            var list = new List<Assembly>();
            foreach (string filename in pluginpath)
            {
                try
                {
                    string asmname = Path.GetFileNameWithoutExtension(filename);
                    if (asmname != string.Empty)
                    {
                        Assembly asm = Assembly.LoadFrom(filename);
                        list.Add(asm);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return list;
        }
        //查找所有插件的路径
        private static List<string> FindPlugin(string dllName)
        {
            List<string> pluginpath = new List<string>();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            string dir = Path.Combine(path, "bin\\");
            string[] dllList = Directory.GetFiles(dir, dllName);
            if (dllList.Length > 0)
            {
                pluginpath.AddRange(dllList.Select(item => Path.Combine(dir, item.Substring(dir.Length))));
            }
            return pluginpath;
        }
        #endregion
    }
}
