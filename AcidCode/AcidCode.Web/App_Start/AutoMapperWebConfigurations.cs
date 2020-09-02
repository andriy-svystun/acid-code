using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AcidCode.Common;

namespace AcidCode.Web
{
    public static class AutoMapperWebConfigurations
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<AcidCodeUser, AcidCode.Web.Models.SiteUserModel>()
                );
        }
    }
}