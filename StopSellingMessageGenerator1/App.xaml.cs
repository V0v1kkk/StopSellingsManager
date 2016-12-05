using System.IO;
using System.Windows;
using Autofac;
using MugenMvvmToolkit;
using MugenMvvmToolkit.WPF.Infrastructure;
using StopSellingMessageGenerator.AdditionalClasses;
using StopSellingMessageGenerator.Interfaces;
// ReSharper disable InconsistentNaming

namespace StopSellingMessageGenerator
{
    public partial class App : Application
    {
        public App()
        {
            //For stable work of application, DI-conteiner will be able to resolve these interfaces:
            //IDataSource, ITtInformationSource, IStopSellingsBulder, IMessageTextGenerator
            //IReportGenerator, IWorkFolderOwnerChecker

            var pathToWorkFolder = StopSellingMessageGenerator.Properties.Settings.Default.PathToWorkFolder;

            var builder = new ContainerBuilder();

            var DALInstance = DAL.GetDAL(pathToWorkFolder);

            builder.Register(c => DALInstance).As<IDataSource>().SingleInstance();
            builder.Register(c => DALInstance).As<ITtInformationSource>().SingleInstance();
            builder.Register(c =>
            {
                var informationService = c.Resolve<ITtInformationSource>();
                var stopSellingsBulder = new StopSellingsBulder(informationService);
                return stopSellingsBulder;
            }).As<IStopSellingsBulder>().InstancePerDependency();
            builder.Register(c => new MessageTextGeneratorReflection(pathToWorkFolder)).As<IMessageTextGenerator>();
            builder.Register(c => new ReportGenerator(pathToWorkFolder)).As<IReportGenerator>();
            builder.Register(c => new WorkFolderOwnerChecker(pathToWorkFolder)).As<IWorkFolderOwnerChecker>();

            // ReSharper disable once ObjectCreationAsStatement
            new Bootstrapper<Starter>(this, new AutofacContainer(builder));
        }
    }
}