using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiApplication.DTOs.API
{
    public class Showtime
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("start_date")]
        [Required]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        [Required]
        public string EndDate { get; set; }

        [JsonPropertyName("schedule")]
        [Required]
        public string Schedule { get; set; }

        [JsonPropertyName("movie")]
        [Required]
        public Movie Movie { get; set; }

        [JsonPropertyName("auditorium_id")]
        [Required]
        public int AuditoriumId { get; set; }
    }
}
