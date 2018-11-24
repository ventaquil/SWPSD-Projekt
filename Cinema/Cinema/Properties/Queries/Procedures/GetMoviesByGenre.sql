CREATE PROCEDURE procedure_GetMoviesByGenre 
	@genre VARCHAR(100)
AS
	SELECT DISTINCT movies.title, CONVERT(VARCHAR(MAX), movies.description) AS description
	FROM Tags, Genres, 
	(
		SELECT Movies.id, Movies.title AS title, Movies.description AS description
		FROM Movies, Screenings
		WHERE (Movies.id = Screenings.movieID) AND (Screenings.screeningDate = CONVERT(date,  GETDATE()))
	) movies
	WHERE (Genres.id = Tags.genreID) AND (Tags.movieID = movies.id) AND (Genres.genre = @genre);
