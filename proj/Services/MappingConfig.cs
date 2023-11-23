using AutoMapper;
using proj.Models;
using proj.Models.BindingModels;

namespace proj.Services
{
    public class MappingConfig// add new class
    {
        public static MapperConfiguration RegisterMaps()
        {//can be written within the code but better organization
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Resume, ResumeInput>();
                config.CreateMap<ResumeInput, Resume>();
                config.CreateMap<ResumeUpdateInput, Resume>();
                config.CreateMap< Resume, ResumeUpdateInput>();
            });
            return mappingConfig;
        }
        //register in services in program.cs
    }
}
