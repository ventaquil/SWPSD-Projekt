create   procedure procedure_GetMoviesByGenre 
	@genre varchar(100)
as
	select distinct movies.title 
		from Tags, Genres, 
		(select Movies.id, Movies.title as title from Movies, Screenings where Movies.id = Screenings.movieID and Screenings.screeningDate = CONVERT(date,  GETDATE())) movies
		where	Genres.id = Tags.genreID and 
				Tags.movieID = movies.id and Genres.genre = @genre;
