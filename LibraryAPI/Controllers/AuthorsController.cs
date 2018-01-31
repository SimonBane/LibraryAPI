﻿using AutoMapper;
using LibraryAPI.Entities;
using LibraryAPI.Helpers;
using LibraryAPI.Models;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LibraryAPI.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IUrlHelper _urlHelper;

        public AuthorsController(ILibraryRepository libraryRepository, IUrlHelper urlHelper)
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            var entityAuthors = _libraryRepository.GetAuthors(authorsResourceParameters);

            var previousPageLink = entityAuthors.HasPrevious
                ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage)
                : null;

            var nextPageLink = entityAuthors.HasNext
                ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = entityAuthors.TotalCount,
                pageSize = entityAuthors.PageSize,
                currentPage = entityAuthors.CurrentPage,
                totalPages = entityAuthors.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var authors = Mapper.Map<IEnumerable<AuthorDto>>(entityAuthors);
            return Ok(authors);
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                default:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize
                        });

            }
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(int id)
        {
            var authorEntity = _libraryRepository.GetAuthor(id);
            if (authorEntity == null)
            {
                return NotFound();
            }

            var author = Mapper.Map<AuthorDto>(authorEntity);
            return Ok(author);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if (author == null)
            {
                return BadRequest();
            }
            var authorEntity = Mapper.Map<Author>(author);

            _libraryRepository.AddAuthor(authorEntity);
            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an author failed on save.");
            }

            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id }, authorToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(int id)
        {
            if (_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(int id)
        {
            var authorEntity = _libraryRepository.GetAuthor(id);
            if (authorEntity == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorEntity);
            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting author {id} failed on save.");
            }

            return NoContent();
        }
    }
}