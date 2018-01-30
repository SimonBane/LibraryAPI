using AutoMapper;
using LibraryAPI.Entities;
using LibraryAPI.Models;
using LibraryAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LibraryAPI.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetBooksForAuthor(int authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorEntity = _libraryRepository.GetBooksForAuthor(authorId);
            var booksForAuthor = Mapper.Map<IEnumerable<BookDto>>(booksForAuthorEntity);

            return Ok(booksForAuthor);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(int authorId, int id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorEntity = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorEntity == null)
            {
                return NotFound();
            }

            var bookForAuthor = Mapper.Map<BookDto>(bookForAuthorEntity);
            return Ok(bookForAuthor);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(int authorId, [FromBody] BookForCreationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = Mapper.Map<Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);
            if (!_libraryRepository.Save())
            {
                throw new Exception($"Creating a book for author {authorId} failed on save.");
            }

            var bookToReturn = Mapper.Map<BookDto>(bookEntity);
            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = bookToReturn.Id }, bookToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(int authorId, int id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorEntity = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorEntity == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookForAuthorEntity);
            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(int authorId, int id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorEntity = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(book, bookForAuthorEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Updating book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(int authorId, int id, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookForAuthorEntity = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorEntity == null)
            {
                return NotFound();
            }

            var bookToPatch = Mapper.Map<BookForUpdateDto>(bookForAuthorEntity);

            patchDocument.ApplyTo(bookToPatch);

            // add validation

            Mapper.Map(bookToPatch, bookForAuthorEntity);
            if (!_libraryRepository.Save())
            {
                throw new Exception($"Patching book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }
    }
}