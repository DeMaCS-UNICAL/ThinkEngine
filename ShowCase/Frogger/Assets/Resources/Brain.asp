#maxint=100. 

% noMove = 0
% up     = 1
% left   = 2
% right  = 3
% down   = 4
% car(pos_x, pos_x, is_moving_right?)

xCoord(0..16).
yCoord(0..11).

lookAhead(3).

car(X,Y,RIGHT)  :- cars(brain(collidersCollector(cars(T,car(moveRight(RIGHT)))))), cars(brain(collidersCollector(cars(T,car(xpos(X)))))),cars(brain(collidersCollector(cars(T,car(ypos(Y)))))).
car(XX,Y,RIGHT)  :- cars(brain(collidersCollector(cars(T,car(moveRight(RIGHT)))))), cars(brain(collidersCollector(cars(T,car(xpos(X)))))),cars(brain(collidersCollector(cars(T,car(ypos(Y)))))), XX=X-1.
car(XX,Y,RIGHT)  :- cars(brain(collidersCollector(cars(T,car(moveRight(RIGHT)))))), cars(brain(collidersCollector(cars(T,car(xpos(X)))))),cars(brain(collidersCollector(cars(T,car(ypos(Y)))))), XX=X+1.
log(XX1,XX2,Y,RIGHT) :-logs(brain(collidersCollector(logs(X,collidableObject(leftMargin(X1)))))), logs(brain(collidersCollector(logs(X,collidableObject(rightMargin(X2)))))), logs(brain(collidersCollector(logs(X,collidableObject(y(Y)))))), logs(brain(collidersCollector(logs(X,collidableObject(right(RIGHT)))))), XX1=X1+1, XX2=X2-1.
home(X1,X2,Y,OCCUPIED) :- homeBay(brain(collidersCollector(bay(X,collidableObject(leftMargin(X1)))))), homeBay(brain(collidersCollector(bay(X,collidableObject(rightMargin(X2)))))), homeBay(brain(collidersCollector(bay(X,collidableObject(y(Y)))))), homeBay(brain(collidersCollector(bay(X,collidableObject(isOccupied(OCCUPIED)))))).


playerPos(X,Y) :- playerSensor(brain(player(xpos(X)))), playerSensor(brain(player(ypos(Y)))).

carNextPos(1,X1,Y,true):- car(X,Y,true),  X1 = X+1.
carNextPos(1,X1,Y,false):- car(X,Y,false),  X1 = X-1.
carNextPos(T1,X1,Y,true):- carNextPos(T,X,Y,true), T1=T+1, X1=X+1, T1<MT, lookAhead(MT).
carNextPos(T1,X1,Y,false):- carNextPos(T,X,Y,false), T1=T+1, X1=X-1, T1<MT, lookAhead(MT).

occupiedByCar(X,Y) :- xCoord(X), yCoord(Y), carNextPos(T,X,Y,_).
occupiedByCar(X,Y) :- xCoord(X), yCoord(Y), car(X,Y,_).
 
occupiedByLog(X,Y) :- xCoord(X), log(X1,X2,Y,_), X>=X1, X<=X2.


% Safe positions
safe(X,Y):- xCoord(X), Y=0.
safe(X,Y):- xCoord(X), Y=6.
safe(X,Y):- occupiedByLog(X,Y).
safe(X,Y):- not occupiedByCar(X,Y), xCoord(X), yCoord(Y), Y<6.

% Final positions are also safe
safe(X,Y) :- xCoord(X), home(X1,X2,Y,false), X>=X1, X<=X2.

moveTo(0)|moveTo(1)|moveTo(2)|moveTo(3)|moveTo(4).


nextPlayerPos(X,Y):-playerPos(X,Y), moveTo(0).
nextPlayerPos(X,Y1):-playerPos(X,Y), moveTo(1), Y1=Y+1.%up
nextPlayerPos(X,Y1):-playerPos(X,Y), moveTo(4), Y1=Y-1.%down
nextPlayerPos(X1,Y):-playerPos(X,Y), moveTo(2), X1=X-1.%left
nextPlayerPos(X1,Y):-playerPos(X,Y), moveTo(3), X1=X+1.%right

:-moveTo(X),moveTo(Y),X!=Y.
:-moveTo(3), playerPos(_,Y), car(_,Y1,true), Y1=Y+1.
:-moveTo(2), playerPos(_,Y), car(_,Y1,false), Y1=Y+1.

% No suicide
:- nextPlayerPos(X,Y), not safe(X,Y). %[1:2]

% Optimization
:~ nextPlayerPos(X,Y), playerPos(X1,Y1), Y<Y1, X=X1. [1:2]
:~ nextPlayerPos(X,Y), playerPos(X1,Y1), Y=Y1, X=X1. [1:2]
:~ moveTo(X). [X:1]
:~ moveTo(2), playerPos(0,Y). [2:1]

setOnActuator(player(brain(player(move(A))))) :- moveTo(A).