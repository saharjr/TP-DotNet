using System.ComponentModel.DataAnnotations;

namespace MoviesAPIWeb.Dtos
{
    public class GenreDto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
