SET ZIPPER="C:\Program Files\7-Zip\7z.exe"
del "C:\Users\Denise\Uni\Assegno\ThinkEngine\ThinkEnginePlugin.zip"
%ZIPPER% a -tzip -bd -mx=9 -r -y "C:\Users\Denise\Uni\Assegno\ThinkEngine\ThinkEnginePlugin.zip"  "C:\Users\Denise\Uni\Assegno\ThinkEngine\bin\Debug\ThinkEnginePlugin\*"