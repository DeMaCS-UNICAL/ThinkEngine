number(1..3).

adjacent(X1,Y1,X2,Y2) :- tile(X1,Y1), tile(X2,Y2), step(DX,DY), X2 = X1 + DX, Y2 = Y1 + DY.
adjacent(X1,Y1,X2,Y2) :- tile(X1,Y1), tile(X2,Y2), stepN(DX,DY), X2 = X1 - DX, Y2 = Y1 - DY.
step(0,1).
stepN(0,1).
step(1,0).
stepN(1,0).

distance(X1,Y1,X2,Y2,1) :- tile(X1,Y1), adjacent(X1,Y1,X2,Y2).
distance(X1,Y1,X3,Y3,Dp1) :- distance(X1,Y1,X2,Y2,D), adjacent(X2,Y2,X3,Y3), D = Dp1 - 1, number(Dp1).

min_distance(X1,Y1,X2,Y2,MD) :- #min{D : distance(X1,Y1,X2,Y2,D)} = MD, tile(X1,Y1), tile(X2,Y2).