

#maxint=100.


tetrominoConfigurationMaxWidth(0,0,1).
tetrominoConfigurationMaxWidth(0,1,4).
tetrominoConfigurationMaxWidth(1,0,2).
tetrominoConfigurationMaxWidth(1,1,3).
tetrominoConfigurationMaxWidth(1,2,2).
tetrominoConfigurationMaxWidth(1,3,3).
tetrominoConfigurationMaxWidth(2,0,2).
tetrominoConfigurationMaxWidth(2,1,3).
tetrominoConfigurationMaxWidth(2,2,2).
tetrominoConfigurationMaxWidth(2,3,3).
tetrominoConfigurationMaxWidth(3,0,2).
tetrominoConfigurationMaxWidth(4,0,3).
tetrominoConfigurationMaxWidth(4,1,2).
tetrominoConfigurationMaxWidth(5,0,3).
tetrominoConfigurationMaxWidth(5,1,2).
tetrominoConfigurationMaxWidth(6,0,3).
tetrominoConfigurationMaxWidth(6,1,2).
tetrominoConfigurationMaxWidth(6,2,3).
tetrominoConfigurationMaxWidth(6,3,2).

%how many columns occupies a tetromino in the bottom line (if it's not the larger one)
extraBottomSpace(0,0,0,0).
extraBottomSpace(0,1,0,0).
extraBottomSpace(1,0,0,0).
extraBottomSpace(1,1,0,0).
extraBottomSpace(1,2,1,2).
extraBottomSpace(1,3,0,1).
extraBottomSpace(2,0,0,0).
extraBottomSpace(2,1,2,3).
extraBottomSpace(2,2,0,1).
extraBottomSpace(2,3,0,0).
extraBottomSpace(3,0,0,0).
extraBottomSpace(4,0,0,2).
extraBottomSpace(4,1,1,2).
extraBottomSpace(5,0,1,3).
extraBottomSpace(5,1,0,1).
extraBottomSpace(6,0,0,0).
extraBottomSpace(6,1,1,2).
extraBottomSpace(6,2,1,2).
extraBottomSpace(6,3,0,1).

%how many rows occupies the tetromino on the top of the lowest line
extraTopSpace(0,0,3).
extraTopSpace(0,1,0).
extraTopSpace(1,0,2).
extraTopSpace(1,1,1).
extraTopSpace(1,2,2).
extraTopSpace(1,3,1).
extraTopSpace(2,0,2).
extraTopSpace(2,1,1).
extraTopSpace(2,2,2).
extraTopSpace(2,3,1).
extraTopSpace(3,0,1).
extraTopSpace(4,0,1).
extraTopSpace(4,1,2).
extraTopSpace(5,0,1).
extraTopSpace(5,1,2).
extraTopSpace(6,0,1).
extraTopSpace(6,1,2).
extraTopSpace(6,2,1).
extraTopSpace(6,3,2).

leftSpaceWrtSpawn(0,0,0).
leftSpaceWrtSpawn(0,1,0).
leftSpaceWrtSpawn(1,0,0).
leftSpaceWrtSpawn(1,1,0).
leftSpaceWrtSpawn(1,2,1).
leftSpaceWrtSpawn(1,3,2).
leftSpaceWrtSpawn(2,0,1).
leftSpaceWrtSpawn(2,1,0).
leftSpaceWrtSpawn(2,2,0).
leftSpaceWrtSpawn(2,3,2).
leftSpaceWrtSpawn(3,0,0).
leftSpaceWrtSpawn(4,0,1).
leftSpaceWrtSpawn(4,1,0).
leftSpaceWrtSpawn(5,0,0).
leftSpaceWrtSpawn(5,1,0).
leftSpaceWrtSpawn(6,0,1).
leftSpaceWrtSpawn(6,1,0).
leftSpaceWrtSpawn(6,2,1).
leftSpaceWrtSpawn(6,3,1).

extraRow(1,2).
extraRow(2,2).

spawnColumn(4).
myTile(R,C,V):-arenaGrid(arena(arena(tiles(C,R,arenaTile(empty(V)))))).
numOfRows(R2):- #max{R1:myTile(R1,C,V)}=R, R2=R+1.
numOfColumns(C2):- #max{C1:myTile(R1,C1,V)}=C,C2=C+1.

rows(R1):- numOfRows(R),R1>=0,R1<R,#int(R1).
columns(C1):-numOfColumns(C),C1>=0,C1<C,#int(C1).

startingConf(X,0):-spawner(tetrominoSpawner(tetrominoSpawner(lastInstantiated(X)))).

bestSolution(X,Y,C) v a(X,Y,C):-columns(C),tetrominoConfigurationMaxWidth(X,Y,_),startingConf(X,_).
:-#count{Y,C:bestSolution(X,Y,C)}>1.
:-#count{Y,C:bestSolution(X,Y,C)}=0.

allFree(R,C,C1):-myTile(R,C,true),C1=C+1.
allFree(R,C,C2):-allFree(R,C,C1),myTile(R,C1,true),C2=C1+1.
firstEmptyRow(R):-numOfColumns(C), #max{R1:allFree(R1,0,C)}=R.

canPut(R):-bestSolution(X,Y,C),firstEmptyRow(R),tetrominoConfigurationMaxWidth(X,Y,W),allFree(R,C,C1),C1=C+W.
canPut(R):-bestSolution(X,Y,C), canPut(R1),tetrominoConfigurationMaxWidth(X,Y,W),allFree(R,C,C1),C1=C+W, R=R1+1.
freeUpTo(R):-canPut(R), not canPut(R1),R1=R+1.
oneMoreRow(R1):-bestSolution(X,Y,C),freeUpTo(R),allFree(R1,C1,C2), extraBottomSpace(X,Y,I,J),R1=R+1,C1=C+I,C2=C+J.
twoMoreRow(R1):-bestSolution(X,Y,C),oneMoreRow(R),extraRow(X,Y),allFree(R1,C1,C2),extraBottomSpace(X,Y,I,J),R1=R+1,C1=C+I,C2=C+J.

bestRow(R):- freeUpTo(R), not oneMoreRow(R2),extraBottomSpace(X,Y,0,0),bestSolution(X,Y,_),R2=R+1.
bestRow(R1):- freeUpTo(R),R1=R-1, not oneMoreRow(R2),not extraBottomSpace(X,Y,0,0),not extraRow(X,Y),bestSolution(X,Y,_),R2=R+1.
bestRow(R1):- freeUpTo(R),R1=R-2, not oneMoreRow(R2),not extraBottomSpace(X,Y,0,0),extraRow(X,Y),bestSolution(X,Y,_),R2=R+1.
bestRow(R):-oneMoreRow(R), not twoMoreRow(R1),not extraRow(X,Y),bestSolution(X,Y,_),R1=R+1.
bestRow(R):-twoMoreRow(R).

reach(R):-bestSolution(X,Y,_),bestRow(R1),extraTopSpace(X,Y,W),R=R1-W.


hole(R1,C1):-bestSolution(X,Y,C),bestRow(R),R1=R+1, tetrominoConfigurationMaxWidth(X,Y,W), myTile(R1,C1,true),C1>=C,C1<W1,W1=C+W.
hole(R,C1):-bestSolution(X,Y,C),extraBottomSpace(X,Y,I,J),L=I+J,L>0,oneMoreRow(R),myTile(R,C1,true),C1>=C,C1<C2,C2=C+I.
hole(R,C1):-bestSolution(X,Y,C),extraBottomSpace(X,Y,I,J),L=I+J,L>0, oneMoreRow(R), myTile(R,C1,true), tetrominoConfigurationMaxWidth(X,Y,W),C1>=C2, C2=C+J, C1<C3,C3=C+W.
hole(R,C1):-bestSolution(X,Y,C),extraBottomSpace(X,Y,I,J),L=I+J,L>0,twoMoreRow(R),myTile(R,C1,true),C1>=C,C1<C2,C2=C+I.
hole(R,C1):-bestSolution(X,Y,C),extraBottomSpace(X,Y,I,J),L=I+J,L>0, twoMoreRow(R), myTile(R,C1,true), tetrominoConfigurationMaxWidth(X,Y,W),C1>=C2, C2=C+J, C1<C3,C3=C+W.


:-#count{R:bestRow(R)}=0.
:~ #count{R,C:hole(R,C)}=N, #int(N1),#int(N),N1=3*N. [N1:4]
:~ bestRow(R),numOfRows(N),D=N-R. [D:4]
:~ reach(R),numOfRows(N),D=N-R.  [D:3]
:~ bestSolution(X,Y,C). [C:2]
:~ bestSolution(X,Y,C). [Y:1]


setOnActuator(player(aI(assetsScriptsAIPlayer(aiProgressive(X))))):-spawner(tetrominoSpawner(tetrominoSpawner(progressiveNumber(X)))).
setOnActuator(player(aI(assetsScriptsAIPlayer(numOfMove(X))))):-setOnActuator(player(aI(assetsScriptsAIPlayer(numOfLateralMove(N))))),setOnActuator(player(aI(assetsScriptsAIPlayer(numOfRotation(N1))))),X= N+N1.
setOnActuator(player(aI(assetsScriptsAIPlayer(numOfLateralMove(N))))):-bestSolution(X,Y,C),spawnColumn(S),leftSpaceWrtSpawn(X,Y,L), N=S-D,D=C+L,D<S.
setOnActuator(player(aI(assetsScriptsAIPlayer(numOfLateralMove(N))))):-bestSolution(X,Y,C),spawnColumn(S),leftSpaceWrtSpawn(X,Y,L), N=D-S,D=C+L,D>=S.
setOnActuator(player(aI(assetsScriptsAIPlayer(numOfRotation(N))))):-bestSolution(_,N,_).
setOnActuator(player(aI(assetsScriptsAIPlayer(typeOfLateralMove(left))))):-bestSolution(X,Y,C),spawnColumn(S),D=C+L,D<S,leftSpaceWrtSpawn(X,Y,L).
setOnActuator(player(aI(assetsScriptsAIPlayer(typeOfLateralMove(right))))):-bestSolution(X,Y,C), spawnColumn(S),D=C+L,D>=S,leftSpaceWrtSpawn(X,Y,L).
