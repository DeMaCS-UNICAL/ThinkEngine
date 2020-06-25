#maxint=100. 

xCoord(0..16).
yCoord(0..11).

lookAhead(3).

car(X,Y,RIGHT)  :- cars(brain(collidersCollector(cars(T,car(moveRight(RIGHT)))))), cars(brain(collidersCollector(cars(T,car(xpos(X)))))),cars(brain(collidersCollector(cars(T,car(ypos(Y)))))).
log(X1,X2,Y,RIGHT) :-logs(brain(collidersCollector(logs(X,collidableObject(leftMargin(X1)))))), logs(brain(collidersCollector(logs(X,collidableObject(rightMargin(X2)))))), logs(brain(collidersCollector(logs(X,collidableObject(y(Y)))))), logs(brain(collidersCollector(logs(X,collidableObject(right(RIGHT)))))).

playerPos(X,Y) :- playerSensor(brain(player(xpos(X)))), playerSensor(brain(player(ypos(Y)))).

carNextPos(1,X1,Y,true):- car(X,Y,true),  X1 = X+1.
carNextPos(1,X1,Y,false):- car(X,Y,false),  X1 = X-1.
carNextPos(T1,X1,Y,true):- carNextPos(T,X,Y,true), T1=T+1, X1=X+1, T1<MT, lookAhead(MT).
carNextPos(T1,X1,Y,false):- carNextPos(T,X,Y,false), T1=T+1, X1=X-1, T1<MT, lookAhead(MT).

occupiedByCar(X,Y) :- xCoord(X), yCoord(Y), carNextPos(T,X,Y,_).
occupiedByCar(X,Y) :- xCoord(X), yCoord(Y), car(X,Y,_).
 
occupiedByLog(X,Y) :- xCoord(X), log(X1,X2,Y,_), X>=X1, X<=X2.

 safe(X,Y):- xCoord(X), Y=6.
 safe(X,Y):- occupiedByLog(X,Y).
 safe(X,Y):- not occupiedByCar(X,Y), xCoord(X), yCoord(Y), Y<7.

moveTo(0)|moveTo(1)|moveTo(2)|moveTo(3)|moveTo(4).
nextPlayerPos(X,Y):-playerPos(X,Y), moveTo(0).
nextPlayerPos(X,Y1):-playerPos(X,Y), moveTo(1), Y1=Y+1.%up
nextPlayerPos(X,Y1):-playerPos(X,Y), moveTo(4), Y1=Y-1.%down
nextPlayerPos(X1,Y):-playerPos(X,Y), moveTo(2), X1=X-1.%left
nextPlayerPos(X1,Y):-playerPos(X,Y), moveTo(3), X1=X+1.%right

:-moveTo(X),moveTo(Y),X!=Y.
:-moveTo(3), playerPos(_,Y), car(_,Y1,true), Y1=Y+1.
:-moveTo(2), playerPos(_,Y), car(_,Y1,false), Y1=Y+1.


:~ nextPlayerPos(X,Y), not safe(X,Y). [1:3]
:~ nextPlayerPos(X,Y), playerPos(X1,Y1), Y=Y1, X=X1. [1:2]
:~ moveTo(X). [X:1]


setOnActuator(player(brain(player(move(A))))) :- moveTo(A).







%moveTo(A) :- not occupiedByCar(X1,Y), playerPos(X2,Y), X1 = X2 + 1, car(_,Y1, false), Y1=Y+1, Y<6, A = 3. %move right
%moveTo(A) :- not occupiedByCar(X1,Y), playerPos(X2,Y), X1 = X2 - 1, car(_,Y1, true), Y1=Y+1, Y<6, A = 2. %move left
%moveTo(A) :- not occupiedByCar(X,Y1), playerPos(X,Y2), Y1 = Y2 + 1, Y2<6, A = 1. %move up
%moveTo(A) :- not occupiedByCar(X,Y1), playerPos(X,Y2), Y1 = Y2 - 1, Y2<6, A = 4. %move down

%moving :- moveTo(A), A<>0.
%moveTo(1) :- occupiedByLog(X,Y), playerPos(X,Y1), Y1=Y-1.

%stay at the same place
%moveTo(A) :- occupiedByCar(X,Y1), playerPos(X,Y2), Y1 = Y2 + 1, A = 0.
%moveTo(0) :- not moveTo(1), not moveTo(2), not moveTo(3), not moveTo(4).
%moveTo(A) :- playerPos(_,Y), Y = 11, A = 0.

%best(X) :- #min{A : moveTo(A)} = X, #int(X).
%:~ best(X). [X:1]

%setOnActuator(player(brain(player(move(A))))) :- best(A).