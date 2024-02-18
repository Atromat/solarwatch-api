using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Contracts;

public record RegistrationRequest(
    [Required]string Email, 
    [Required]string Username, 
    [Required]string Password);