--basic version : util-only

create table Genres (
	id int identity(1,1) primary key,
	genre varchar(50) not null
);

create table Movies (
	id int identity(1,1) primary key,
	title varchar(50) not null
);

create table Tags (
	movieID int references Movies(id),
	genreID int references Genres(id),

	primary key (movieID, genreID)
);

create table Auditoriums (
	id int primary key
);

create table Seats (
	id int identity(1,1) primary key,
	auditoriumID int references Auditoriums(id) not null,
	rowNo int not null,
	seatNo int not null
);

create table Screenings (
	id int identity(1,1) primary key,
	auditoriumID int references Auditoriums(id) not null,
	movieID int references Movies(id) not null,
	screeningDate date not null,
	screeningTime time not null
);

create table Prices (
	id int identity(1,1) primary key,
	price float not null,
	priceDescription varchar(100)
);

create table Tickets (
	id int identity(1,1) primary key,
	seatID int references Seats(id) not null,
	screeningID int references Screenings(id) not null,
	priceID int references Prices(id) not null,
	bookerName varchar(100) not null
);