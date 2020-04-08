using System;
using System.Reflection;
using Autofac;
using EasyCiteLib.Configuration;
using EasyCiteLib.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Neo4jClient;
using Module = Autofac.Module;

namespace EasyCiteLib
{
    public class LibraryModule : Module
    {
        readonly IHostEnvironment _environment;

        public LibraryModule(IHostEnvironment environment)
        {
            _environment = environment;
        }

        protected override void Load(ContainerBuilder builder)
        {
            Assembly assembly = typeof(LibraryModule).Assembly;

            if (_environment.IsDevelopment())
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.StartsWith("Mock")
                        && !t.IsInterface
                        && t.Name.EndsWith("Processor"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
                
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.StartsWith("Mock")
                        && !t.IsInterface
                        && t.Name.EndsWith("Manager"))
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => !t.Name.StartsWith("Mock")
                    && !t.IsInterface
                    && t.Name.EndsWith("Processor"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => !t.Name.StartsWith("Mock")
                    && !t.IsInterface
                    && t.Name.EndsWith("Manager"))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(context =>
                {
                    var neo4jOptions = context.Resolve<IOptionsSnapshot<Neo4jOptions>>();

                    return NeoServerConfiguration.GetConfiguration(new Uri(neo4jOptions.Value.Uri), neo4jOptions.Value.UserName, neo4jOptions.Value.Password);
                })
                .SingleInstance();

            builder.RegisterType<GraphClientFactory>()
                .As<IGraphClientFactory>()
                .SingleInstance();

            builder.RegisterType<ProjectReferencesContext>().SingleInstance();
            builder.RegisterType<DocumentContext>().SingleInstance();
        }
    }
}