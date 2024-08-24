using AutoMapper;
using DB = MotorcycleRental.Models.Database;
using DTO = MotorcycleRental.Models.DTO;
namespace MotorcycleRental.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DB.User, DTO.User>().PreserveReferences().ReverseMap();
            CreateMap<DB.UserType, DTO.UserType>().PreserveReferences().ReverseMap();
            CreateMap<DB.DeliveryPerson, DTO.DeliveryPerson>().PreserveReferences().ReverseMap();
            CreateMap<DB.Vehicle, DTO.Vehicle>().PreserveReferences().ReverseMap();
            CreateMap<DB.Vehicle, DTO.VehicleSimplified>().PreserveReferences().ReverseMap();
            //CreateMap<DB.Order, DTO.Order>().PreserveReferences().ReverseMap();
            //CreateMap<DB.OrderItem, DTO.OrderItem>().PreserveReferences().ReverseMap();
            CreateMap<DB.Rental, DTO.Rental>().PreserveReferences().ReverseMap();
            CreateMap<DB.RentalPlan, DTO.RentalPlan>().PreserveReferences().ReverseMap();
            CreateMap<DB.CNHType, DTO.CNHType>().PreserveReferences().ReverseMap();
            CreateMap<DB.Delivery, DTO.Delivery>().PreserveReferences().ReverseMap();
            //CreateMap<DB.Notification, DTO.Notification>().PreserveReferences().ReverseMap();

            // Configure Rental to RentalInfo mapping
            CreateMap<DB.Rental, DTO.RentalInfo>()
                .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
                .ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.RentalPlan))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))
                .ForMember(dest => dest.Status, opt => opt.Ignore());
        }
    }
}
