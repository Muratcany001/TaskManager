using AutoMapper;
using Dtos;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Mappings;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<UserTask, TaskDto>();
        
        CreateMap<CreateTaskDto, UserTask>()
            .ForMember(dest =>dest.Id , opt=> opt.Ignore())
            .ForMember(dest => dest.CreateDate , opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdateTaskDto, UserTask>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDate, opt => opt.Ignore());

        CreateMap<UserTask, TaskSummaryDto>();
        CreateMap<UserTask, TaskResponseDto>();
    }
}