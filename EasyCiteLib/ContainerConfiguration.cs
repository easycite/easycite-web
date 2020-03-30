using System.Reflection;
using Autofac;
using EasyCiteLib.Repository;
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
            _builder.RegisterType<ProjectReferencesContext>().SingleInstance();

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
