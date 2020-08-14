% setOnActuator(column(muovi(aggiornaK(k(X))))):-
% setOnActuator(player(player(player(posX(X))))):-
% setOnActuator(player(player(player(posY(X))))):-
%ground(gameController(grid(ground(X,Y,cell(x(V)))))).
%ground(gameController(grid(ground(X,Y,cell(y(V)))))).
%ground(gameController(grid(ground(X,Y,cell(content(V)))))).



ground(gameController(grid(ground(1,2,cell(content(3)))))).
ground(gameController(grid(ground(4,5,cell(content(6)))))).
ground(gameController(grid(ground(3,1,cell(content(2)))))).
% cella(X,Y,V) :- ground(gameController(grid(ground(X,Y,cell(content(V)))))).
% maxColonna(C) :- cella(C,_,_).
% setOnActuator(column(muovi(aggiornaK(k(X))))) :- maxColonna(X).

setOnActuator(column(muovi(aggiornaK(k(X))))):-ground(gameController(grid(ground(X,Y,cell(content(V)))))).