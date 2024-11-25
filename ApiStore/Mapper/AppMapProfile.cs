﻿using ApiStore.Data.Entities;
using ApiStore.Models.Category;
using ApiStore.Models.Product;
using ApiStore.Models.Products;
using AutoMapper;

namespace ApiStore.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            // Налаштування мапінгу для категорій
            CreateMap<CategoryCreateViewModel, CategoryEntity>()
                .ForMember(x => x.Image, opt => opt.Ignore());
            CreateMap<CategoryEditViewModel, CategoryEntity>()
                .ForMember(x => x.Image, opt => opt.Ignore());
            CreateMap<CategoryEntity, CategoryItemViewModel>(); 

            // Налаштування мапінгу для продуктів
            CreateMap<ProductCreateViewModel, ProductEntity>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore()); // Ігноруємо, оскільки ми будемо додавати зображення окремо

            // Мапінг для редагування продуктів
            CreateMap<ProductEditViewModel, ProductEntity>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore()); // Ігноруємо, оскільки зображення обробляються окремо

            // Для зображень продуктів
            CreateMap<ProductImageEntity, ProductImageViewModel>();

            // Мапінг для відображення продуктів (ItemViewModel)
            CreateMap<ProductEntity, ProductItemViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages.Select(i => i.Image).ToArray()));

            // Додаємо мапінг для завантаження продукту на редагування
            CreateMap<ProductEntity, ProductEditViewModel>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages)); // Передаємо зображення\\

            CreateMap<ProductDescImageEntity, ProductDescImageIdViewModel>();
        }
    }
}