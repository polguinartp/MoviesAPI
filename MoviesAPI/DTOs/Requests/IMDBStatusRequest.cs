using System;

namespace MoviesAPI.DTOs.Requests;

public record IMDBStatusRequest(bool Up, DateTime LastCall);
