#maxint=19. 

xCoord(0..16).
yCoord(0..6).

lookAhead(2).

car(X,Y,RIGHT)  :- greenCar(green(car(xpos(X)))), greenCar(green(car(ypos(Y)))),greenCar(green(car(moveRight(RIGHT)))).
car(X,Y,RIGHT) :- greenCar2(green2(car(xpos(X)))), greenCar2(green2(car(ypos(Y)))),greenCar2(green2(car(moveRight(RIGHT)))).
car(X,Y,RIGHT) :- greenCar3(green3(car(xpos(X)))), greenCar3(green3(car(ypos(Y)))),greenCar3(green3(car(moveRight(RIGHT)))).
car(X,Y,RIGHT) :- greenCar4(green4(car(xpos(X)))), greenCar4(green4(car(ypos(Y)))),greenCar4(green4(car(moveRight(RIGHT)))).

car(X,Y,RIGHT)  :- orangeCar(orange(car(xpos(X)))), orangeCar(orange(car(ypos(Y)))),orangeCar(orange(car(moveRight(RIGHT)))).
car(X,Y,RIGHT) :- orangeCar2(orange2(car(xpos(X)))), orangeCar2(orange2(car(ypos(Y)))),orangeCar2(orange2(car(moveRight(RIGHT)))).

car(X,Y,RIGHT)  :- pinkCar(pink(car(xpos(X)))), pinkCar(pink(car(ypos(Y)))), pinkCar(pink(car(moveRight(RIGHT)))).
car(X,Y,RIGHT) :- pinkCar2(pink2(car(xpos(X)))), pinkCar2(pink2(car(ypos(Y)))), pinkCar2(pink2(car(moveRight(RIGHT)))).

car(X,Y,RIGHT)  :- purpleCar(purple(car(xpos(X)))), purpleCar(purple(car(ypos(Y)))), purpleCar(purple(car(moveRight(RIGHT)))).
car(X,Y,RIGHT) :- purpleCar2(purple2(car(xpos(X)))), purpleCar2(purple2(car(ypos(Y)))), purpleCar2(purple2(car(moveRight(RIGHT)))).

car(X,Y,RIGHT)  :- whiteCar(white(car(xpos(X)))), whiteCar(white(car(ypos(Y)))), whiteCar(white(car(moveRight(RIGHT)))).
car(X,Y,RIGHT) :- whiteCar2(white2(car(xpos(X)))), whiteCar2(white2(car(ypos(Y)))), whiteCar2(white2(car(moveRight(RIGHT)))).

playerPos(X,Y) :- playerSensor(brain(player(xpos(X)))), playerSensor(brain(player(ypos(Y)))).

%carNextPos(1,X,Y1):- carPos(X,Y,true),  

occupied(X,Y) :- xCoord(X), yCoord(Y), car(X,Y,_).

%moveTo(0).
moveTo(A) :- not occupied(X1,Y), playerPos(X2,Y), X1 = X2 + 1, A = 3. %move right
moveTo(A) :- not occupied(X1,Y), playerPos(X2,Y), X2 = X1 + 1, A = 2. %move left
moveTo(A) :- not occupied(X,Y1), playerPos(X,Y2), Y1 = Y2 + 1, A = 1. %move up
moveTo(A) :- not occupied(X,Y1), playerPos(X,Y2), Y2 = Y1 + 1, A = 4. %move down

%stay at the same place
moveTo(A) :- occupied(X,Y1), playerPos(X,Y2), Y1 = Y2 + 1, A = 0.
moveTo(A) :- playerPos(_,Y), Y = 6, A = 0.

best(X) :- #min{A : moveTo(A)} = X, #int(X).
:~ best(X). [X:1]

setOnActuator(player(brain(player(move(A))))) :- best(A).