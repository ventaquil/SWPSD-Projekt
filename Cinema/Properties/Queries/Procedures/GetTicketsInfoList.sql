create or alter procedure procedure_GetTicketsInfoList
as
	select Seats.rowNo, Seats.seatNo, Movies.title, Screenings.screeningDate, Screenings.screeningTime, Prices.priceDescription, Prices.price, Tickets.bookerName 
		from Movies, Screenings, Seats, Prices, Tickets
		where	Movies.id = Screenings.movieID and
				Screenings.id = Tickets.screeningID and
				Seats.id = Tickets.seatID and
				Prices.id = Tickets.priceID;


