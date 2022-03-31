ECHO OFF
CLS

IF "%1"=="" GOTO EOF
IF "%1"=="d" GOTO DECOMPILE
IF "%1"=="c" GOTO COMPILE

:DECOMPILE

ECHO Furni Editor is decompiling the SWF.
ECHO Please wait......

swfbinexport temp\%2\%2.swf

GOTO EOF

:COMPILE
ECHO Furni Editor is compiling the SWF.
ECHO Please wait......

swfbinreplace temp\%2\%2.swf %3 temp\%2\%2-%3.bin


GOTO EOF

:EOF