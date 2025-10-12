using System;
using Autofac;
using MvvmDialogs;
using TIMS_X.CloudAssistant.ViewModels;
using TIMS_X.CloudDataServices;
using TIMS_X.Core;
using TIMS_X.Core.Services;
using TIMS_X.CoreServices;

namespace TIMS_X.CloudAssistant
{
	public class Locator
	{
		private IContainer _container;
		private readonly ContainerBuilder _containerBuilder;

		public Locator()
		{
			_containerBuilder = new ContainerBuilder();

			// Register core services
			_containerBuilder.RegisterType<LogService>().As<ILogService>().SingleInstance();
			_containerBuilder.RegisterType<RequestService>().As<IRequestService>();
			_containerBuilder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();

			_containerBuilder.RegisterType<SchedulerService>().As<ISchedulerService>();
			_containerBuilder.RegisterType<ProviderService>().As<IProviderService>();
			_containerBuilder.RegisterType<UserService>().As<IUserService>().SingleInstance();
			_containerBuilder.RegisterType<PracticeService>().As<IPracticeService>();
			_containerBuilder.RegisterType<PatientService>().As<IPatientService>();
			_containerBuilder.RegisterType<CustomerService>().As<ICustomerService>();
			_containerBuilder.RegisterType<ContextHelper>().As<ContextHelper>().SingleInstance();

			// Register view models
			_containerBuilder.RegisterType<ManageNoahViewModel>();
			_containerBuilder.RegisterType<ChangeServerViewModel>();
			_containerBuilder.RegisterType<LoginViewModel>();
			_containerBuilder.RegisterType<MainViewModel>();
			_containerBuilder.RegisterType<PatientViewModel>();
			_containerBuilder.RegisterType<DialogService>().As<IDialogService>();
			_containerBuilder.RegisterType<NoahManager>();
		}

		public static Locator Instance { get; } = new Locator();

		public void Build()
		{
			_container = _containerBuilder.Build();
		}

		public void Register<TInterface, TImplementation>()
			where TImplementation : TInterface
		{
			_containerBuilder.RegisterType<TImplementation>().As<TInterface>();
		}

		public T Resolve<T>()
		{
			return _container.Resolve<T>();
		}

		public object Resolve(Type type)
		{
			return _container.Resolve(type);
		}
	}
}