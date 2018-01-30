﻿using AutoMapper;
using LibraryAPI.Entities;
using LibraryAPI.Helpers;
using LibraryAPI.Models;

namespace LibraryAPI.Mapping
{
    /// <summary>
    /// Automapper configuration.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // From Entity to Dto
            CreateMap<Author, AuthorDto>()
                .ForMember(dto => dto.Name, opt => opt.MapFrom(entity =>
                    $"{entity.FirstName} {entity.LastName}"))
                .ForMember(dto => dto.Age, opt => opt.MapFrom(entity =>
                    entity.DateOfBirth.GetCurrentAge()));

            CreateMap<Book, BookDto>();

            // From Dto to Entity
            CreateMap<AuthorForCreationDto, Author>();
            CreateMap<BookForCreationDto, Book>();
        }
    }
}