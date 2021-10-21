package es.ucm.gdv.launcher.android;

import java.util.Random;

public class Logic {
    public Logic (){

    }
    //crear tablero inicial
    public void init(int tam){
        tablero = new Casilla [tam][tam];
        for(int i = 0; i < tablero[0].length; ++i){
            for(int j =0 ; j < tablero[0].length; ++j) {
                tablero[i][j] = new Casilla();
            }
        }
        Random rnd = new Random();
        //rellenamos el tablero con azules y rojos de forma aleatoria
        for(int i = 0; i < tablero[0].length; ++i){
            for(int j =0 ; j < tablero[0].length; ++j){
                if(rnd.nextFloat() < 0.5) {
                    tablero[i][j].estadoSolucion = Casilla.ColorCasilla.Azul;
                    tablero[i][j].estadoActual = Casilla.ColorCasilla.Gris;
                }
                else {
                    tablero[i][j].estadoSolucion = Casilla.ColorCasilla.Rojo;
                    tablero[i][j].estadoActual = Casilla.ColorCasilla.Gris;
                }
            }
        }

        //contar elementos adyacentes de la fila y columna
        //el tablero es cuadrado asi que se puede hacer asi
        for(int i = 0; i < tablero[0].length; ++i){
            cuentaFila(tablero,i);//cuenta los adyacentes azules que hay en esa fila
            cuentaCol(tablero,i);//cuenta los adyacentes azules que hay en esa columna
        }



    }

    //recibe el tablero y la fila a calcular
    public void cuentaFila(Casilla[][] tablero, int i){
        int j=0;
        while(j != tablero[0].length){
            if(tablero[i][j].fila == 0 && tablero[i][j].estadoSolucion == Casilla.ColorCasilla.Azul)
                cuentaFilaRec(tablero, i, j,0);
            j++;
        }
    }
    //recibe el tablero, la fila, la columna y el numero que ha contado de adyacentes en esa fila
    //la condicion de parada es encontrar una casilla roja o el final
    private int cuentaFilaRec(Casilla[][] tablero, int i, int j,int cont){
        if(j == tablero[0].length-1) {//si llegamos al final
            tablero[i][j].fila = cont;
            return cont;
        }
        if (tablero[i][j + 1].estadoSolucion == Casilla.ColorCasilla.Azul)//si el siguiente es azul, aumentar el contador de adyacentes
            tablero[i][j].fila = cuentaFilaRec(tablero, i, j+1, ++cont);
        else//si el siguiente es rojo, dejo de aumentar adyacentes
            tablero[i][j].fila = cont;
        //aqui se llega al encontrar un rojo.
        return tablero[i][j].fila;
    }

    //recibe el tablero y la columna a calcular el numero de azules adyacentes
    public void cuentaCol(Casilla[][] tablero, int j){
        int i=0;
        while(i != tablero[0].length){
            if(tablero[i][j].column == 0 && tablero[i][j].estadoSolucion == Casilla.ColorCasilla.Azul)
                cuentaColRec(tablero, i, j,0);
            i++;
        }
    }
    //recibe el tablero, la fila, la columna y el numero que ha contado de adyacentes en esa columna
    //la condicion de parada es encontrar una casilla roja o el final
    private int cuentaColRec(Casilla[][] tablero, int i, int j,int cont){
        if(i == tablero[0].length-1) {//si llegamos al final
            tablero[i][j].column = cont;
            return cont;
        }
        if (tablero[i + 1][j].estadoSolucion == Casilla.ColorCasilla.Azul)//si el siguiente es azul, aumentar el contador de adyacentes
            tablero[i][j].column = cuentaColRec(tablero, i+1, j, ++cont);
        else//si el siguiente es rojo, dejo de aumentar adyacentes
            tablero[i][j].column = cont;
        //aqui se llega al encontrar un rojo.
        return tablero[i][j].column;
    }

    //recibe el tablero y la fila a calcular
    public void markShowInRow(Casilla[][] tablero, int i){
        int j=0;
        if(tablero[i][j].estadoSolucion == Casilla.ColorCasilla.Azul /*&& !tablero[i][j].showInColumn*/){
            tablero[i][j].lock = true;
        }
        j++;
        while(tablero[0].length != j){
            if(!tablero[i][j].lock  && tablero[i][j].estadoSolucion == Casilla.ColorCasilla.Azul ) {
                if(tablero[i][j-1].showInRow  || tablero[i][j-1].lock  )
                    tablero[i][j].showInRow = true;
                else{
                    tablero[i][j].lock = true;
                    tablero[i][j].showInRow = true;
                }
            }
            j++;
        }
    }

    public void markShowInCol(Casilla[][] tablero, int j){
        int i=0;
        if(tablero[i][j].estadoSolucion == Casilla.ColorCasilla.Azul){
            tablero[i][j].lock = true;
        }
        i++;
        while( tablero[0].length != i){
            if(!tablero[i][j].lock && tablero[i][j].estadoSolucion == Casilla.ColorCasilla.Azul ) {
                if(tablero[i-1][j].showInRow  || tablero[i-1][j].lock  )
                    tablero[i][j].showInRow = true;
                else{
                    tablero[i][j].lock = true;
                    tablero[i][j].showInRow = true;
                }
            }
            i++;
        }
    }

   /* //recibe el tablero y la columna a calcular el numero de azules adyacentes
    public void markShowInCol(Casilla[][] tablero, int j){
        int i=0;
        while(i != tablero[0].length){
            if(tablero[i][j].column == 0 && tablero[i][j].estadoSolucion == Casilla.ColorCasilla.Azul)
                markShowInColRec(tablero, i, j,0);
            i++;
        }
    }
    //recibe el tablero, la fila, la columna y el numero que ha contado de adyacentes en esa columna
    //la condicion de parada es encontrar una casilla roja o el final
    private int markShowInColRec(Casilla[][] tablero, int i, int j,int cont){
        if(i == tablero[0].length-1) {//si llegamos al final
            tablero[i][j].column = cont;
            return cont;
        }
        if (tablero[i + 1][j].estadoSolucion == Casilla.ColorCasilla.Azul)//si el siguiente es azul, aumentar el contador de adyacentes
            tablero[i][j].column = cuentaColRec(tablero, i+1, j, ++cont);
        else//si el siguiente es rojo, dejo de aumentar adyacentes
            tablero[i][j].column = cont;
        //aqui se llega al encontrar un rojo.
        return tablero[i][j].column;
    }*/

    private void reveal(Casilla[][] tablero){
        int size = tablero[0].length;
        Random r = new Random();
        int revCount = 0;

        for(int i = 0; i < size ; ++i){
            for(int j = 0; j < size ; ++j){
                // comprobar que no se ha generado aislado
                if((tablero[i][j].fila + tablero[i][j].column) == 0 && tablero[i][j].estadoSolucion == Casilla.ColorCasilla.Azul){
                    tablero[i][j].estadoSolucion = Casilla.ColorCasilla.Rojo;
                }
                // revelar

            }
            System.out.println();
        }

        //if(revCount < revLimit)
    }

    public Casilla[][] tablero;
    public float revealProb = 0.2f;
    private float revealLimit = 0.3f;
}

