using AutoMapper;
using Dtos.DocumentDtos;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Mappings;

public class DocumentProfile : Profile
{
    public DocumentProfile() {

        CreateMap<Document, CreateDocumentDto>();

        CreateMap<Document, UpdateDocumentDto>();
    }
}