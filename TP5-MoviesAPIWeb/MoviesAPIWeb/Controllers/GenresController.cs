using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPIWeb.Dtos;
using MoviesAPIWeb.Models;
using MoviesAPIWeb.Services;

namespace MoviesAPIWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;
        public GenresController(IGenresService genresService)
        {
            _genresService = genresService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _genresService.GetAll();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre(GenreDto dto)
        {
            var genre = new Genre
            {
                Name = dto.Name
            };
            await _genresService.Add(genre);
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(byte id, [FromBody] GenreDto dto)
        {
            var genre = await _genresService.GetById(id);
            if (genre == null)
                return NotFound($"No genre was found with ID {id}");

            genre.Name = dto.Name;
            _genresService.Update(genre);
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(byte id)
        {
            var genre = await _genresService.GetById(id);
            if (genre == null)
                return NotFound($"No genre was found with ID {id}");

            _genresService.Delete(genre);
            return Ok(genre);
        }
    }
}

