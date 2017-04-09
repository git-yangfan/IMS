

/*******************************************************************************
* Copyright (C)  JuCheap.Com
* 
* Author: dj.wong
* Create Date: 2015/8/7 11:12:12
* Description: Automated building by service@JuCheap.com 
* 
* Revision History:
* Date         Author               Description
*
*********************************************************************************/

using System.Linq;
using AutoMapper;


namespace IMS.Web.Config
{
    /// <summary>
    /// AutoMapper 自定义扩展配置
    /// </summary>
    public partial class AutoMapperConfiguration
    {
        /// <summary>
        /// AutoMapper 自定义扩展配置
        /// </summary>
        public static void ConfigExt()
        {
            //Mapper.Initialize.CreateMap<UserDto, UserEntity>()
            //    .ForMember(u => u.Status, e => e.MapFrom(s => (byte) s.Status));

           
        }
    }
}
