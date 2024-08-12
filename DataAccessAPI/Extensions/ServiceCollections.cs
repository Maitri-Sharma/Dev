using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using System.Reflection;
using MediatR;
using Puma.Infrastructure.Interface;
using Puma.Infrastructure.Repository;
using Puma.Infrastructure.Interface.KsupDB;
using Puma.Infrastructure.Repository.KspuDB;
using Puma.Infrastructure.Interface.KsupDB.Utvalg;
using Puma.Infrastructure.Repository.KspuDB.Utvalg;
using Puma.Infrastructure.Interface.KsupDB.Reol;
using Puma.Infrastructure.Repository.KspuDB.Reol;
using DataAccessAPI.Hangfire;
using Puma.Infrastructure.Interface.MemoryCache;
using Puma.Infrastructure.Repository.MemoryCache;
using Puma.Infrastructure.Interface.KsupDB.OEBSService;
using Puma.Infrastructure.Repository.KspuDB.OEBSService;
using Puma.Infrastructure.Interface.KsupDB.RestCapacity;
using Puma.Infrastructure.Repository.KspuDB.RestCapacity;
using Puma.Infrastructure.Interface.Logger;
using Puma.Infrastructure.Repository.Logger;

namespace DataAccessAPI.Extensions
{
    /// <summary>
    /// Class to for service collection
    /// </summary>
    public static class ServiceCollections
    {
        /// <summary>
        /// Configure service collections
        /// </summary>
        /// <param name="_serviceCollections"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppMVC(this IServiceCollection _serviceCollections)
        {
            //Register fluent validation
            _serviceCollections.AddMvc().AddFluentValidation(fv =>
           { fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()); }).
            AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //Register mediatoR
            _serviceCollections.AddMediatR(typeof(Startup));

            return _serviceCollections;
        }

        /// <summary>
        /// Add all dependency need to register
        /// </summary>
        /// <param name="_serviceCollections"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterDependencies(this IServiceCollection _serviceCollections)
        {
            //_serviceCollections.AddTransient(typeof(IKsupDBGenericRepository<>), typeof(KsupDBGenericRepository<>));
            _serviceCollections.AddTransient(typeof(IKspUDBUnitOfWork), typeof(KspUDBUnitOfWork));
            _serviceCollections.AddTransient(typeof(IAddressPointStateRepository), typeof(AddressPointStateRepository));
            _serviceCollections.AddTransient(typeof(IUtvalgRepository), typeof(UtvalgRepository));
            _serviceCollections.AddTransient(typeof(IUtvalgListRepository), typeof(UtvalgListRepository));
            _serviceCollections.AddTransient(typeof(IReolRepository), typeof(ReolRepository));
            _serviceCollections.AddSingleton(typeof(IConfigurationRepository), typeof(ConfigurationRepository));
            _serviceCollections.AddTransient(typeof(IGjenskapUtvalgRepository), typeof(GjenskapUtvalgRepository));
            _serviceCollections.AddTransient(typeof(IReolGeneratorRepository), typeof(ReolGeneratorRepository));
            _serviceCollections.AddTransient(typeof(IArbeidsListStateRepository), typeof(ArbeidsListStateRepository));
            _serviceCollections.AddTransient(typeof(IAvisDekningRepository), typeof(AvisDekningRepository));
            _serviceCollections.AddTransient(typeof(IByDelRepository), typeof(ByDelRepository));
            _serviceCollections.AddTransient(typeof(IFylkeRepository), typeof(FylkeRepository));
            _serviceCollections.AddTransient(typeof(IGetPrsCalendarAdminDetailsRepository), typeof(GetPrsCalendarAdminDetailsRepository));
            _serviceCollections.AddTransient(typeof(IKapasitetRepository), typeof(KapasitetRepository));
            _serviceCollections.AddTransient(typeof(IKommuneRepository), typeof(KommuneRepository));
            _serviceCollections.AddTransient(typeof(IRecreateRepository), typeof(RecreateRepository));
            _serviceCollections.AddTransient(typeof(IReolerKommuneRepository), typeof(ReolerKommuneRepository));
            _serviceCollections.AddTransient(typeof(ITeamRepository), typeof(TeamRepository));
            _serviceCollections.AddTransient(typeof(IPostsoneReolMapperRepository), typeof(PostsoneReolMapperRepository));
            _serviceCollections.AddTransient(typeof(IHangfireJob), typeof(HangfireJob));
            _serviceCollections.AddTransient(typeof(IManageCache), typeof(ManageCache));
            _serviceCollections.AddTransient(typeof(IOEBSServiceRepository), typeof(OEBSServiceRepository));
            _serviceCollections.AddTransient(typeof(IPointCheckRepository), typeof(PointCheckRepository));
            //_serviceCollections.AddTransient(typeof(IUtvalgListModificationRepository), typeof(UtvalgListModificationRepository));
            _serviceCollections.AddTransient(typeof(IRouteImportRepository), typeof(RouteImportRepository));
            // _serviceCollections.AddTransient(typeof(ISelectionDistributionRepository), typeof(SelectionDistributionRepository));
            _serviceCollections.AddTransient(typeof(IPumaRestCapacityRepository), typeof(PumaRestCapacityRepository));
            _serviceCollections.AddTransient(typeof(IMapOperationRepository), typeof(MapOperationRepository));
            _serviceCollections.AddTransient(typeof(IRestCapacityRepository), typeof(RestCapacityRepository));
            _serviceCollections.AddTransient(typeof(ILoggerRepository), typeof(LoggerRepository));

            //IMapOperationRepository
            return _serviceCollections;
        }
    }
}
