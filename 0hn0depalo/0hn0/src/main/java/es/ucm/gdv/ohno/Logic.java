package es.ucm.gdv.ohno;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;


public class Logic {
    public Square[][] board; // Tablero con todas las casillas
    private List<Square> locked;     // Las casillas lockeadas

    public Logic (int tam){
        locked = new ArrayList <Square>();
        init(tam);
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
        for(int i = 0; i < board[0].length; ++i){
            for(int j = 0; j < board[1].length; ++j){
                //bordeamos de rojos
                if(i == 0 || i== board[0].length -1|| j == 0 || j== board[1].length -1) {
                    board[i][j].solutionState = Square.SquareColor.Red;
                    board[i][j].currentState = Square.SquareColor.Red;
                }
                else{
                    if(rnd.nextFloat() < 0.5) {
                        board[i][j].solutionState = Square.SquareColor.Blue;
                        board[i][j].currentState = Square.SquareColor.Grey;
                    }
                    else {
                        board[i][j].solutionState = Square.SquareColor.Red;
                        board[i][j].currentState = Square.SquareColor.Grey;
                    }
                }
                board[i][j].posX = i;
                board[i][j].posY = j;
            }
        }


        //contar elementos adyacentes de la fila y columna
        //el tablero es cuadrado asi que se puede hacer asi
        for(int i = 0; i < board[0].length; ++i){
            countRow(i);//cuenta los adyacentes azules que hay en esa fila
            countCol(i);//cuenta los adyacentes azules que hay en esa columna
        }

        for(int i = 0; i < board[0].length; ++i) {
            for (int j = 0; j < board[0].length; ++j) {
                board[i][j].total = board[i][j].row + board[i][j].column;
            }
        }

        reveal();
    }

    public String giveHint(){
        for(Square s : locked){
            if(hint3(s)){
                System.out.println("Pista 3 aceptada en "+s.posX + " "+ s.posY);
            }
        }
        return "lamo";
    }

    //devuelve true si ve suficiente para cerrarlo.
    private boolean hint1(Square square){
        boolean check = square.total == square.playerColumn + square.playerRow;
        if(check) {
            for (Dirs d : Dirs.values()) {
                int x = square.posX;
                int y = square.posY;
                while (board[x][y].currentState == Square.SquareColor.Blue) {
                    x += d.getRow();
                    y += d.getCol();
                }
                if(board[x][y].currentState == Square.SquareColor.Grey){
                    return true;
                }
            }
        }
        return  false;
    }

    //devuelve true si veria demasiados.
    private boolean hint2(Square square){
        for (Dirs d : Dirs.values()) {
            int x = square.posX;
            int y = square.posY;
            while (board[x][y].currentState == Square.SquareColor.Blue) {
                x += d.getRow();
                y += d.getCol();
            }
            if(board[x][y].currentState == Square.SquareColor.Grey &&   // Hay un Grey seguido do un Blue
                    board[x += d.getRow()][y += d.getCol()].currentState == Square.SquareColor.Blue){
                if(d == Dirs.UP || d == Dirs.DOWN){ // Columna
                    if((board[x][y].playerColumn + (square.playerRow + square.playerColumn + 1))
                            > (square.total)) return true;    // Más grande que la solución
                }else{                              // Fila
                    if((board[x][y].playerRow + (square.playerRow + square.playerColumn + 1))
                            > (square.total)) return true;    // Más grande que la solución
                }
            }
        }
        return false;
    }

    private boolean hint3(Square square){
        //guardamos los que puede ver y los que ya ve por cada direccion

        Integer[][] posAdyCount = new Integer[4][2];
        int i=0;
        int posCount=0;
        int blueCount=0;
        for(Dirs d: Dirs.values()){
            int x = square.posX + d.getRow();
            int y = square.posY + d.getCol();
            while(board[x][y].currentState != Square.SquareColor.Red){
                if(board[x][y].currentState == Square.SquareColor.Blue)
                    blueCount++;
                posCount++;
                x += d.getRow();
                y += d.getCol();
            }

            posAdyCount[i][0]=posCount;
            posAdyCount[i][1]=blueCount;
            posCount=blueCount=0;
            i++;
        }
        //ordenamos de menor a mayor los posibles azules menos los que ya son azules
        for( i = 0; i < posAdyCount[0].length -1; ++i){
            for(int j = 0; j < posAdyCount[0].length -1; ++j){
                int tmp = posAdyCount[j][0] - posAdyCount[j][1];
                int aux = posAdyCount[j+1][0] - posAdyCount[j+1][1];
                if(tmp > aux){
                    tmp = posAdyCount[j][0];
                    aux = posAdyCount[j][1];
                    posAdyCount[j][0] = posAdyCount[j+1][0];
                    posAdyCount[j][1] = posAdyCount[j+1][1];

                    posAdyCount[j+1][0] = tmp;
                    posAdyCount[j+1][1] = aux;
                }
            }
        }//ya esta ordenado de menor a mayor

        //sumamos los posibles en todas las direcciones menos
        // la que estamos comprobando (tiene mas posibles)
        int count=0;
        for(i=0;i<3;++i){
            count += posAdyCount[i][0];
        }
        return count < square.total;
    }

    public void print(){
        for(int i = 0; i < board[0].length ; ++i){
            for(int j = 0; j < board[1].length ; ++j){
                System.out.print(board[i][j].solutionState + " ");
            }
            System.out.println();
        }
        /*for(int i = 0; i < 4 ; ++i){
            for(int j = 0; j < 4 ; ++j){
                System.out.print(logic.tablero[i][j].estadoActual + " ");
            }
            System.out.println();
        }*/
        System.out.println();
        for(int i = 0; i < board[0].length ; ++i){
            for(int j = 0; j < board[1].length ; ++j){
                System.out.print((board[i][j].total) + " " );
            }
            System.out.println();
        }
        System.out.println();

        //muestra el estado del tablero
        for(int i = 0; i < board[0].length ; ++i){
            for(int j = 0; j < board[1].length ; ++j){
                if(board[i][j].lock)
                    System.out.print(board[i][j].total+" ");
                else if(board[i][j].currentState == Square.SquareColor.Blue)
                    System.out.print("A ");
                else if(board[i][j].currentState == Square.SquareColor.Grey)
                    System.out.print("0 ");
                if(board[i][j].currentState == Square.SquareColor.Red)
                    System.out.print("R ");
            }
            System.out.println();
        }
    }

    //recibe el tablero y la fila a calcular
    public void countRow(int i){
        int j=0;
        while(j != board[0].length){
            if(board[i][j].row == 0 && board[i][j].solutionState == Square.SquareColor.Blue)
                countRowRec(i, j,0);
            j++;
        }
    }

    //recibe el tablero, la fila, la columna y el numero que ha contado de adyacentes en esa fila
    //la condicion de parada es encontrar una casilla roja o el final
    private int countRowRec(int i, int j, int cont){
        if(j == board[0].length-1) {//si llegamos al final
            board[i][j].row = cont;
            return cont;
        }
        if (board[i][j + 1].solutionState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
            board[i][j].row = countRowRec(i, j+1, ++cont);
        else//si el siguiente es rojo, dejo de aumentar adyacentes
            board[i][j].row = cont;
        //aqui se llega al encontrar un rojo.
        return board[i][j].row;
    }

    //recibe el tablero y la columna a calcular el numero de azules adyacentes
    public void countCol(int j){
        int i=0;
        while(i != board[0].length){
            if(board[i][j].column == 0 && board[i][j].solutionState == Square.SquareColor.Blue)
                countColRec(i, j,0);
            i++;
        }
    }
    //recibe el tablero, la fila, la columna y el numero que ha contado de adyacentes en esa columna
    //la condicion de parada es encontrar una casilla roja o el final
    private int countColRec(int i, int j, int cont){
        if(i == board[0].length-1) {//si llegamos al final
            board[i][j].column = cont;
            return cont;
        }
        if (board[i + 1][j].solutionState == Square.SquareColor.Blue)//si el siguiente es azul, aumentar el contador de adyacentes
            board[i][j].column = countColRec(i+1, j, ++cont);
        else//si el siguiente es rojo, dejo de aumentar adyacentes
            board[i][j].column = cont;
        //aqui se llega al encontrar un rojo.
        return board[i][j].column;
    }

    //recibe el tablero y la fila a calcular
    private void markShowInRow(int i){
        int j=0;
        if(board[i][j].solutionState == Square.SquareColor.Blue /*&& !tablero[i][j].showInColumn*/){
            board[i][j].lock = true;
            locked.add(board[i][j]);
            board[i][j].showInRow = true;
            board[i][j].currentState = board[i][j].solutionState;
        }
        j++;
        while(board[0].length != j){
            if(!board[i][j].lock  && board[i][j].solutionState == Square.SquareColor.Blue) {
                if(board[i][j-1].showInRow  || board[i][j-1].lock)//el anterior es azul
                    board[i][j].showInRow = true;
                else{//el anterior es rojo
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
        int i=0;
        if(board[i][j].solutionState == Square.SquareColor.Blue){
            board[i][j].lock = true;
            locked.add(board[i][j]);
            board[i][j].showInColumn = true;
            board[i][j].currentState = board[i][j].solutionState;
        }
        i++;
        while( board[0].length != i){
            if(!board[i][j].lock && board[i][j].solutionState == Square.SquareColor.Blue) {
                if(board[i-1][j].showInColumn  || board[i-1][j].lock )//el anterior es azul
                    board[i][j].showInColumn = true;
                else{//el anterior es rojo
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
        int revCount = 0;

        for(int i = 0; i < size ; ++i){
            for(int j = 0; j < size ; ++j){
                // comprobar que no se ha generado aislado
                if((board[i][j].total) == 0 && board[i][j].solutionState == Square.SquareColor.Blue){
                    board[i][j].solutionState = Square.SquareColor.Red;
                }
            }
        }

        for(int i = 0; i < board[0].length; ++i){
            markShowInRow(i);   //revela los necesarios en filas
            markShowInCol(i);   //revela los nocesarios en columnas
        }

    }
}

