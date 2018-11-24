CREATE PROCEDURE procedure_MostPopularMovies 
AS
	SELECT tt.title, tt.description
	FROM (
		SELECT DISTINCT Movies.title AS title, CONVERT(VARCHAR(MAX), Movies.description) AS description, t.all_tickets AS tickets
		FROM (
			SELECT Movies.id AS all_id, COUNT(Tickets.id) AS all_tickets
			FROM Tickets, Screenings, Movies
			WHERE Tickets.screeningID = Screenings.id AND Screenings.movieID = Movies.id
			GROUP BY Movies.id
		) t, Movies, Screenings
		WHERE all_id = Movies.id AND Screenings.movieID = all_id AND Screenings.screeningDate = CONVERT(date,  GETDATE())
	) tt
	ORDER BY tt.tickets DESC
        