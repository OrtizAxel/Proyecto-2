Archivo: prueba.cpp
Fecha: 20/10/2022 09:58:34 a. m.
make COM
include 'emu 8086.inc'
ORG 100h
Mov AX, 3
Push AX
Mov AX, 5
Push AX
Pop AX
Pop BX
ADD AX, BX
Push AX
Mov AX, 8
Push AX
Pop AX
Pop BX
MUL AX, BX
Push AX
Mov AX, 10
Push AX
Mov AX, 4
Push AX
Pop AX
Pop BX
SUB AX, BX
Push AX
Mov AX, 2
Push AX
Pop AX
Pop BX
DIV AX, BX
Push AX
Pop AX
Pop BX
SUB AX, BX
Push AX
Pop AX
RET
END
Pop AX
