Archivo: prueba.cpp
Fecha: 21/10/2022 09:55:02 a. m.
make COM
include 'emu 8086.inc'
ORG 100h
Variables: 
	area DB ?
	radio DB ?
	pi DB ?
	resultado DB ?
	a DB ?
	d DB ?
	altura DB ?
	x DB ?
	y DB ?
	i DB ?
	j DB ?
Mov AX, 3
Push AX
Mov AX, 5
Push AX
Pop BX
Pop AX
ADD AX, BX
Push AX
Mov AX, 8
Push AX
Pop BX
Pop AX
MUL BX
Push AX
Mov AX, 10
Push AX
Mov AX, 4
Push AX
Pop BX
Pop AX
SUB AX, BX
Push AX
Mov AX, 2
Push AX
Pop BX
Pop AX
DIV BX
Push AX
Pop BX
Pop AX
SUB AX, BX
Push AX
Pop AX
Mov y, AX
RET
END
Pop AX
