﻿using Domain.Enums;

namespace Domain.Entities;

public record Movie
{
	public Guid Id { get; set; }
	public string Title { get; set; } = default!;
	public int DurationInMinutes { get; set; }
	public AgeRating AgeRating { get; set; }
	public bool IsActive { get; set; } = true;
	public IList<MovieSession> MovieSessions { get; set; } = default!;
}
