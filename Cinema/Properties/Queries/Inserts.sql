INSERT INTO Genres(genre) VALUES ('Film akcji');
INSERT INTO Genres(genre) VALUES ('Komedia');
INSERT INTO Genres(genre) VALUES ('Romans');
INSERT INTO Genres(genre) VALUES ('Film animowany');
INSERT INTO Genres(genre) VALUES ('Horror');
INSERT INTO Genres(genre) VALUES ('Film dokumentalny');

INSERT INTO Movies(title, description) VALUES ('World War Z', 'World War Z – amerykańsko-maltański horror sci-fi z 2013 roku w reżyserii Marca Forstera.');
INSERT INTO Movies(title, description) VALUES ('Underworld', 'Underworld – amerykański film z 2003 roku w reżyserii Lena Wisemana.');
INSERT INTO Movies(title, description) VALUES ('Shrek', 'Shrek – amerykański film animowany z 2001 roku w reżyserii Andrew Adamsona i Vicky Jenson.');
INSERT INTO Movies(title, description) VALUES ('Potwory i spółka', 'Potwory i spółka - amerykański pełnometrażowy film animowany w reżyserii Pete''a Doctera z 2001 roku wyprodukowany przez wytwórnię Pixar i Disney. Film został wykonany w technice trójwymiarowej. W lutym 2013 roku film trafił ponownie do kin, ale w wersji 3D. 5 lipca 2013 roku do kin trafia prequel o nazwie Uniwersytet potworny.');
INSERT INTO Movies(title, description) VALUES ('Turysta', 'Turysta - amerykańsko-francuski thriller w reżyserii Floriana Henckela von Donnersmarcka z Johnnym Deppem i Angeliną Jolie w rolach głównych.');
INSERT INTO Movies(title, description) VALUES ('Latawce', 'Latawce – bollywoodzki film akcji wyreżyserowany w 2010 w Ameryce przez Anurag Basu. W rolach głównych Hrithik Roshan, Bárbara Mori, Kangana Ranaut i Kabir Bedi.');
INSERT INTO Movies(title, description) VALUES ('Wojownik', 'Wojownik − amerykański film fabularny z 2011 roku, napisany i wyreżyserowany przez Gavina O’Connora.');
INSERT INTO Movies(title, description) VALUES ('Zmierzch', 'Zmierzch – amerykański film z 2008 roku, w reżyserii Catherine Hardwicke, oparty na książce Stephenie Meyer – Zmierzch. W rolach głównych występują Kristen Stewart i Robert Pattinson.');

INSERT INTO Tags VALUES (1,1);
INSERT INTO Tags VALUES (1,5);
INSERT INTO Tags VALUES (2,1);
INSERT INTO Tags VALUES (2,5);
INSERT INTO Tags VALUES (3,2);
INSERT INTO Tags VALUES (3,4);
INSERT INTO Tags VALUES (4,2);
INSERT INTO Tags VALUES (4,4);
INSERT INTO Tags VALUES (5,3);
INSERT INTO Tags VALUES (6,1);
INSERT INTO Tags VALUES (6,3);
INSERT INTO Tags VALUES (7,1);
INSERT INTO Tags VALUES (7,3);
INSERT INTO Tags VALUES (8,3);
INSERT INTO Tags VALUES (8,5);

INSERT INTO Auditoriums VALUES (1);
INSERT INTO Auditoriums VALUES (2);
INSERT INTO Auditoriums VALUES (3);

INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 1, 1);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 1, 2);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 1, 3);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 1, 4);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 1, 5);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 2, 1);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 2, 2);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 2, 3);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 2, 4);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 2, 5);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 3, 1);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 3, 2);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 3, 3);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 3, 4);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (1, 3, 5);

INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (2, 1, 1);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (2, 1, 2);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (2, 1, 3);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (2, 1, 4);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (2, 2, 1);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (2, 2, 2);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (2, 2, 3);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (2, 2, 4);

INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (3, 1, 1);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (3, 1, 2);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (3, 1, 3);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (3, 1, 4);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (3, 1, 5);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (3, 1, 6);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (3, 1, 7);
INSERT INTO Seats(auditoriumID, rowNo, seatNo) VALUES (3, 1, 8);

INSERT INTO Prices(price, priceDescription) VALUES (24.0, 'Bilet normalny');
INSERT INTO Prices(price, priceDescription) VALUES (16.0, 'Bilet studencki');
INSERT INTO Prices(price, priceDescription) VALUES (18.0, 'Bilet ulgowy');
INSERT INTO Prices(price, priceDescription) VALUES (12.0, 'Bilet poniedziałkowy poranny');
INSERT INTO Prices(price, priceDescription) VALUES (8.50, 'Bilet dla dzieci do lat 56');
INSERT INTO Prices(price, priceDescription) VALUES (99.99, 'Bilet dla niegrzecznych klientów');

INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (1, 2, CONVERT(VARCHAR(10), GETDATE(), 126), '08:00');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (1, 2, CONVERT(VARCHAR(10), GETDATE(), 126), '11:15');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (1, 2, CONVERT(VARCHAR(10), GETDATE(), 126), '14:30');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (1, 8, CONVERT(VARCHAR(10), GETDATE(), 126), '17:45');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (1, 8, CONVERT(VARCHAR(10), GETDATE(), 126), '21:00');

INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (2, 8, CONVERT(VARCHAR(10), GETDATE(), 126), '08:00');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (2, 5, CONVERT(VARCHAR(10), GETDATE(), 126), '11:15');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (2, 7, CONVERT(VARCHAR(10), GETDATE(), 126), '13:10');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (2, 6, CONVERT(VARCHAR(10), GETDATE(), 126), '16:45');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (2, 6, CONVERT(VARCHAR(10), GETDATE(), 126), '20:10');

INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (3, 4, CONVERT(VARCHAR(10), GETDATE(), 126), '08:00');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (3, 2, CONVERT(VARCHAR(10), GETDATE(), 126), '09:45');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (3, 2, CONVERT(VARCHAR(10), GETDATE(), 126), '13:00');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (3, 1, CONVERT(VARCHAR(10), GETDATE(), 126), '16:15');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (3, 5, CONVERT(VARCHAR(10), GETDATE(), 126), '19:30');
INSERT INTO Screenings(auditoriumID, movieID, screeningDate, screeningTime) VALUES (3, 1, CONVERT(VARCHAR(10), GETDATE(), 126), '21:25');
