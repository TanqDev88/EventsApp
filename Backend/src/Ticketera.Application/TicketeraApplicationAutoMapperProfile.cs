using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Ticketera.Entities;
using Ticketera.Events;
using Ticketera.FileAttachments;
using Ticketera.ProviderPayments;
using Ticketera.TicketCategories;
using Ticketera.Tickets;
using Ticketera.TicketSectors;

namespace Ticketera;

public class TicketeraApplicationAutoMapperProfile : Profile
{
    public TicketeraApplicationAutoMapperProfile()
    {
        CreateMap<TicketCategory, TicketCategoryDto>().ReverseMap();
        CreateMap<TicketSector, TicketSectorDto>().ReverseMap();
        CreateMap<ProviderPayment, ProviderPaymentDto>().ReverseMap();
        CreateMap<PriceModel, PriceDto>().ReverseMap();
        CreateMap<EventDate, EventDateDto>().ReverseMap();
        CreateMap<Event, EventDto>()
                                    .ForMember(dest => dest.PhotoDetail, opt => opt.MapFrom(src => src.FileAttachments.FirstOrDefault(x => !x.IsDefault && x.FileType == Enum.FileAttachmentType.Detail).Path))
                                    .ForMember(dest => dest.PhotoGallery, opt => opt.MapFrom(src => src.FileAttachments.FirstOrDefault(x => x.IsDefault && x.FileType == Enum.FileAttachmentType.Gallery).Path))
                                    .ForMember(dest => dest.PhotoLogo, opt => opt.MapFrom(src => src.FileAttachments.FirstOrDefault(x => x.FileType == Enum.FileAttachmentType.Logo).Path))
                                    .ForMember(dest => dest.IdPhotoLogo, opt => opt.MapFrom(src => src.FileAttachments.FirstOrDefault(x => x.FileType == Enum.FileAttachmentType.Logo).Id))
                                    .ForMember(dest => dest.IdProviderPayment, opt => opt.MapFrom(src => src.EventProviders.Select(ep => ep.ProviderPaymentId)))
                                    .ReverseMap();
        CreateMap<FileAttachment, FileAttachmentDto>().ReverseMap();
        CreateMap<Ticket, TicketDto>().ReverseMap();
        CreateMap<EventInputDto, Event>();

        CreateMap<PurchaseEventDto, Purchase>();
        CreateMap<PurchaseTicketDto, Ticket>();
        CreateMap<PurchaseTicketInputDto, Ticket>();
        CreateMap<PurchaseInputDto, Purchase>();
        CreateMap<Purchase, PurchaseDto>();
        CreateMap<Ticket, TicketOutDto>();
        CreateMap<IdentityUser, UserOutDto>();
        CreateMap<UserEvent, UserEventDto>();
    }
}
