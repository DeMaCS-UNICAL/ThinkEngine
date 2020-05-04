%% INPUT %%
% next(left) | next(right) | next(up) | next(down).
% pellet(X,Y).
% pacman(X,Y).
% ghost(X,Y, name).
% tile(X,Y).
% closestPellet(X,Y).
% distanceClosestPellet(X,Y).
% previous_action(X). %% left, right, up, down
% min_distance(Xp,Yp,Xg,Yg,D).
% distance/1


distance(1..10).

nextCell(X,Y) :- pacman(Px, Y), next(right), X=Px+1, tile(X,Y).
nextCell(X,Y) :- pacman(Px, Y), next(left), X=Px-1, tile(X,Y).
nextCell(X,Y) :- pacman(X, Py), next(up), Y=Py+1, tile(X,Y).
nextCell(X,Y) :- pacman(X, Py), next(down), Y=Py-1, tile(X,Y).

empty(X,Y) :- tile(X,Y), not pellet(X,Y).

distancePacmanNextGhost(D, G) :- nextCell(Xp, Yp), ghost(Xg, Yg, G),
                                    min_distance(Xp,Yp,Xg,Yg,D).
minDistancePacmanNextGhost(MD) :- #min{D : distancePacmanNextGhost(D, _)} = MD, distance(MD).

:~ minDistancePacmanNextGhost(MD), Min=10-MD, not powerup. [Min@4,MD]
:~ minDistancePacmanNextGhost(MD), powerup. [MD@4]

:~ nextCell(X,Y), empty(X,Y). [1@3,X,Y]
:~ nextCell(X,Y), closestPellet(X,Y), distanceClosestPellet(D). [D@2,X,Y]
:~ previous_action(X), next(Y), X!=Y. [1@1,X,Y]
