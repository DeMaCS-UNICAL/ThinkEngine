setOnActuator(nextMove(pacman(playerController(nextStep(X))))):-next(X).
%blinky(blinky(positionToInt(x(X)))).
%blinky(blinky(positionToInt(y(X)))).
%clyde(clyde(positionToInt(x(X)))).
%clyde(clyde(positionToInt(y(X)))).
%inky(inky(positionToInt(x(X)))).
%inky(inky(positionToInt(y(X)))).
%pinky(pinky(positionToInt(x(X)))).
%pinky(pinky(positionToInt(y(X)))).
%pacman(pacman(positionToInt(x(X)))).
%pacman(pacman(positionToInt(y(X)))).
%extra(brain(aISupportScript(closestPelletX(X)))).
%extra(brain(aISupportScript(closestPelletY(X)))).
%extra(brain(aISupportScript(distanceClosestPellet(X)))).
%extra(brain(aISupportScript(powerup(X)))).
%extra(brain(aISupportScript(distances(X,distance(x1(V)))))).
%extra(brain(aISupportScript(distances(X,distance(y1(V)))))).
%extra(brain(aISupportScript(distances(X,distance(x2(V)))))).
%extra(brain(aISupportScript(distances(X,distance(y2(V)))))).
%extra(brain(aISupportScript(distances(X,distance(distance(V)))))).
%tilesAndPacdots(pacdots(mazeTiles(tiles(X,Y,myTile(pacdot(V)))))).
%tilesAndPacdots(pacdots(mazeTiles(tiles(X,Y,myTile(occupied(V)))))).
%tilesAndPacdots(pacdots(mazeTiles(tiles(X,Y,myTile(x(V)))))).
%tilesAndPacdots(pacdots(mazeTiles(tiles(X,Y,myTile(y(V)))))).
%previousMove(pacman(playerController(nextStep(X)))).

%% INPUT %%

% pellet(X,Y).
% pacman(X,Y).
% ghost(X,Y, name).
% tile(X,Y).
% closestPellet(X,Y).
% distanceClosestPellet(X,Y).
% previous_action(X). %% left, right, up, down
% min_distance(Xp,Yp,Xg,Yg,D).
% distance/1

pellet(X,Y):-tiles(brain(aISupportScript(neededTiles(Z,myTile(pacdot(true)))))),tiles(brain(aISupportScript(neededTiles(Z,myTile(x(X)))))),tiles(brain(aISupportScript(neededTiles(Z,myTile(y(Y)))))).
pacman2(X,Y):-pacman(pacman(positionToInt(x(X)))),pacman(pacman(positionToInt(y(Y)))).
ghost(X,Y, blinky):-blinky(blinky(positionToInt(x(X)))),blinky(blinky(positionToInt(y(Y)))).
ghost(X,Y, inky):-inky(inky(positionToInt(x(X)))),inky(inky(positionToInt(y(Y)))).
ghost(X,Y, pinky):-pinky(pinky(positionToInt(x(X)))),pinky(pinky(positionToInt(y(Y)))).
ghost(X,Y, clyde):-clyde(clyde(positionToInt(x(X)))),clyde(clyde(positionToInt(y(Y)))).
tile(X,Y):-tiles(brain(aISupportScript(neededTiles(Z,myTile(occupied(false)))))),tiles(brain(aISupportScript(neededTiles(Z,myTile(x(X)))))),tiles(brain(aISupportScript(neededTiles(Z,myTile(y(Y)))))).
closestPellet(X,Y):-extra(brain(aISupportScript(closestPelletX(X)))),extra(brain(aISupportScript(closestPelletY(Y)))).
distanceClosestPellet(X):-extra(brain(aISupportScript(distanceClosestPellet(X)))).
previous_action(X):-previousMove(pacman(playerController(nextStep(X)))). %% left, right, up, down
pre_previous_action(X):-prePreviousMove(pacman(playerController(prePreviousStep(X)))).
min_distance(Xp,Yp,Xq,Yq,D):-extra(brain(aISupportScript(distances(X,distance(x1(Xp)))))),extra(brain(aISupportScript(distances(X,distance(y1(Yp)))))),extra(brain(aISupportScript(distances(X,distance(x2(Xq)))))),extra(brain(aISupportScript(distances(X,distance(y2(Yq)))))),extra(brain(aISupportScript(distances(X,distance(distance(D)))))).
powerup:-extra(brain(aISupportScript(powerup(true)))).
sameAxis(right,left).
sameAxis(up,down).
sameAxis(Y,X):-sameAxis(X,Y).
distance(1..10).
next(left) | next(right) | next(up) | next(down).

nextCell(X,Y) :- pacman2(Px, Y), next(right), X=Px+1, tile(X,Y).
nextCell(X,Y) :- pacman2(Px, Y), next(left), X=Px-1, tile(X,Y).
nextCell(X,Y) :- pacman2(X, Py), next(up), Y=Py+1, tile(X,Y).
nextCell(X,Y) :- pacman2(X, Py), next(down), Y=Py-1, tile(X,Y).

valid(right) :- pacman2(Px, Y), next(right), X=Px+1, tile(X,Y).
valid(left) :- pacman2(Px, Y), next(left), X=Px-1, tile(X,Y).
valid(up) :- pacman2(X, Py), next(up), Y=Py+1, tile(X,Y).
valid(down) :- pacman2(X, Py), next(down), Y=Py-1, tile(X,Y).

:-not valid(X),next(X).
empty(X,Y) :- tile(X,Y), not pellet(X,Y).

distancePacmanNextGhost(D, G) :- nextCell(Xp, Yp), ghost(Xg, Yg, G),
                                    min_distance(Xp,Yp,Xg,Yg,D).
minDistancePacmanNextGhost(MD) :- #min{D : distancePacmanNextGhost(D, _)} = MD, distance(MD).

:~ minDistancePacmanNextGhost(MD), Min=10-MD, not powerup. [Min:5]
:~ minDistancePacmanNextGhost(MD), powerup. [MD:5]
:~ nextCell(X,Y), empty(X,Y). [1:4]
:~ closestPellet(X,Y), not nextCell(X,Y). [1:3]
:~ nextCell(X,Y), closestPellet(X1,Y1), min_distance(X,Y,X1,Y1,D), D<3. [D:1]%distanceClosestPellet(D). [D:2]
:~ previous_action(X), next(Y), X!=Y. [1:2]
:~ previous_action(X), next(Y), sameAxis(X,Y). [3:1]

