//Axel Ortiz Ricalde
using System;
using System.Collections.Generic;
//Requerimiento 1: Actualizacion:
//                 a)agregar el residuo de la division en porfactor (Hecho)
//                 b)agregar en instruccion los incrementos de termino y los incrementos de factor
//                   a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1
//                   en donde el uno puede ser una expresion (Hecho)
//                 c)programar el destructor para ejecutar el metodo cerrarArchivo()
//                  #libreria especial
//Requerimiento 2:    
//                 a)Marcar errores semanticos cuando los incrementos de termino o incrementos de factor
//                   superen el rango de la variable  (Hecho)
//                 b)Considerar el inciso b y c del requerimiento 1 para el for
//                 c)Que funcione el do while y el while (Hecho)
//Requerimiento 3:
//                 a)Considerar las variables y los casteos de las expresiones matematicas en ensamblador (Hecho)
//                 b)Considerar el residuo de la division en ensamblador   (Hecho)
//                 c)Programar el printf y el scanf en ensamblador (Hecho)
//Requerimiento 4: 
//                 a)Programar el else en ensamblador (Hecho)
//                 b)Programar el for en ensamblador
//Requerimiento 5:
//                 a)Programar el while en ensamblador
//                 b)Programar el do while en ensamblador


namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List <Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        int cIf;
        int cFor;

        public Lenguaje()
        {
            cIf = cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFor = 0;
        }

        ~Lenguaje()
        {
            Console.WriteLine("Destructor");
            cerrar();
        }

        private void addVariable(String nombre,Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }

        private void displayVariables()
        {
            log.WriteLine("Variables: ");
            foreach(Variable v in variables)
            {
                log.WriteLine(v.getNombre()+" "+v.getTipo()+" "+v.getValor());
            }
        }

        private void variablesAsm()
        {
            asm.WriteLine("Variables: ");
            foreach(Variable v in variables)
            {
                asm.WriteLine("\t" + v.getNombre()+" DW ?");
            }
        }

        private bool existeVariable(string nombre)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }

        private void modVariable(string nombre, float nuevoValor)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        
        private float convert(float valor, Variable.TipoDato tipo)
        {
            if(dominante == Variable.TipoDato.Char)
            {
                valor = (char)(valor)%256;
                return valor;
            } else if(dominante == Variable.TipoDato.Int)
            {
                valor = (int)(valor)%65536;
                return valor;
            }
            else
            {
                return valor;
            }
        }

        private float getValor(string nombreVariable)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombreVariable))
                {
                    return v.getValor();
                }
            }
            return 0;
        }

        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombreVariable))
                {
                    return v.getTipo();
                }
            }
            return Variable.TipoDato.Char;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make COM");
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            variablesAsm();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if(getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if(getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

         //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if(getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char; 
                switch (getContenido())
                {
                    case "int": tipo = Variable.TipoDato.Int; break;
                    case "float": tipo = Variable.TipoDato.Float; break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

         //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if(getClasificacion() == Tipos.Identificador)
            {
                if(!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" +getContenido()+"> en linea: "+linea, log);
                }
            }
            match(Tipos.Identificador);
            if(getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        
        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true, true);
        }

        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool evaluaAsm)
        {
            match("{");
            if(getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, evaluaAsm);
            }    
            match("}"); 
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool evaluaAsm)
        {
            Instruccion(evaluacion, evaluaAsm);
            if(getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, evaluaAsm);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool evaluaAsm)
        {
            Instruccion(evaluacion, evaluaAsm);
            if(getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, evaluaAsm);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool evaluaAsm)
        {
            if(getContenido() == "printf")
            {
                Printf(evaluacion, evaluaAsm);
            }
            else if(getContenido() == "scanf")
            {
                Scanf(evaluacion, evaluaAsm);
            }
            else if(getContenido() == "if")
            {
                If(evaluacion, evaluaAsm);
            }
            else if(getContenido() == "while")
            {
                While(evaluacion, evaluaAsm);
            }
            else if(getContenido() == "do")
            {
                Do(evaluacion, evaluaAsm);
            }
            else if(getContenido() == "for")
            {
                For(evaluacion, evaluaAsm);
            }
            else if(getContenido() == "switch")
            {
                Switch(evaluacion, evaluaAsm);
            }
            else
            {
                Asignacion(evaluacion, evaluaAsm);
            }
        }

        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if(resultado%1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if(resultado<=255)
            {
                return Variable.TipoDato.Char;
            }
            else if(resultado<=65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }

        private bool evaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            return false;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion, bool evaluaAsm)
        {
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
            }
            log.WriteLine();
            log.Write(getContenido() + " = ");
            string nombre = getContenido();
            match(Tipos.Identificador); 
            dominante = Variable.TipoDato.Char;
            if(getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                float resultado = Incremento(nombre);
                match(";");
                log.Write("= " + resultado);
                log.WriteLine();
                if(dominante < evaluaNumero(resultado))
                {
                    dominante = evaluaNumero(resultado);
                }
                if(dominante <= getTipo(nombre))
                {
                    if(evaluacion)
                    {
                        modVariable(nombre, resultado);
                    }
                }
                else
                {
                    throw new Error("Error de semantica, no podemos asignar un <" + dominante + "> a un <" + getTipo(nombre) + "> en la linea " + linea, log);
                }
                if(evaluaAsm)
                {
                    asm.WriteLine("Mov " + nombre + ", AX");
                }
            }
            else
            {
                match(Tipos.Asignacion);
                Expresion();
                match(";");
                float resultado = stack.Pop();
                if(evaluaAsm)
                {
                    asm.WriteLine("Pop AX");
                }
                log.Write("= " + resultado);
                log.WriteLine();
                if(dominante < evaluaNumero(resultado))
                {
                    dominante = evaluaNumero(resultado);
                }
                if(dominante <= getTipo(nombre))
                {
                    if(evaluacion)
                    {
                        modVariable(nombre, resultado);
                    }
                }
                else
                {
                    throw new Error("Error de semantica, no podemos asignar un <" + dominante + "> a un <" + getTipo(nombre) + "> en la linea " + linea, log);
                }
                if(evaluaAsm)
                {
                    asm.WriteLine("Mov " + nombre + ", AX");
                }
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool evaluaAsm)
        {
            match("while");
            match("(");
            string variable = getContenido();
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
            bool validarWhile;
            int pos = posicion;
            int lin = linea;
            do
            {
                validarWhile = Condicion("");            
                if(!evaluacion)
                {
                    validarWhile = false;
                }
                match(")");
                if(getContenido() == "{") 
                {
                    BloqueInstrucciones(validarWhile, evaluaAsm);
                }
                else
                {
                    Instruccion(validarWhile, evaluaAsm);
                }
                if(validarWhile)
                {
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
            }while(validarWhile);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool evaluaAsm)
        {
            match("do");
            bool validarDo = evaluacion;
            int pos = posicion;
            int lin = linea;
            do
            {
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(validarDo, evaluaAsm);
                }
                else
                {
                    Instruccion(validarDo, evaluaAsm);
                } 
                match("while");
                match("(");
                string variable = getContenido();
                if(!existeVariable(getContenido()))
                    throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
                validarDo = Condicion("");
                if(!evaluacion)
                {
                    validarDo = false;
                }
                match(")");
                match(";");
                if(validarDo)
                {
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
            }while(validarDo);
            
        }

        public void setPosicion(long posicion)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(posicion, SeekOrigin.Begin);
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool evaluaAsm)
        {
            string etiquetaInicioFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor++;
            asm.WriteLine(etiquetaInicioFor + ":");
            match("for");
            match("(");
            Asignacion(evaluacion, evaluaAsm);
            string variable = getContenido();
            if(!existeVariable(getContenido()))
                throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
            bool validarFor;
            int pos = posicion;
            int lin = linea;
            do
            {
                validarFor = Condicion("");
                if(!evaluacion)
                {
                    validarFor = false;
                }
                match(";");
                match(Tipos.Identificador);
                float resultado = Incremento(variable);
                match(")");
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor, evaluaAsm);  
                }
                else
                {
                    Instruccion(validarFor, evaluaAsm);
                }
                if(validarFor)
                {
                    posicion = pos - variable.Length;
                    linea = lin;
                    setPosicion(posicion);
                    NextToken();
                }
                if(evaluacion)
                {
                    modVariable(variable, resultado);
                }
            }while(validarFor);
            asm.WriteLine(etiquetaFinFor + ":");
        }

        private float Incremento(string nombre)
        {
            float resultado = getValor(nombre);
            switch(getContenido())
            {
                case "++":
                    match(Tipos.IncrementoTermino);
                    resultado++;
                    asm.WriteLine("Inc " + nombre);
                    break;
                case "--":
                    match(Tipos.IncrementoTermino);
                    asm.WriteLine("Dec " + nombre);
                    resultado--;
                    break;
                case "+=":
                    match(Tipos.IncrementoTermino);
                    Expresion();
                    resultado += stack.Pop();
                    asm.WriteLine("Pop AX");
                    break;
                case "-=":
                    match(Tipos.IncrementoTermino);
                    Expresion();
                    resultado -= stack.Pop();
                    break;
                case "*=":
                    match(Tipos.IncrementoFactor);
                    Expresion();
                    resultado *= stack.Pop();
                    break;
                case "/=":
                    match(Tipos.IncrementoFactor);
                    Expresion();
                    resultado /= stack.Pop();
                    break;
                case "%=":
                    match(Tipos.IncrementoFactor);
                    Expresion();
                    resultado %= stack.Pop();
                    break;
            }
            return resultado;
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool evaluaAsm)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            asm.WriteLine("Pop AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion, evaluaAsm);
            if(getContenido() == "default")
            {
                match("default");
                match(":");
                if(getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, evaluaAsm);  
                }
                else
                {
                    Instruccion(evaluacion, evaluaAsm);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool evaluaAsm)
        {
            match("case");
            Expresion();
            stack.Pop();
            if(evaluaAsm)
            {
                asm.WriteLine("Pop AX");
            }
            match(":");
            ListaInstruccionesCase(evaluacion, evaluaAsm);
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos(evaluacion, evaluaAsm);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            String operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("Pop AX");
            float e1 = stack.Pop();
            asm.WriteLine("Pop BX");
            asm.WriteLine("Cmp AX, BX");
            switch(operador)
            {
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case "<":
                    asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case "<=":
                    asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                case ">":
                    asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case ">=":
                    asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool evaluaAsm)
        {
            string etiquetaIf = "if" + ++cIf;
            string finIf = "else" + cIf;
            match("if");
            match("(");
            bool validarIf = Condicion(etiquetaIf);
            if(!evaluacion)
            {
                validarIf = false;
            }
            match(")");
            if(getContenido() == "{")
            {
                BloqueInstrucciones(validarIf, evaluaAsm);          
            }
            else
            {
                Instruccion(validarIf, evaluaAsm);  
            }
            if(evaluaAsm)
            {
                asm.WriteLine("JMP " + finIf);
                asm.WriteLine(etiquetaIf + ":");
            }
            if(getContenido() == "else")
            {
                match("else");
                if(getContenido() == "{")
                {
                    if(evaluacion)
                    {
                        BloqueInstrucciones(!validarIf, evaluaAsm);
                    }
                    else
                    {
                        BloqueInstrucciones(evaluacion, evaluaAsm);
                    }
                }
                else
                {
                    if(evaluacion)
                    {
                        Instruccion(!validarIf, evaluaAsm);
                    }
                    else
                    {
                        Instruccion(evaluacion, evaluaAsm);
                    }
                }
            }
            asm.WriteLine(finIf + ":");
        }

        //Printf -> printf(cadena o expresion);
        private void Printf(bool evaluacion, bool evaluaAsm)
        {
            match("printf");
            match("(");
            if(getClasificacion()==Tipos.Cadena)
            {
                if(evaluacion){
                    string cadena = getContenido();
                    if(cadena.Contains("\\n"))
                    {
                        cadena=cadena.Replace("\\n", "\n");
                    }
                    if(cadena.Contains("\\t"))
                    {
                       cadena=cadena.Replace("\\t", "\t");
                    }
                    for(int i=1; i<cadena.Length-1; i++)
                    {
                        Console.Write(cadena[i]);
                    }
                }
                if(evaluaAsm)
                {
                    asm.WriteLine("PRINTN " + getContenido());
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                if(evaluaAsm)
                {
                    asm.WriteLine("Pop AX");
                }
                if(evaluacion)
                {
                    Console.Write(resultado);
                }
            }
            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &identificador);
        private void Scanf(bool evaluacion, bool evaluaAsm)    
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if(!existeVariable(getContenido()))
            {
                throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
            } 
            if(evaluacion)
            {
                string val = ""+Console.ReadLine(); 
                double validaVal;
                if(!double.TryParse(val, out validaVal))
                {
                    throw new Error("Error de sintaxis, se espera un numero en linea: "+linea, log);
                }
                float valorFloat = float.Parse(val);
                modVariable(getContenido(), valorFloat);
            } 
            if(evaluaAsm)
            {
                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + getContenido() + ", CX");
            }      
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if(getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador+" ");
                float n1=stack.Pop();
                asm.WriteLine("Pop BX");
                float n2=stack.Pop();
                asm.WriteLine("Pop AX");
                switch(operador)
                {
                    case "+":
                        stack.Push(n2+n1);
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("Push AX");
                        break;
                    case "-":
                        stack.Push(n2-n1);
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("Push AX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if(getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador+" ");
                float n1=stack.Pop();
                asm.WriteLine("Pop BX");
                float n2=stack.Pop();
                asm.WriteLine("Pop AX");
                //Requerimiento 1.a y 3.b
                switch(operador)
                {
                    case "*":
                        stack.Push(n2*n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("Push AX");
                        break;
                    case "/":
                        stack.Push(n2/n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("Push AX");
                        break;
                    case "%":
                        stack.Push(n2%n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("Push DX");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if(getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if(dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }       
                stack.Push(float.Parse(getContenido()));
                asm.WriteLine("Mov AX, "+ getContenido());
                asm.WriteLine("Push AX");
                match(Tipos.Numero);
            }
            else if(getClasificacion() == Tipos.Identificador)
            {
                if(!existeVariable(getContenido()))
                {
                    throw new Error("Error de sintaxis, variable inexistente <" +getContenido()+"> en linea: "+linea, log);
                }
                log.Write(getContenido() + " ");
                if(dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                //Requerimiento 3.a
                asm.WriteLine("Mov AX, " + getContenido());
                asm.WriteLine("Push AX");
                match(Tipos.Identificador);
            }
            else
            {
                bool huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                match("(");
                if(getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch(getContenido())
                    {
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if(huboCasteo)
                {
                    dominante = casteo;
                    float valor = stack.Pop();
                    asm.WriteLine("Pop AX");
                    valor = convert(valor, dominante);
                    stack.Push(valor);
                    //Requerimiento 3.a
                    asm.WriteLine("Mov AX, " + valor);
                    asm.WriteLine("Push AX");
                }
            }
        }
    }
}