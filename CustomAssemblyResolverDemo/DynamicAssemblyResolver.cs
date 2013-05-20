using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;
using WebApiCodeGenLib;

namespace CustomAssemblyResolverDemo
{
    public class DynamicAssemblyResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);

            // Add our controller library assembly
            try
            {
                WebApiGenerator gen = new WebApiGenerator("DynamicWebApiController");
                Assembly onTheFly = gen.CreateDll();
                if (onTheFly != null)
                {
                    assemblies.Add(onTheFly);
                }
            }
            catch
            {
                // We ignore errors and just continue
            }

            return assemblies;
        }
    }
}
