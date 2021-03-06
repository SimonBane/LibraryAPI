﻿using AutoMapper;
using Library.Web.Entities;
using Library.Web.Helpers;
using Library.Web.Models;
using Library.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Web.Controllers
{
    /// <summary>
    /// Controller for manipulating Authors.
    /// </summary>
    [Route("api/authors")]
    [Authorize]
    [EnableCors("AllowSpecificOrigin")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private ILog _logger = Logger.GetInstance;

        /// <inheritdoc />
        public AuthorsController(ILibraryRepository libraryRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        /// <summary>
        /// Gets all existing authors.
        /// </summary>
        /// <param name="authorsResourceParameters"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetAuthors")]
        [ProducesResponseType(typeof(IEnumerable<AuthorDto>), 200, StatusCode = StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>
                (authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields))
            {
                return BadRequest();
            }

            try
            {
                var entityAuthors = _libraryRepository.GetAuthors(authorsResourceParameters);

                var paginationMetadata = new
                {
                    totalCount = entityAuthors.TotalCount,
                    pageSize = entityAuthors.PageSize,
                    currentPage = entityAuthors.CurrentPage,
                    totalPages = entityAuthors.TotalPages
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

                var authors = Mapper.Map<IEnumerable<AuthorDto>>(entityAuthors);

                var links = CreateLinksForAuthors(authorsResourceParameters,
                    entityAuthors.HasNext, entityAuthors.HasPrevious);

                var shapedAuthors = authors.ShapeData(authorsResourceParameters.Fields);

                var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
                {
                    var authorAsDictionary = author as IDictionary<string, object>;
                    var authorLinks =
                        CreateLinksForAuthor((int)authorAsDictionary["Id"], authorsResourceParameters.Fields);

                    authorAsDictionary.Add("links", authorLinks);

                    return authorAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedAuthorsWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }
            catch (Exception ex)
            {
                await _logger.LogCustomExceptionAsync(ex, null);
                return RedirectToAction("Error", "Home");
            }
        }

        #region CreateResourceUri

        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters,
                    ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber - 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber + 1,
                            pageSize = authorsResourceParameters.PageSize
                        });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetAuthors",
                        new
                        {
                            fields = authorsResourceParameters.Fields,
                            orderBy = authorsResourceParameters.OrderBy,
                            searchQuery = authorsResourceParameters.SearchQuery,
                            genre = authorsResourceParameters.Genre,
                            pageNumber = authorsResourceParameters.PageNumber,
                            pageSize = authorsResourceParameters.PageSize
                        });

            }
        }

        #endregion

        /// <summary>
        /// Gets a single author.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetAuthor")]
        [ProducesResponseType(typeof(AuthorDto), 200, StatusCode = StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuthor(int id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }

            try
            {
                var authorEntity = _libraryRepository.GetAuthor(id);
                if (authorEntity == null)
                {
                    return NotFound();
                }

                var author = Mapper.Map<AuthorDto>(authorEntity);

                var links = CreateLinksForAuthor(id, fields);

                var linkedResourceToReturn = author.ShapeData(fields)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);
                return Ok(linkedResourceToReturn);
            }
            catch (Exception ex)
            {
                await _logger.LogCustomExceptionAsync(ex, null);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            try
            {
                var authorEntity = Mapper.Map<Author>(author);

                _libraryRepository.AddAuthor(authorEntity);
                if (!_libraryRepository.Save())
                {
                    throw new Exception("Creating an author failed on save.");
                }

                var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);

                var links = CreateLinksForAuthor(authorToReturn.Id, null);

                var linkedResourceToReturn = authorToReturn.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return CreatedAtRoute("GetAuthor", new { id = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
            }
            catch (Exception ex)
            {
                await _logger.LogCustomExceptionAsync(ex, null);
                return RedirectToAction("Error", "Home");
            }
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

        [HttpDelete("{id}", Name = "DeleteAuthor")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
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
            catch (Exception ex)
            {
                await _logger.LogCustomExceptionAsync(ex, null);
                return RedirectToAction("Error", "Home");
            }
        }

        private IEnumerable<LinkDto> CreateLinksForAuthor(int id, string fields)
        {
            var links = new List<LinkDto>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(_urlHelper.Link("GetAuthor", new { id }),
                    "self",
                    "GET"));
            }
            else
            {
                links.Add(
                    new LinkDto(_urlHelper.Link("GetAuthor", new { id, fields }),
                    "self",
                    "GET"));
            }

            links.Add(
                new LinkDto(_urlHelper.Link("DeleteAuthor", new { id }),
                "delete_author",
                "DELETE"));

            links.Add(
                new LinkDto(_urlHelper.Link("CreateBookForAuthor", new { authorId = id }),
                "create_book_for_author",
                "POST"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParameters authorsResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters,
                        ResourceUriType.Current),
                    "self", "GET")
            };


            if (hasNext)
            {
                links.Add(
                    new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters,
                    ResourceUriType.NextPage),
                    "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}