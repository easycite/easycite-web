using System.Reflection;
using Autofac;
using Microsoft.Extensions.Hosting;

namespace EasyCiteLib
{
    public class ContainerConfiguration
    {
        readonly IHostEnvironment _environment;
        readonly ContainerBuilder _builder;
        
        public ContainerConfiguration(IHostEnvironment environment, ContainerBuilder builder)
        {
            _environment = environment;
            _builder = builder;
        }

        public void RegisterAssembly(Assembly assembly)
        {
            if (_environment.IsDevelopment())
            {
                _builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.StartsWith("Mock")
                        && t.IsInterface == false
                        && t.Name.EndsWith("Processor"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }

            _builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.StartsWith("Mock") == false
                    && t.IsInterface == false
                    && t.Name.EndsWith("Processor"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
