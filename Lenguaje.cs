using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;

/*
    - REQUERIMIENTOS -
        -----------------------------------------------------------------------------------
         1) Concatenaciones                                                               
            Concatenaciones -> Identificador | cadena (+ Concatenaciones)?                
                                            // sin comillas                                 
        ------------------------------------------------------------------------------------
         2) Inicializar una variable                                                       
            ListaIdentificadores -> identificador (= Expresion)? (, ListaIdentificadores)? 
        ------------------------------------------------------------------------------------
         3) Evaluar las expresiones                                                        
        ------------------------------------------------------------------------------------
         4) Condicion, Asignacion                                                          
        ------------------------------------------------------------------------------------
*/

namespace Sintaxis_1 {
    public class Lenguaje : Sintaxis {
        Stack<float> s;
        List<Variable> l;
        public Lenguaje() : base() {
            s = new Stack<float>();
            l = new List<Variable>();
        }
        public Lenguaje(string name) : base(name) {
            s = new Stack<float>();
            l = new List<Variable>();
        }

        private void DysplayStack() {
            Console.WriteLine("Contenido del Stack");
            foreach (float elemento in s) {
                Console.WriteLine(elemento);
            }
        }

        private void DysplayList() {
            log.WriteLine("Lista de variables");
            foreach (Variable elemento in l) {
                log.WriteLine("{0}  {1}  {2}", elemento.getTipoDato(), elemento.getNombre(), elemento.getValor());
            }
        }

        // ? Cerradura epsilon
        //Programa  -> Librerias? Variables? Main
        public void Programa() {
            if (getContenido() == "using") {
                Librerias();
            }

            if (getClasificacion() == Tipos.TipoDato) {
                Variables();
            }
            Main();
            DysplayList();
        }

        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias() {
            match("using");
            ListaLibrerias();
            match(";");

            if (getContenido() == "using") {
                Librerias();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables() {
            Variable.TipoDato t = Variable.TipoDato.Char;

            switch (getContenido()) {
                case "int":   t = Variable.TipoDato.Int; break;
                case "floar": t = Variable.TipoDato.Float; break;
            }
            
            match(Tipos.TipoDato);
            ListaIdentificadores(t);
            match(";");

            if (getClasificacion() == Tipos.TipoDato) {
                Variables();
            }
        }

        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias() {
            match(Tipos.Identificador);

            if (getContenido() == ".") {
                match(".");
                ListaLibrerias();
            }
        }

        //ListaIdentificadores -> identificador (, ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TipoDato t) {
            l.Add(new Variable(t, getContenido()));
            match(Tipos.Identificador);

            if (getContenido() == ",") {
                match(",");
                ListaIdentificadores(t);
            }
        }

        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones() {
            match("{");
            if (!(getContenido() == "}")) {
                ListaInstrucciones();
            } else {
                match("}");
            }
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones() {
            Instruccion();

            if (getContenido() != "}") {
                ListaInstrucciones();
            } else {
                match("}");
            }
        }

        //Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion() {
            if (getContenido() == "Console") {
                console();
            } else if (getContenido() == "if") {
                If();
            } else if (getContenido() == "while") {
                While();
            } else if (getContenido() == "do") {
                Do();
            } else if (getContenido() == "for") {
                For();
            } else if (getClasificacion() == Tipos.TipoDato) {
                Variables();
            } else {
                Asignacion();
                match(";");
            }
        }
        
        //Asignacion -> id = Expresion | id++ | id-- | id  IncTermino expresion |                                           
                      //id IncrementoFactor Expresion | id = console.Read() | id = console.ReadLine()
        private void Asignacion() {
            Console.Write(getContenido() + " = ");
            match(Tipos.Identificador);

            if (getContenido() == "=") {
                match("=");
                if (getContenido() == "Console") {
                     match("Console");
                     match(".");

                    if (getContenido() == "Read") {
                        match("Read");
                        match("(");
                        match(")");
                        Console.Read();
                    } else {
                        match("ReadLine");
                        match("(");
                        match(")");
                        Console.ReadLine();
                    }
                } else {
                    Expresion();
                }
            } else {
                if (getContenido() == "++" || getContenido() == "--") {
                    match(Tipos.IncrementoTermino);
                } else {
                    if (getContenido() == "+=" || getContenido() == "-=") {
                        match(Tipos.IncrementoTermino);
                    } else {
                        match(Tipos.IncrementoFactor);
                    }
                    Expresion();
                }
            }
            Console.WriteLine(" = " + s.Pop());
            DysplayStack();
        }

        //If -> if (Condicion) bloqueInstrucciones | instruccion
             //(else bloqueInstrucciones | instruccion)?
        private void If() {
            match("if");
            match("(");
            Condicion();
            match(")");

            if (getContenido() == "{") {
                BloqueInstrucciones();
            } else {
                Instruccion();
            }

            if (getContenido() == "else") {
                match("else");
                if (getContenido() == "{") {
                    BloqueInstrucciones();
                } else {
                    Instruccion();
                }
            }
        }

        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion() {
            Expresion();
            match(Tipos.OperadorRelacional);
            Expresion();
        }

        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While() {
            match("while");
            match("(");
            Condicion();
            match(")");

            if (getContenido() == "{") {
                BloqueInstrucciones();
            } else {
                Instruccion();
            }
        }
        
        //Do -> do 
                //bloqueInstrucciones | intruccion 
                //while(Condicion);
        private void Do() {
            match("do");

            if (getContenido() == "{") {
                BloqueInstrucciones();
            } else {
                Instruccion();
            }

            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        
        //For -> for(Asignacion; Condicion; Asignacion) 
               //BloqueInstrucciones | Intruccion
        private void For() {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion();
            match(";");
            Asignacion();
            match(")");

            if (getContenido() == "{") {
                BloqueInstrucciones();
            } else {
                Instruccion();
            }
        }

        //console -> console.(WriteLine|Write) (cadena concatenaciones?);
        private void console() {
            String tipoWrite,
                   texto;

            match("Console");
            match(".");
            
            if (getContenido() == "Write") {
                tipoWrite = "Write";
                match("Write");
            } else {
                tipoWrite = "WriteLine";
                match("WriteLine");
            } 
           
            match("(");

            if (tipoWrite == "WriteLine" && getContenido() == ")") {
                match(")");
                match(";");
                Console.WriteLine();
            } else {
                texto = getContenido().Trim('"');

                if (getClasificacion() == Tipos.Cadena) {
                    match(Tipos.Cadena);
                } else {
                    match(Tipos.Identificador);
                }

                if (getContenido() == "+") {
                    Concatenaciones(texto, tipoWrite);
                } else {
                    match(")");
                    match(";");

                    if (tipoWrite == "Write") {
                        Console.Write(texto);
                    } else {
                        Console.WriteLine(texto);
                    }
                }
            }
        }

        //Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main() {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones();
        }

        //Expresion -> Termino MasTermino
        private void Expresion() {
            Termino();
            MasTermino();
        }
        
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino() {
            if (getClasificacion() == Tipos.OperadorTermino) {
                String operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                Console.Write(operador + " ");

                float n1 = s.Pop();
                float n2 = s.Pop();

                switch (operador) {
                    case "+": s.Push(n2 + n1); break;
                    case "-": s.Push(n2 - n1); break;
                }
            }
        }
        
        //Termino -> Factor PorFactor
        private void Termino() {
            Factor();
            PorFactor();
        }
        
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor() {
            if (getClasificacion() == Tipos.OperadorFactor) {
                String operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                Console.Write(operador + " ");

                float n1 = s.Pop();
                float n2 = s.Pop();

                switch (operador) {
                    case "*": s.Push(n2 * n1); break;
                    case "/": s.Push(n2 / n1); break;
                    case "%": s.Push(n2 % n1); break;
                }
            }
        }
        
        //Factor -> numero | identificador | (Expresion)
        private void Factor() {
            if (getClasificacion() == Tipos.Numero) {
                s.Push(float.Parse(getContenido()));
                Console.Write(getContenido() + " ");
                match(Tipos.Numero);
            } else if (getClasificacion() == Tipos.Identificador) {
                s.Push(0);
                Console.Write(getContenido() + " ");
                match(Tipos.Identificador);
            } else {
                match("(");
                Expresion();
                match(")");
            }
        }

        //Concatenaciones -> Identificador | cadena (+ Concatenaciones)?                
                                          // sin comillas
        private void Concatenaciones(String texto, String tipo) {
            match("+");

            texto += getContenido().Trim('"');

            if (getClasificacion() == Tipos.Cadena) {
                match(Tipos.Cadena);
            } else {
                match(Tipos.Identificador);
            }
            
            if (getContenido() == "+") {
                Concatenaciones(texto, tipo);
            } else {
                match(")");
                match(";");

                if (tipo == "Write") {
                    Console.Write(texto);
                } else {
                    Console.WriteLine(texto);
                }
            }
        }
    }
}