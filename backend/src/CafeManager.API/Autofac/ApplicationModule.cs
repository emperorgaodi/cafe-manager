using Autofac;
using CafeManager.Domain.Repositories;
using CafeManager.Infrastructure.Repositories;

namespace CafeManager.API.Autofac;

public class ApplicationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<CafeRepository>().As<ICafeRepository>().InstancePerLifetimeScope();
        builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>().InstancePerLifetimeScope();
    }
}