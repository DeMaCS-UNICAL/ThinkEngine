#maxint=19. 

xCoord(0..16).
yCoord(0..6).

lookAhead(3).

car(X,Y,RIGHT)  :- cars(brain(collidersCollector(cars(T,car(moveRight(RIGHT)))))), cars(brain(collidersCollector(cars(T,car(xpos(X)))))),cars(brain(collidersCollector(cars(T,car(ypos(Y)))))).

playerPos(X,Y) :- playerSensor(brain(player(xpos(X)))), playerSensor(brain(player(ypos(Y)))).

carNextPos(1,X1,Y,true):- car(X,Y,true),  X1 = X+1.
carNextPos(1,X1,Y,false):- car(X,Y,false),  X1 = X-1.
carNextPos(T1,X1,Y,true):- carNextPos(T,X,Y,true), T1=T+1, X1=X+1, T1<MT, lookAhead(MT).
carNextPos(T1,X1,Y,false):- carNextPos(T,X,Y,false), T1=T+1, X1=X+1, T1<MT, lookAhead(MT).

occupied(X,Y) :- xCoord(X), yCoord(Y), carNextPos(T,X,Y,_).

%moveTo(0).
moveTo(A) :- not occupied(X1,Y), playerPos(X2,Y), X1 = X2 + 1, car(_,Y1, false), Y1=Y+1, A = 3. %move right
moveTo(A) :- not occupied(X1,Y), playerPos(X2,Y), X1 = X2 - 1, car(_,Y1, true), Y1=Y+1, A = 2. %move left
moveTo(A) :- not occupied(X,Y1), playerPos(X,Y2), Y1 = Y2 + 1, A = 1. %move up
moveTo(A) :- not occupied(X,Y1), playerPos(X,Y2), Y1 = Y2 - 1, A = 4. %move down

%stay at the same place
%moveTo(A) :- occupied(X,Y1), playerPos(X,Y2), Y1 = Y2 + 1, A = 0.
%moveTo(0) :- not moveTo(A), int (A), A>0, A<5.
moveTo(A) :- playerPos(_,Y), Y = 6, A = 0.

best(X) :- #min{A : moveTo(A)} = X, #int(X).
:~ best(X). [X:1]

setOnActuator(player(brain(player(move(A))))) :- best(A).