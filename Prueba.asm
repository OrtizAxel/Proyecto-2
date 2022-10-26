Archivo: prueba.cpp
Fecha: 25/10/2022 09:48:11 a. m.
make COM
include 'emu 8086.inc'
ORG 100h
Variables: 
	area DW ?
	radio DW ?
	pi DW ?
	resultado DW ?
	a DW ?
	d DW ?
	altura DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
Mov AX, 61
Push AX
Pop AX
Mov y, AX
Mov AX, 60
Push AX
Mov AX, 61
Push AX
Pop AX
Pop BX
Cmp AX, BX
JNE, if1
Mov AX, 10
Push AX
Pop AX
Mov x, AX
if1:
RET
END
