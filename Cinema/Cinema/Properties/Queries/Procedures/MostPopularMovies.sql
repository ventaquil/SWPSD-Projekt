create   procedure procedure_MostPopularMovies 
as
	select tt.title 
		from (select distinct Movies.title as title, t.all_tickets as tickets
			from (
				select Movies.id as all_id, count(Tickets.id) as all_tickets
					from Tickets, Screenings, Movies
					where	Tickets.screeningID = Screenings.id and 
							Screenings.movieID = Movies.id
					group by Movies.id) t, Movies, Screenings
			where all_id = Movies.id and Screenings.movieID = all_id and
			Screenings.screeningDate = CONVERT(date,  GETDATE())) tt
		order by tt.tickets DESC
        