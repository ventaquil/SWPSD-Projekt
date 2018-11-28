--basic version : util-only

CREATE TABLE Genres (
	id INT IDENTITY(1,1) PRIMARY KEY,
	genre VARCHAR(50) NOT NULL
);

CREATE TABLE Movies (
	id INT IDENTITY(1,1) PRIMARY KEY,
	title VARCHAR(50) NOT NULL,
	description TEXT NOT NULL
);

CREATE TABLE Tags (
	movieID INT REFERENCES Movies(id),
	genreID INT REFERENCES Genres(id),

	PRIMARY KEY (movieID, genreID)
);

CREATE TABLE Auditoriums (
	id INT PRIMARY KEY
);

CREATE TABLE Seats (
	id INT IDENTITY(1,1) PRIMARY KEY,
	auditoriumID INT REFERENCES Auditoriums(id) NOT NULL,
	rowNo INT NOT NULL,
	seatNo INT NOT NULL
);

CREATE TABLE Screenings (
	id INT IDENTITY(1,1) PRIMARY KEY,
	auditoriumID INT REFERENCES Auditoriums(id) NOT NULL,
	movieID INT REFERENCES Movies(id) NOT NULL,
	screeningDate DATE NOT NULL,
	screeningTime TIME NOT NULL
);

CREATE TABLE Prices (
	id INT IDENTITY(1,1) PRIMARY KEY,
	price float NOT NULL,
	priceDescription VARCHAR(100)
);

CREATE TABLE Tickets (
	id INT IDENTITY(1,1) PRIMARY KEY,
	seatID INT REFERENCES Seats(id) NOT NULL,
	screeningID INT REFERENCES Screenings(id) NOT NULL,
	priceID INT REFERENCES Prices(id) NOT NULL,
	bookerName VARCHAR(100) NOT NULL
);
