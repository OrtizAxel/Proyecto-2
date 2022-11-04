Archivo: prueba.cpp
Fecha: 04/11/2022 09:40:04 a. m.
#make COM
include 'emu8086.inc'
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
	k DW ?
	l DW ?
inicioFor0:
Mov AX, 0
Push AX
Pop AX
Mov i, AX
Mov AX, i
Push AX
Mov AX, 3
Push AX
Pop AX
Pop BX
Cmp AX, BX
JGE 
Inc i
PRINTN "Hola"
Mov AX, i
Push AX
Mov AX, 3
Push AX
Pop AX
Pop BX
Cmp AX, BX
JGE 
Inc i
Mov AX, i
Push AX
Mov AX, 3
Push AX
Pop AX
Pop BX
Cmp AX, BX
JGE 
Inc i
Mov AX, i
Push AX
Mov AX, 3
Push AX
Pop AX
Pop BX
Cmp AX, BX
JGE 
Inc i
finFor0:
