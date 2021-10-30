package es.ucm.gdv.ohno;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;


public class Logic {
    private Square[][] board;     // Tablero con todas las casillas
    private List<Square> locked; // Las casillas lockeadas

    public Logic (){
        locked = new ArrayList <Square>();
    }

    //crear tablero inicial
    public void init(int tam){
        board = new Square[tam+2][tam+2];//tamaño +2 para bordear con rojos
        for(int i = 0; i < board[0].length; ++i){
            for(int j = 0; j < board[1].length; ++j) {
                board[i][j] = new Square();
            }
        }
        Random rnd = new Random();

        //rellenamos el tablero con azules y rojos de forma aleatoria
        //minimo un rojo y un azul que no este rodeado por rojos
        do {
            for (int i = 0; i < board[0].length; ++i) {
                for (int j = 0; j < board[1].length; ++j) {
                    //bordeamos de rojos
                    if (i == 0 || i == board[0].length - 1 || j == 0 || j == board[1].length - 1) {
                        board[i][j].solutionState = Square.SquareColor.Red;
                        board[i][j].currentState = Square.SquareColor.Red;
                        board[i][j].lock = true;
                    } else {// rellenar con aleatorios rojos y azules
                        if (rnd.nextFloat() < 0.5) {
                            board[i][j].solutionState = Square.SquareColor.Blue;
                            board[i][j].currentState = Square.SquareColor.Grey;
                        } else {
                            board[i][j].solutionState = Square.SquareColor.Red;
                            board[i][j].currentState = Square.SquareColor.Grey;
                        }
                    }
                    board[i][j].posX = i;
                    board[i][j].posY = j;
                }
            }
            //TODO: quitar esto y el metodo
            //pruebas();

            //contar elementos adyacentes de la fila y columna
            //el tablero es cuadrado asi que se puede hacer asi
            for (int i = 1; i < board[0].length -1; ++i) {
                countRow(i, false);//cuenta los adyacentes azules que hay en esa fila
                countCol(i, false);//cuenta los adyacentes azules que hay en esa columna
            }

            for (int i = 1; i < board[0].length-1; ++i) {
                for (int j = 1; j < board[0].length-1; ++j) {
                    board[i][j].total = board[i][j].row + board[i][j].column;
                }
            }

            reveal();
        }while (locked.size() == 0);//tablero con al menos un rojo y azul

    }

    // Comprueba si se ha solucionado
    public boolean check(){
        for(int i = 1; i < board[0].length -1; ++i) {
            for (int j = 1; j < board[1].length - 1; ++j) {
                if(board[i][j].currentState != board[i][j].solutionState)
                    return false;
            }
        }
        return true;
    }

    // Metodo para reiniciar el nivel y dejarlo como al principio
    // si canFinish es false, significa que el nivel no se puede completar
    // y hay que añadir mas casillas visibles al empezar
    public void reStart(Boolean canFinish){

        //añadir un rojo si no es posible completar el nivel
        if(!canFinish) {
            Square sDif = null;
            for(Square s : locked){
                if(s.total != s.playerColumn + s.playerRow) {
                    sDif = s;
                    break;
                }
            }
            Square s = mustBeRed(sDif);

            //if(i < locked.size()) //TODO esto no deberia pasar, hacemos algo?
            s.lock=true;
            s.currentState = s.solutionState;
            System.out.println("Añadido rojo lockeado en "+(s.posX -1) + " "+ (s.posY -1));

        }//rojo añadido

        //resetear valores
        for(int i = 1; i < board[0].length -1; ++i) {
            for (int j = 1; j < board[1].length - 1; ++j) {
                if(!board[i][j].lock) {
                    board[i][j].currentState= Square.SquareColor.Grey;
                }
                board[i][j].playerColumn = 0;
                board[i][j].playerRow = 0;
            }
            countRow(i,false);//cuenta los adyacentes azules que hay en esa fila
            countCol(i,false);//cuenta los adyacentes azules que hay en esa columna
        }

    }

    // Devuelve un rojo adyacente al square dado no lockeado
    // Devuelve board[0][0] en caso de no encontrar ninguno.
    /*private Square giveRedAdyacent(Square square){
        for(Dirs d: Dirs.values()) {//recorrer en todas las direcciones
            int x = square.posX + d.getRow();
            int y = square.posY + d.getCol();
            //buscar un rojo adyacente no lockeado
            while(x != 0 && x != board[0].length -1 && y != 0 && y != board[1].length -1 )//hasta encontrar un rojo no lockeado
            {
                if(board[x][y].solutionState == Square.SquareColor.Red ) {//encontramos un rojo
                    if( !board[x][y].lock)//si no esta lockeado es el que queremos
                        return board[x][y];
                    else break;//si esta lockeado no nos sirve
                }
                x += d.getRow();
                y += d.getCol();
            }
        }
        return board[0][0];
    }*/

    //encontrar un gris adyacente a square y cambiarlo a rojo
    private Square mustBeRed(Square square){
        for(Dirs d: Dirs.values()) {//recorrer en todas las direcciones
            int x = square.posX + d.getRow();
            int y = square.posY + d.getCol();
            //buscar un gris adyacente sin pasar por rojo
            while(x != 0 && x != board[0].length -1 && y != 0 && y != board[1].length -1 )
            {
                //encontramos un gris que en la solucion es rojo
                if(board[x][y].currentState == Square.SquareColor.Grey  &&
                        board[x][y].solutionState == Square.SquareColor.Red) {
                    return board[x][y];
                }
                else if(board[x][y].solutionState == Square.SquareColor.Red ) {//encontramos un rojo
                    break;
                }
                x += d.getRow();
                y += d.getCol();
            }
        }
        return board[0][0];//no se ha encontrado ninguno (el square pasado por parametro no era el correcto)
    }


    //Devuelve una pista
    public String giveHint(){
        for(Square s : locked){
            if(hint1(s,false)){
                System.out.println("Pista 1 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                return Hint.CanClose.name();
            }
            else if(hint2(s,false)){
                System.out.println("Pista 2 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                return Hint.WouldSeeTooMuch.name();

            }
            else if(hint3(s,false)){
                System.out.println("Pista 3 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                return Hint.WouldSeeTooLittle.name();

            }
            else if(hint4(s)){
                System.out.println("Pista 4 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                return Hint.SeesTooMuch.name();

            }
            else if(hint5(s)){
                System.out.println("Pista 5 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                return Hint.SeesToLittle.name();

            }
            else{
                //si entra aqui es que se cumple todas pistas en ese square, mover al final de los locked
                //para no revisar de nuevo al pedir pista al principio porque probablemente
                //siga sin cumplir ninguna pista. No hay que eliminarlo por si el jugador se equivoca
                int ind = locked.indexOf(s);
                locked.add(locked.size(),s);
                locked.remove(ind);
            }
        }
        for(int i = 1; i < board[0].length -1; ++i){
            for(int j = 1; j < board[1].length -1; ++j) {
                if(!board[i][j].lock)
                    if(hint6_7(board[i][j], false)) {
                        System.out.println("Pista 6/7 aceptada en " + (board[i][j].posX - 1)
                                + " " + (board[i][j].posY - 1));
                        if(board[i][j].currentState == Square.SquareColor.Blue)
                            return Hint.MustBeRedBlue.name();
                        return Hint.MustBeRedGrey.name();
                    }

            }
        }
        return "todo perfecto";
    }

    // Aplica pistas. Se usa para terminar de generar un nivel soluble
    public boolean doHint(){
        for(Square s : locked){
            if(hint1(s,true)){
                System.out.println("Pista 1 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
            else if(hint2(s,true)){
                System.out.println("Pista 2 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
            else if(hint3(s,true)){
                System.out.println("Pista 3 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
            else if(hint4(s)){
                System.out.println("Pista 4 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
            else if(hint5(s)){
                System.out.println("Pista 5 aceptada en "+(s.posX -1) + " "+ (s.posY -1));
                countRow(s.posX,true);
                countCol(s.posY,true);
                return true;
            }
        }

        //si alguno de los locked no estan completos, no se puede completar el nivel
        for(Square s : locked) {
            if (s.total != s.playerColumn + s.playerRow) {
                return false;
                //break;
            }
        }

        for(int i = 1; i < board[0].length -1; ++i){
            for(int j = 1; j < board[1].length -1; ++j) {
                if(!board[i][j].lock)
                    if(hint6_7(board[i][j], true)) {
                        System.out.println("Pista 6/7 aceptada en " + (board[i][j].posX - 1)
                                + " " + (board[i][j].posY - 1));
                        countRow(board[i][j].posX,true);
                        countCol(board[i][j].posY,true);
                        return true;
                    }

            }
        }
        return false;
    }

    //devuelve true si ve suficiente para cerrarlo y no esta cerrado.
    private boolean hint1(Square square, boolean modify){
        boolean check = square.total == square.playerColumn + square.playerRow;
        if(check) {
            for (Dirs d : Dirs.values()) {
                int x = square.posX;
                int y = square.posY;
                if(x >= board[0].length || y >= board[0].length )
                    System.out.println("F");
                while (board[x][y].currentState == Square.SquareColor.Blue) {
                    x += d.getRow();
                    y += d.getCol();
                    if(x >= board[0].length || y >= board[0].length )
                        System.out.println("F");
                }
                if(board[x][y].currentState == Square.SquareColor.Grey){
                    if(modify)
                        board[x][y].currentState = Square.SquareColor.Red;
                    return true;
                }
            }
        }
        return  false;
    }

    //devuelve true si veria demasiados. Es decir, tiene que poner un rojo en cierta direccion
    private boolean hint2(Square square, boolean modify){
        for (Dirs d : Dirs.values()) {
            int x = square.posX;
            int y = square.posY;
            while (board[x][y].currentState == Square.SquareColor.Blue) {
                x += d.getRow();
                y += d.getCol();
            }
            if(board[x][y].currentState == Square.SquareColor.Grey &&   // Hay un gris seguido de un azul
                    board[x + d.getRow()][y + d.getCol()].currentState == Square.SquareColor.Blue){
                //x += d.getRow();y += d.getCol();
                if(d == Dirs.UP || d == Dirs.DOWN){ // Columna
                    if((board[x][y].playerColumn + (square.playerRow + square.playerColumn + 1))
                            > (square.total)) {
                        if(modify)//modificar
                            board[x][y].currentState = Square.SquareColor.Red;
                        return true;    // Más grande que la solución
                    }
                }
                else{                              // Fila
                    if((board[x][y].playerRow + (square.playerRow + square.playerColumn + 1))
                            > (square.total)){
                        if(modify) {
                            board[x][y].currentState = Square.SquareColor.Red;//modificar

                        }
                        return true;    // Más grande que la solución
                    }
                }
            }
        }
        return false;
    }

    private boolean hint3(Square square, boolean modify){
        //guardamos los que puede ver y los que ya ve por cada direccion
        //vamos a guardar en las 4 direcciones los que pueden ser azules,
        //los que actualmente son azules
        //y la direccion a la que llega ahi (x, y) 1,0 = derecha
        Integer[][] posAdyCount = new Integer[4][4];
        int i=0;
        int posCount=0;
        int blueCount=0;
        int dir=0;
        for(Dirs d: Dirs.values()){//recorrer en todas las direcciones
            int x = square.posX + d.getRow();
            int y = square.posY + d.getCol();
            boolean countBlue = true;//cuentas los azules adyacentes
            while(board[x][y].currentState != Square.SquareColor.Red){
                if(board[x][y].currentState == Square.SquareColor.Grey )//guardamos los azules en esa dir
                   countBlue = false;
                else if(countBlue)
                    blueCount++;
                posCount++;//guardamos todos los posibles azules
                x += d.getRow();
                y += d.getCol();
            }

            posAdyCount[i][0]=posCount;
            posAdyCount[i][1]=blueCount;
            posAdyCount[i][2]=d.getRow();
            posAdyCount[i][3]=d.getCol();
            posCount=blueCount=0;
            i++;
        }//fin buscar en 4 direcciones los posibles azules

        //ordenamos de menor a mayor los posibles azules menos los que ya son azules
        for( i = 0; i < posAdyCount[0].length -1; ++i){
            for(int j = 0; j < posAdyCount[0].length -1; ++j){
                int tmp = posAdyCount[j][0] - posAdyCount[j][1];
                int aux = posAdyCount[j+1][0] - posAdyCount[j+1][1];
                if(tmp > aux){
                    tmp = posAdyCount[j][0];
                    aux = posAdyCount[j][1];
                    int aux2 = posAdyCount[j][2];
                    int aux3 = posAdyCount[j][3];
                    posAdyCount[j][0] = posAdyCount[j+1][0];
                    posAdyCount[j][1] = posAdyCount[j+1][1];
                    posAdyCount[j][2] = posAdyCount[j+1][2];
                    posAdyCount[j][3] = posAdyCount[j+1][3];

                    posAdyCount[j+1][0] = tmp;
                    posAdyCount[j+1][1] = aux;
                    posAdyCount[j+1][2] = aux2;
                    posAdyCount[j+1][3] = aux3;
                }
            }
        }//ya esta ordenado de menor a mayor

        //sumamos los posibles en todas las direcciones menos
        // la que estamos comprobando (tiene mas posibles)
        int count=0;
        for(i=0;i<3;++i){
            count += posAdyCount[i][0];
        }

        if(modify && count < square.total - posAdyCount[3][1]){
            int x= square.posX + posAdyCount[3][2];
            int y= square.posY + posAdyCount[3][3];
            while(board[x][y].currentState == Square.SquareColor.Blue) {
                x += posAdyCount[3][2];
                y += posAdyCount[3][3];
            }
            if(board[x][y].currentState != Square.SquareColor.Red)
                board[x][y].currentState = Square.SquareColor.Blue;
            //actualizar valores de los que ve en fila y columna



        }

        return count < square.total - posAdyCount[3][1];
    }

    //Ve más de los que puede ver
    private boolean hint4(Square square){
        return square.total < square.playerColumn + square.playerRow;
    }

    //Tiene que ver más y ya está cerrado
    private boolean hint5(Square square){
        //si ves menos de los que deberias y al final de tus adyacentes azules hay un rojo
         if(square.total > square.playerColumn + square.playerRow){
             for(Dirs d: Dirs.values()){//buscar en todas direcciones
                 int x = square.posX + d.getRow();
                 int y = square.posY + d.getCol();
                 while(board[x][y].currentState != Square.SquareColor.Red){
                     //si encuentra un gris significa que no esta rodeado por rojos
                     if(board[x][y].currentState == Square.SquareColor.Grey)
                         return false;
                     x += d.getRow();
                     y += d.getCol();
                 }
             }
             //si no has encontrado un gris al mirar en todas las direcciones esta cerrado por rojos
             return true;
         }
         return false;
    }

    //devuelve si el square esta rodeado por rojos
    //le tienen que llegar square azules o grises
    private boolean hint6_7(Square square, boolean modify){
        if(square.currentState == Square.SquareColor.Red ) return false;
        for(Dirs d: Dirs.values()){//buscar en todas direcciones que no haya un azul
            int x = square.posX + d.getRow();
            int y = square.posY + d.getCol();
            while(board[x][y].currentState == Square.SquareColor.Grey ||
                    (board[x][y].currentState == Square.SquareColor.Blue && !board[x][y].lock)){
                x += d.getRow();
                y += d.getCol();
            }
            //Si el que encuentra es azul no esta rodeado por rojos
            if(board[x][y].currentState == Square.SquareColor.Blue && board[x][y].lock)
                return false;
        }//deja de buscar. Rodeado por rojos

        if(modify )
            square.currentState = Square.SquareColor.Red;
        return true;
    }

    public void printSolution(){
        System.out.println();
        for(int i = 1; i < board[0].length -1; ++i){
            for(int j = 1; j < board[1].length -1; ++j){
                System.out.print((board[i][j].total) + " " );
            }
            System.out.println();
        }
    }
    public void print(){


        System.out.println();

        //muestra el estado del tablero
        for(int i = 1; i < board[0].length -1; ++i){
            for(int j = 1; j < board[1].length -1; ++j){
                if(board[i][j].lock && board[i][j].currentState == Square.SquareColor.Blue)
                    System.out.print(board[i][j].total+" ");
                else if(board[i][j].currentState == Square.SquareColor.Blue)
                    System.out.print("A ");
                else if(board[i][j].currentState == Square.SquareColor.Grey)
                    System.out.print("0 ");
                else if(board[i][j].currentState == Square.SquareColor.Red)
                    System.out.print("R ");
            }
            System.out.println();
        }
    }

    //recibe el tablero y la fila a calcular.
    // si player es true es que lo modifica el player
    //sino es para la solucion incial
    private void countRow(int i, boolean player){
        int j=1;
        if(!player){
            while(j != board[0].length){
                if(board[i][j].row == 0 && board[i][j].solutionState == Square.SquareColor.Blue)
                    countRowRec(i, j,0, player);
                j++;
            }
        }
        else{
            while(j != board[0].length){
                if( board[i][j].currentState == Square.SquareColor.Blue && board[i][j-1].currentState != Square.SquareColor.Blue)
                    countRowRec(i, j,0, player);
                j++;
            }
        }
    }

    //recibe el tablero, la fila, la columna y el numero que ha contado de adyacentes en esa fila
    //la condicion de parada es encontrar una casilla roja o el final
    private int countRowRec(int i, int j, int cont, boolean player){
        if(!player) {
            if (j == board[0].length - 1) {//si llegamos al final
                board[i][j].row = cont;
                return cont;
            }
            if (board[i][j + 1].solutionState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
                board[i][j].row = countRowRec(i, j + 1, ++cont, player);
            else//si el siguiente es rojo, dejo de aumentar adyacentes
                board[i][j].row = cont;
            //aqui se llega al encontrar un rojo.
            return board[i][j].row;
        }
        else{
            if (j == board[0].length - 1) {//si llegamos al final
                board[i][j].playerRow = cont;
                return cont;
            }
            if (board[i][j + 1].currentState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
                board[i][j].playerRow = countRowRec(i, j + 1, ++cont, player);
            else//si el siguiente es rojo, dejo de aumentar adyacentes
                board[i][j].playerRow = cont;
            //aqui se llega al encontrar un rojo.
            return board[i][j].playerRow;
        }
    }

    //recibe el tablero y la columna a calcular el numero de azules adyacentes
    private void countCol(int j,boolean player){
        int i=1;
        if(!player) {
            while (i != board[0].length) {
                if (board[i][j].column == 0 && board[i][j].solutionState == Square.SquareColor.Blue)
                    countColRec(i, j, 0, player);
                i++;
            }
        }
        else{
            while (i != board[0].length) {
                if (board[i][j].currentState == Square.SquareColor.Blue && board[i-1][j].currentState != Square.SquareColor.Blue)
                    countColRec(i, j, 0, player);
                i++;
            }
        }
    }
    //recibe el tablero, la fila, la columna y el numero que ha contado de adyacentes en esa columna
    //la condicion de parada es encontrar una casilla roja o el final
    private int countColRec(int i, int j, int cont,boolean player){
        if(!player) {
            if (i == board[0].length - 1) {//si llegamos al final
                board[i][j].column = cont;
                return cont;
            }
            if (board[i + 1][j].solutionState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
                board[i][j].column = countColRec(i + 1, j, ++cont, player);
            else//si el siguiente es rojo, dejo de aumentar adyacentes
                board[i][j].column = cont;
            //aqui se llega al encontrar un rojo.
            return board[i][j].column;
        }
        else{
            if(i == board[0].length-1) {//si llegamos al final
                board[i][j].playerColumn = cont;
                return cont;
            }
            if (board[i + 1][j].currentState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
                board[i][j].playerColumn = countColRec(i+1, j, ++cont,player);
            else//si el siguiente es rojo, dejo de aumentar adyacentes
                board[i][j].playerColumn = cont;
            //aqui se llega al encontrar un rojo.
            return board[i][j].playerColumn;
        }
    }

    //recibe el tablero y la fila a calcular
    private void markShowInRow(int i){
        int j=1;
        while(board[0].length -1 != j){
            if(!board[i][j].lock  && board[i][j].solutionState == Square.SquareColor.Blue) {
                if(board[i][j-1].showInRow  || (board[i][j-1].lock &&
                        board[i][j-1].currentState == Square.SquareColor.Blue))//el anterior es azul
                    board[i][j].showInRow = true;
                //el de la izquierda es rojo y el de la derecha no
                else if(board[i][j+1].solutionState != Square.SquareColor.Red)
                {
                    board[i][j].lock = true;
                    locked.add(board[i][j]);
                    board[i][j].showInRow = true;
                    board[i][j].currentState = board[i][j].solutionState;
                }
            }
            j++;
        }
    }

    private void markShowInCol(int j){
        int i=1;
        while( board[0].length -1 != i){
            if(!board[i][j].lock && board[i][j].solutionState == Square.SquareColor.Blue) {
                if(board[i-1][j].showInColumn  || (board[i-1][j].lock &&
                        board[i-1][j].currentState == Square.SquareColor.Blue) )//el anterior es azul
                    board[i][j].showInColumn = true;
                //el de arriba es rojo y el de abajo no
                else if(board[i+1][j].solutionState != Square.SquareColor.Red)
                {
                    board[i][j].lock = true;
                    locked.add(board[i][j]);
                    board[i][j].showInColumn = true;
                    board[i][j].currentState = board[i][j].solutionState;
                }
            }
            i++;
        }
    }

    private void reveal(){
        int size = board[0].length;
        Random r = new Random();

        for(int i = 1; i < size -1; ++i){
            for(int j = 1; j < size -1; ++j){
                // comprobar que no se ha generado aislado
                if((board[i][j].total) == 0 && board[i][j].solutionState == Square.SquareColor.Blue){
                    board[i][j].solutionState = Square.SquareColor.Red;
                }
                if(board[i][j].solutionState == Square.SquareColor.Red){
                    if(r.nextFloat() >=0.5f) {
                        board[i][j].lock = true;
                        board[i][j].currentState = Square.SquareColor.Red;
                    }
                }
            }
        }

        for(int i = 1; i < board[0].length -1; ++i){
            markShowInRow(i);   //revela los necesarios en filas
            markShowInCol(i);   //revela los nocesarios en columnas
        }

    }

    private void pruebas(){
        //para crear nuestro tablero si queremos, para hacer pruebas
        for(int i = 0; i < board[0].length; ++i) {
            for (int j = 0; j < board[1].length; ++j) {
                //bordeamos de rojos
                if (i == 0 || i == board[0].length - 1 || j == 0 || j == board[1].length - 1) {
                    board[i][j].solutionState = Square.SquareColor.Red;
                    board[i][j].currentState = Square.SquareColor.Red;
                }
                board[i][j].posX = i;
                board[i][j].posY = j;
            }
        }
        int x=1, y=1;
        board[x][y].solutionState = Square.SquareColor.Red;
        board[x][y].currentState = Square.SquareColor.Grey;
        //board[x][y].lock = true;
        x=1; y=2;
        board[x][y].solutionState = Square.SquareColor.Blue;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=1; y=3;
        board[x][y].solutionState = Square.SquareColor.Blue;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=1; y=4;
        board[x][y].solutionState = Square.SquareColor.Blue;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=2; y=1;
        board[x][y].solutionState = Square.SquareColor.Red;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=2; y=2;
        board[x][y].solutionState = Square.SquareColor.Blue;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=2; y=3;
        board[x][y].solutionState = Square.SquareColor.Red;
        board[x][y].currentState = Square.SquareColor.Grey;
        //board[x][y].lock = true;
        x=2; y=4;
        board[x][y].solutionState = Square.SquareColor.Blue;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=3; y=1;
        board[x][y].solutionState = Square.SquareColor.Red;
        board[x][y].currentState = Square.SquareColor.Grey;
        //board[x][y].lock = true;
        x=3; y=2;
        board[x][y].solutionState = Square.SquareColor.Red;
        board[x][y].currentState = Square.SquareColor.Grey;
        //board[x][y].lock = true;
        x=3; y=3;
        board[x][y].solutionState = Square.SquareColor.Red;
        board[x][y].currentState = Square.SquareColor.Grey;
        //board[x][y].lock = true;
        x=3; y=4;
        board[x][y].solutionState = Square.SquareColor.Blue;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=4; y=1;
        board[x][y].solutionState = Square.SquareColor.Red;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=4; y=2;
        board[x][y].solutionState = Square.SquareColor.Blue;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=4; y=3;
        board[x][y].solutionState = Square.SquareColor.Blue;
        board[x][y].currentState = Square.SquareColor.Grey;
        x=4; y=4;
        board[x][y].solutionState = Square.SquareColor.Red;
        board[x][y].currentState = Square.SquareColor.Grey;
        //board[x][y].lock = true;
    }
}

