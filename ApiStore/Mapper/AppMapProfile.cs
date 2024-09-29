using ApiStore.Data.Entities;
using ApiStore.Models.Category;
using ApiStore.Models.Products;
using AutoMapper;

namespace ApiStore.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            CreateMap<CategoryCreateViewModel, CategoryEntity>()
                .ForMember(x => x.Image, opt => opt.Ignore());
            CreateMap<CategoryEditViewModel, CategoryEntity>()
                .ForMember(x => x.Image, opt => opt.Ignore());
            CreateMap<CategoryEntity, CategoryItemViewModel>();





            // Налаштування мапінгу для продуктів
            CreateMap<ProductCreateViewModel, ProductEntity>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore()); // Ігноруємо, оскільки ми будемо додавати зображення окремо

            CreateMap<ProductImageEntity, ProductImageViewModel>();
            CreateMap<ProductEntity, ProductItemViewModel>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages.Select(i => i.Image).ToArray()));
        }
    }

}
