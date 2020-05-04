#maxint=19. 

xCoord(0..16).
yCoord(0..6).

greenCarPos(X,Y)  :- greenCar(green(car(xpos(X)))), greenCar(green(car(ypos(Y)))).
greenCar2Pos(X,Y) :- greenCar2(green2(car(xpos(X)))), greenCar2(green2(car(ypos(Y)))).
greenCar3Pos(X,Y) :- greenCar3(green3(car(xpos(X)))), greenCar3(green3(car(ypos(Y)))).
greenCar4Pos(X,Y) :- greenCar4(green4(car(xpos(X)))), greenCar4(green4(car(ypos(Y)))).

orangeCarPos(X,Y)  :- orangeCar(orange(car(xpos(X)))), orangeCar(orange(car(ypos(Y)))).
orangeCar2Pos(X,Y) :- orangeCar2(orange2(car(xpos(X)))), orangeCar2(orange2(car(ypos(Y)))).

pinkCarPos(X,Y)  :- pinkCar(pink(car(xpos(X)))), pinkCar(pink(car(ypos(Y)))).
pinkCar2Pos(X,Y) :- pinkCar2(pink2(car(xpos(X)))), pinkCar2(pink2(car(ypos(Y)))).

purpleCarPos(X,Y)  :- purpleCar(purple(car(xpos(X)))), purpleCar(purple(car(ypos(Y)))).
purpleCar2Pos(X,Y) :- purpleCar2(purple2(car(xpos(X)))), purpleCar2(purple2(car(ypos(Y)))).

whiteCarPos(X,Y)  :- whiteCar(white(car(xpos(X)))), whiteCar(white(car(ypos(Y)))).
whiteCar2Pos(X,Y) :- whiteCar2(white2(car(xpos(X)))), whiteCar2(white2(car(ypos(Y)))).

playerPos(X,Y) :- playerSensor(brain(player(xpos(X)))), playerSensor(brain(player(ypos(Y)))).

empty(X,Y) :- xCoord(X), yCoord(Y), not greenCarPos(X,Y), not greenCar2Pos(X,Y), not greenCar3Pos(X,Y), not greenCar4Pos(X,Y),
                                    not orangeCarPos(X,Y), not orangeCar2Pos(X,Y), not pinkCarPos(X,Y), not pinkCar2Pos(X,Y),
                                    not purpleCarPos(X,Y), not purpleCar2Pos(X,Y), not whiteCarPos(X,Y), not whiteCar2Pos(X,Y).

%moveTo(0).
moveTo(A) :- empty(X1,Y), playerPos(X2,Y), X1 = X2 + 1, A = 3. %move right
moveTo(A) :- empty(X1,Y), playerPos(X2,Y), X2 = X1 + 1, A = 2. %move left
moveTo(A) :- empty(X,Y1), playerPos(X,Y2), Y1 = Y2 + 1, A = 1. %move up
moveTo(A) :- empty(X,Y1), playerPos(X,Y2), Y2 = Y1 + 1, A = 4. %move down

%stay at the same place
moveTo(A) :- not empty(X,Y1), playerPos(X,Y2), Y1 = Y2 + 1, A = 0.
moveTo(A) :- playerPos(_,Y), Y = 6, A = 0.

best(X) :- #min{A : moveTo(A)} = X, #int(X).
:~ best(X). [X:1]

setOnActuator(player(brain(player(move(A))))) :- best(A).