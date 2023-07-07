using Autofac;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Autofac.Extras.DynamicProxy;
using Application.Services;
using Application.Interface;
using DataAccess.Repositories;
using Entities.Concrete;
using DataAccess.Interfaces;
using AutoMapper;

namespace Application.DependencyResolvers.Autofac
{
    public class AutofacBusinessModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProductManager>().As<IProductServices>().SingleInstance();
            builder.RegisterType<ProductRepository>().As<IProductDal>().SingleInstance();
            builder.RegisterType<StockManager>().As<IStockServices>().SingleInstance();
            builder.RegisterType<StockRepository>().As<IStockDal>().SingleInstance();
            builder.RegisterType<ColorManager>().As<IColorServices>().SingleInstance();
            builder.RegisterType<ColorRepository>().As<IColorDal>().SingleInstance();
            builder.RegisterType<TempRepository>().As<ITempDal>().SingleInstance();
            builder.RegisterType<TempManager>().As<ITempServices>().SingleInstance();
            builder.RegisterType<ShippingRepository>().As<IShippingDal>().SingleInstance();
            builder.RegisterType<ShippingManager>().As<IShippingServices>().SingleInstance();
          
            builder.RegisterType<ShippingDetailsRepository>().As<IShippingDetails>().SingleInstance();
            builder.RegisterType<ShippingDetailsManager>().As<IShippingDetailsServices>().SingleInstance();   
            
            builder.RegisterType<OrderRepository>().As<IOrderDal>().SingleInstance();    
            builder.RegisterType<OrderManager>().As<IOrderServices>().SingleInstance();

            builder.RegisterType<TenantRepository>().As<ITenantDal>().SingleInstance();
            builder.RegisterType<TenantManager>().As<ITenantServices>().SingleInstance();
            //builder.RegisterType<UserManager<AppUser>>().As<UserManager<AppUser>>().SingleInstance();
            //builder.RegisterType<SignInManager<AppUser>>().As<SignInManager<AppUser>>().SingleInstance();
             builder.RegisterType<AppUserManager>().As<IAppUserServices>().SingleInstance();
             builder.RegisterType<AppUserRepository>().As<IAppUserDal>().SingleInstance();

            builder.RegisterType<UserRoleManager>().As<IUserRolServices>().SingleInstance();
            builder.RegisterType<AppRoleRepository>().As<IAppRoleDal>().SingleInstance();


            builder.RegisterType<OrderDateRepository>().As<IOrderDateDal>().SingleInstance();
            builder.RegisterType<OrderDateManager>().As<IOrderDateServices>().SingleInstance();


            builder.RegisterType<TeklilerManager>().As<ITeklilerServices>().SingleInstance();
            builder.RegisterType<TeklilerRepository>().As<ITekliler>().SingleInstance();



            builder.RegisterType<MagazaStockRepository>().As<IMagazaStock>().SingleInstance();
            builder.RegisterType<ProductAgeRepository>().As<IProductAgeDal>().SingleInstance();    
            
          

            builder.RegisterType<TenantRepository>().As<ITenantDal>().SingleInstance();
            //builder.RegisterType<ShippingDetailsManager>().As<IShippingDetailsServices>().SingleInstance();
            //builder.RegisterType<Mapper>().As<IMapper>().SingleInstance();

            //builder.RegisterType<EfProductDal>().As<IProductDal>().SingleInstance();    

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    //bütün sınıflar için yukarıdaki aspect var mı kontol ediyor
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();
        }
    }
}
