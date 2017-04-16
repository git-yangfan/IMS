using AutoMapper;
using IMS.Data.DAL;
using IMS.Model.Entity;
using IMS.Web.Dto;


namespace IMS.Web.Config
{
    /// <summary>
    /// AutoMapper 配置
    /// </summary>
    public partial class AutoMapperConfiguration
    {
        /// <summary>
        /// 配置AutoMapper
        /// </summary>
        public static void Config()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<RepairApplication, ApplicationDto>();
                cfg.CreateMap<RepairApplication, ApplicationDto>()
                    .ForMember(u => u.FullLocation, e => e.MapFrom(s => s.FirstLocation + (s.SecondLocation == null ? "" : "/" + s.SecondLocation) + (s.ThirdLocation == null ? "" : "/" + s.ThirdLocation)))
                    .ForMember(u => u.DeviceShortName, e => e.MapFrom(s => RepairDAL.DeviceShortNameAndNoDic[s.DeviceNo]));
                cfg.CreateMap<ApplicationDto, RepairApplication>();

                cfg.CreateMap<Device, DeviceDto>();
                cfg.CreateMap<DeviceDto, Device>();

                cfg.CreateMap<SelfRepairPlan, SelfRepairPlanDto>();
                cfg.CreateMap<SelfRepairPlanDto, SelfRepairPlan>();

                cfg.CreateMap<SubSystem, SubSystemDto>();
                cfg.CreateMap<SubSystemDto, SubSystem>();

                cfg.CreateMap<DispatchDto, Dispatch>();
                cfg.CreateMap<Dispatch, DispatchDto>();

                cfg.CreateMap<DispatchDto, DiagnoseDispatch>();
                
                cfg.CreateMap<Users, EngineerDto>()
                    .ForMember(u => u.EngineerName, e => e.MapFrom(s => s.Name))
                    .ForMember(u => u.TeamName, e => e.MapFrom(s => s.BanZu))
                    .ForMember(u => u.EngineerId, e => e.MapFrom(s => s.Id));

                cfg.CreateMap<EmployeeDto, Employee>();
                cfg.CreateMap<Employee, EmployeeDto>();


            });
        }
    }
}
