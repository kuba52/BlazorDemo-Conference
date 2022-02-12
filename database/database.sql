CREATE TABLE plane(
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    model varchar(255) NOT NULL,
    company VARCHAR(255) NOT NULL,
    capacity INTEGER NOT NULL,
    UNIQUE (model, company, capacity)
);
INSERT INTO plane(model, company, capacity)
VALUES
    ('F16', 'British Airlines', 20),
    ('F17', 'LOT', 30),
    ('F22', 'Emirates', 15),
    ('F21', 'Ryanair', 19),
    ('TU154M', 'Lufthansa', 22)
;

CREATE TABLE city (
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    destination VARCHAR(255) NOT NULL,
    UNIQUE (destination)
);

INSERT INTO city(destination)
VALUES
    ('London'),
    ('Warsaw'),
    ('Cracow'),
    ('Madrit'),
    ('New York'),
    ('Mocsow'),
    ('Rio de Janeo'),
    ('Sidney')
;

CREATE TABLE flights(
    id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    start_time TIMESTAMP(0) NOT NULL,
    start INTEGER REFERENCES city NOT NULL,
    arrival_time TIMESTAMP(0) NOT NULL,
    destination INTEGER REFERENCES city NOT NULL,
    plane_id INTEGER REFERENCES plane NOT NULL,
    CHECK ( start_time < arrival_time )
);

INSERT INTO flights(start_time, arrival_time, start, destination, plane_id) VALUES
    ('2022-03-09', '2022-03-10',  1, 4, 4),
    ('2022-03-09', '2022-04-10',  2, 5, 2),
    ('2022-03-09', '2022-04-10',  2, 1, 3),
    ('2022-03-09', '2022-04-10',  3, 7, 1),
    ('2022-03-09', '2022-04-10',  1, 5, 3),
    ('2022-03-09', '2022-04-10',  2, 1, 1),
    ('2022-03-09', '2022-04-10',  4, 7, 5)

;

CREATE TABLE account(
    id INTEGER GENERATED ALWAYS AS identity PRIMARY KEY,
    name varchar(255) NOT NULL,
    surname varchar(255) not null,
    is_staff boolean not null
);

INSERT INTO account(name, surname, is_staff) VALUES('Krzysztof', 'Piątek', TRUE);

CREATE TABLE passengers(
    flight_id INTEGER REFERENCES flights NOT NULL,
    account_id INTEGER REFERENCES account NOT NULL,
    UNIQUE (flight_id, account_id)
);

INSERT INTO passengers(flight_id, account_id) VALUES (1,1), (2,1), (5,1);


CREATE OR REPLACE FUNCTION max_people_on_board_trgfn() RETURNS TRIGGER AS
$$
DECLARE min INTEGER;
BEGIN

    WITH quantity AS (
        SELECT flight_id, count(account_id) as quantity
        FROM passengers
        GROUP BY flight_id
    )
    SELECT (p.capacity - q.quantity) AS free_seats
    FROM quantity q JOIN flights f ON q.flight_id = f.id JOIN plane p ON f.plane_id = p.id
    ORDER BY p.capacity - q.quantity
    LIMIT 1 INTO min;

    IF (min < 0) THEN
      RAISE EXCEPTION 'There is no space for more passengers.';
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS max_people_on_board_trg ON passengers;

CREATE TRIGGER max_people_on_board_trg
  BEFORE INSERT OR UPDATE ON passengers
EXECUTE PROCEDURE max_people_on_board_trgfn();
