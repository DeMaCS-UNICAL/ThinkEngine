%grid(grid(grid(grid(X,Y,gridSpace(player(V)))))).
%gameController(gameController(gameController(playerToPlay(X)))).

#maxint = 9.

player(x). player(o).

ai(A) :- gameController(gameController(gameController(playerToPlay(A)))).
opponent(O) :- player(O), not ai(O).

tile(X, Y, P, 0) :- grid(grid(grid(grid(X,Y,gridSpace(player(P)))))).

possibleMove(X, Y) :- tile(X, Y, empty, 0).

move(X, Y) | out(X, Y) :- possibleMove(X, Y).
:- #count{X, Y: move(X, Y)} > 1.
:- #count{X, Y: move(X, Y)} < 1.


tile(X, Y, P, 1) :- move(X, Y), ai(P).
tile(X, Y, P, 1) :- tile(X, Y, P, 0), not move(X, Y).


% make winninig move if possible
:~ tile(X, 0, Player, 1), tile(X, 1, Player, 1), tile(X, 2, empty, 1), tile(X, 0, Player, 0), tile(X, 1, Player, 0), ai(Player). [1:9] 
:~ tile(X, 0, Player, 1), tile(X, 1, empty, 1), tile(X, 2, Player, 1), tile(X, 0, Player, 0), tile(X, 2, Player, 0), ai(Player). [1:9] 
:~ tile(X, 0, empty, 1), tile(X, 1, Player, 1), tile(X, 2, Player, 1), tile(X, 1, Player, 0), tile(X, 2, Player, 0), ai(Player). [1:9]

:~ tile(0, Y, Player, 1), tile(1, Y, Player, 1), tile(2, Y, empty, 1), tile(0, Y, Player, 0), tile(1, Y, Player, 0), ai(Player). [1:9] 
:~ tile(0, Y, Player, 1), tile(1, Y, empty, 1), tile(2, Y, Player, 1), tile(0, Y, Player, 0), tile(2, Y, Player, 0), ai(Player). [1:9] 
:~ tile(0, Y, empty, 1), tile(1, Y, Player, 1), tile(2, Y, Player, 1), tile(1, Y, Player, 0), tile(2, Y, Player, 0), ai(Player). [1:9]

:~ tile(0, 0, Player, 1), tile(1, 1, Player, 1), tile(2, 2, empty, 1), tile(0, 0, Player, 0), tile(1, 1, Player, 0), ai(Player). [1:9]
:~ tile(0, 0, Player, 1), tile(1, 1, empty, 1), tile(2, 2, Player, 1), tile(0, 0, Player, 0), tile(2, 2, Player, 0), ai(Player). [1:9]
:~ tile(0, 0, empty, 1), tile(1, 1, Player, 1), tile(2, 2, Player, 1), tile(1, 1, Player, 0), tile(2, 2, Player, 0), ai(Player). [1:9]

:~ tile(0, 2, Player, 1), tile(1, 1, Player, 1), tile(2, 0, empty, 1), tile(0, 2, Player, 0), tile(1, 1, Player, 0), ai(Player). [1:9]
:~ tile(0, 2, Player, 1), tile(1, 1, empty, 1), tile(2, 0, Player, 1), tile(0, 2, Player, 0), tile(2, 0, Player, 0), ai(Player). [1:9]
:~ tile(0, 2, empty, 1), tile(1, 1, Player, 1), tile(2, 0, Player, 1), tile(1, 1, Player, 0), tile(2, 0, Player, 0), ai(Player). [1:9]


% prevent opponent make winninig move
:~ tile(X, 0, Opponent, 1), tile(X, 1, Opponent, 1), tile(X, 2, empty, 1), opponent(Opponent). [1:8] 
:~ tile(X, 0, Opponent, 1), tile(X, 1, empty, 1), tile(X, 2, Opponent, 1), opponent(Opponent). [1:8] 
:~ tile(X, 0, empty, 1), tile(X, 1, Opponent, 1), tile(X, 2, Opponent, 1), opponent(Opponent). [1:8]

:~ tile(0, Y, Opponent, 1), tile(1, Y, Opponent, 1), tile(2, Y, empty, 1), opponent(Opponent). [1:8] 
:~ tile(0, Y, Opponent, 1), tile(1, Y, empty, 1), tile(2, Y, Opponent, 1), opponent(Opponent). [1:8] 
:~ tile(0, Y, empty, 1), tile(1, Y, Opponent, 1), tile(2, Y, Opponent, 1), opponent(Opponent). [1:8]

:~ tile(0, 0, Opponent, 1), tile(1, 1, Opponent, 1), tile(2, 2, empty, 1), opponent(Opponent). [1:8]
:~ tile(0, 0, Opponent, 1), tile(1, 1, empty, 1), tile(2, 2, Opponent, 1), opponent(Opponent). [1:8]
:~ tile(0, 0, empty, 1), tile(1, 1, Opponent, 1), tile(2, 2, Opponent, 1), opponent(Opponent). [1:8]

:~ tile(0, 2, Opponent, 1), tile(1, 1, Opponent, 1), tile(2, 0, empty, 1), opponent(Opponent). [1:8]
:~ tile(0, 2, Opponent, 1), tile(1, 1, empty, 1), tile(2, 0, Opponent, 1), opponent(Opponent). [1:8]
:~ tile(0, 2, empty, 1), tile(1, 1, Opponent, 1), tile(2, 0, Opponent, 1), opponent(Opponent). [1:8]


% forks
:~ tile(0, 0, Player, 0), tile(2, 2, Player, 0), tile(1, 0, empty, 0), tile(2, 0, empty, 0), tile(2, 1, empty, 0), tile(2, 0, empty, 1), ai(Player). [1:7]
:~ tile(0, 0, Player, 0), tile(2, 2, Player, 0), tile(0, 1, empty, 0), tile(0, 2, empty, 0), tile(1, 2, empty, 0), tile(0, 2, empty, 1), ai(Player). [1:7]
:~ tile(0, 2, Player, 0), tile(2, 0, Player, 0), tile(0, 0, empty, 0), tile(0, 1, empty, 0), tile(1, 0, empty, 0), tile(0, 0, empty, 1), ai(Player). [1:7]
:~ tile(0, 2, Player, 0), tile(2, 0, Player, 0), tile(2, 2, empty, 0), tile(2, 1, empty, 0), tile(1, 2, empty, 0), tile(2, 2, empty, 1), ai(Player). [1:7]

:~ tile(0, 0, empty, 0), tile(0, 1, Player, 0), tile(1, 0, Player, 0), tile(0, 2, empty, 0), tile(2, 0, empty, 0), tile(0, 0, empty, 1), ai(Player). [1:7]
:~ tile(0, 2, empty, 0), tile(0, 1, Player, 0), tile(1, 2, Player, 0), tile(0, 0, empty, 0), tile(2, 2, empty, 0), tile(0, 2, empty, 1), ai(Player). [1:7]
:~ tile(2, 2, empty, 0), tile(1, 2, Player, 0), tile(2, 1, Player, 0), tile(0, 2, empty, 0), tile(2, 0, empty, 0), tile(2, 2, empty, 1), ai(Player). [1:7]
:~ tile(2, 0, empty, 0), tile(1, 0, Player, 0), tile(2, 1, Player, 0), tile(0, 0, empty, 0), tile(2, 2, empty, 0), tile(2, 0, empty, 1), ai(Player). [1:7]


% prevent forks
:~ tile(0, 0, Opponent, 0), tile(2, 2, Opponent, 0), tile(1, 1, Player, 0), #count{X, Y: tile(X, Y, empty, 0)} = 6, tile(0, 2, Player, 1), ai(Player), opponent(Opponent). [1:5]
:~ tile(0, 0, Opponent, 0), tile(2, 2, Opponent, 0), tile(1, 1, Player, 0), #count{X, Y: tile(X, Y, empty, 0)} = 6, tile(2, 0, Player, 1), ai(Player), opponent(Opponent). [1:5]
:~ tile(0, 2, Opponent, 0), tile(2, 0, Opponent, 0), tile(1, 1, Player, 0), #count{X, Y: tile(X, Y, empty, 0)} = 6, tile(0, 0, Player, 1), ai(Player), opponent(Opponent). [1:5]
:~ tile(0, 2, Opponent, 0), tile(2, 0, Opponent, 0), tile(1, 1, Player, 0), #count{X, Y: tile(X, Y, empty, 0)} = 6, tile(2, 2, Player, 1), ai(Player), opponent(Opponent). [1:5]

:~ tile(0, 0, Opponent, 0), tile(2, 2, Opponent, 0), tile(0, 1, empty, 0), tile(0, 2, empty, 0), tile(1, 2, empty, 0), tile(0, 2, empty, 1), opponent(Opponent). [1:4]
:~ tile(0, 0, Opponent, 0), tile(2, 2, Opponent, 0), tile(1, 0, empty, 0), tile(2, 0, empty, 0), tile(2, 1, empty, 0), tile(2, 0, empty, 1), opponent(Opponent). [1:4]
:~ tile(0, 2, Opponent, 0), tile(2, 0, Opponent, 0), tile(0, 0, empty, 0), tile(0, 1, empty, 0), tile(1, 0, empty, 0), tile(0, 0, empty, 1), opponent(Opponent). [1:4]
:~ tile(0, 2, Opponent, 0), tile(2, 0, Opponent, 0), tile(2, 2, empty, 0), tile(2, 1, empty, 0), tile(1, 2, empty, 0), tile(2, 2, empty, 1), opponent(Opponent). [1:4]

:~ tile(0, 0, empty, 0), tile(0, 1, Opponent, 0), tile(1, 0, Opponent, 0), tile(0, 2, empty, 0), tile(2, 0, empty, 0), tile(0, 0, empty, 1), opponent(Opponent). [1:4]
:~ tile(0, 2, empty, 0), tile(0, 1, Opponent, 0), tile(1, 2, Opponent, 0), tile(0, 0, empty, 0), tile(2, 2, empty, 0), tile(0, 2, empty, 1), opponent(Opponent). [1:4]
:~ tile(2, 2, empty, 0), tile(1, 2, Opponent, 0), tile(2, 1, Opponent, 0), tile(0, 2, empty, 0), tile(2, 0, empty, 0), tile(2, 2, empty, 1), opponent(Opponent). [1:4]
:~ tile(2, 0, empty, 0), tile(1, 0, Opponent, 0), tile(2, 1, Opponent, 0), tile(0, 0, empty, 0), tile(2, 2, empty, 0), tile(2, 0, empty, 1), opponent(Opponent). [1:4]


% lines of 2
:~ tile(0, 0, Player, 0), tile(0, 1, empty, 0), tile(0, 2, empty, 0), tile(0, 2, empty, 1), ai(Player). [1:3]
:~ tile(0, 0, empty, 0), tile(0, 1, empty, 0), tile(0, 2, Player, 0), tile(0, 0, empty, 1), ai(Player). [1:3]
:~ tile(2, 0, Player, 0), tile(2, 1, empty, 0), tile(2, 2, empty, 0), tile(2, 2, empty, 1), ai(Player). [1:3]
:~ tile(2, 0, empty, 0), tile(2, 1, empty, 0), tile(2, 2, Player, 0), tile(2, 0, empty, 1), ai(Player). [1:3]

:~ tile(0, 0, Player, 0), tile(1, 0, empty, 0), tile(2, 0, empty, 0), tile(2, 0, empty, 1), ai(Player). [1:3]
:~ tile(0, 0, empty, 0), tile(1, 0, empty, 0), tile(2, 0, Player, 0), tile(0, 0, empty, 1), ai(Player). [1:3]
:~ tile(0, 2, Player, 0), tile(1, 2, empty, 0), tile(2, 2, empty, 0), tile(2, 2, empty, 1), ai(Player). [1:3]
:~ tile(0, 2, empty, 0), tile(1, 2, empty, 0), tile(2, 2, Player, 0), tile(0, 2, empty, 1), ai(Player). [1:3]


% start at corner
:~ tile(0, 0, empty, 1), #count{X, Y : tile(X, Y, empty, 0)} > 8. [1:3]
:~ tile(0, 2, empty, 1), #count{X, Y : tile(X, Y, empty, 0)} > 8. [1:3]
:~ tile(2, 0, empty, 1), #count{X, Y : tile(X, Y, empty, 0)} > 8. [1:3]
:~ tile(2, 2, empty, 1), #count{X, Y : tile(X, Y, empty, 0)} > 8. [1:3]


% play center
:~ tile(1, 1, empty, 0), tile(1, 1, empty, 1). [1:3]


% opposite corner
:~ tile(0, 0, Opponent, 1), tile(2, 2, empty, 1), opponent(Opponent). [1:2]
:~ tile(0, 2, Opponent, 1), tile(2, 0, empty, 1), opponent(Opponent). [1:2]
:~ tile(2, 0, Opponent, 1), tile(0, 2, empty, 1), opponent(Opponent). [1:2]
:~ tile(2, 2, Opponent, 1), tile(0, 0, empty, 1), opponent(Opponent). [1:2]


% corner
:~ tile(0, 0, empty, 0), tile(0, 0, empty, 1). [1:1]
:~ tile(0, 2, empty, 0), tile(0, 2, empty, 1). [1:1]
:~ tile(2, 0, empty, 0), tile(2, 0, empty, 1). [1:1]
:~ tile(2, 2, empty, 0), tile(2, 2, empty, 1). [1:1]




setOnActuator(actuator(iA(iA(x(X))))):- move(X, _).
setOnActuator(actuator(iA(iA(y(Y))))):- move(_, Y).