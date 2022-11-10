Archivo: prueba.cpp
Fecha: 08/11/2022 12:46:02 p. m.
#make COM
include 'emu8086.inc'
ORG 100h
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_PRINT_NUM
Variables: 
	area DW ?
	radio DW ?
	pi DW ?
	resultado DW ?
	a DW ?
	d DW ?
	altura DW ?
	cinco DW ?
	x DW ?
	y DW ?
	i DW ?
	j DW ?
	k DW ?
Mov AX, 3
Push AX
Mov AX, 3
Push AX
Pop BX
Pop AX
Cmp AX, BX
JNE if1
PRINT 'Hola'
JMP else1
if1:
PRINT 'Adios'
else1:
RET
