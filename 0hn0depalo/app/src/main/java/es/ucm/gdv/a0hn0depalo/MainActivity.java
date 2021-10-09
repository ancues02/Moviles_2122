package es.ucm.gdv.a0hn0depalo;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;

import java.util.Random;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        init(4);
        for(int i = 0; i < 4; ++i){
            for(int j =0 ; j < 4; ++j){
                System.out.print(tablero[i][j].col + tablero[i][j].fila + " ");
            }
            System.out.println();
        }
    }

    //crear tablero inicial
    private void init(int tam){
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
                if(rnd.nextFloat() < 0.5)
                    tablero[i][j].azul=false;
                else
                    tablero[i][j].azul=true;
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
    private void cuentaFila(Casilla[][] tablero, int i){
        int j=0;
        while(j != tablero[0].length){
            if(tablero[i][j].fila == 0 && tablero[i][j].azul)
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
        if (tablero[i][j+1].azul)//si el siguiente es azul, aumentar el contador de adyacentes
            tablero[i][j].fila = cuentaFilaRec(tablero, i, j+1, ++cont);
        else//si el siguiente es rojo, dejo de aumentar adyacentes
            tablero[i][j].fila = cont;
        //aqui se llega al encontrar un rojo.
        return cont;
    }

    //recibe el tablero y la columna a calcular el numero de azules adyacentes
    private void cuentaCol(Casilla[][] tablero, int j){
        int i=0;
        while(i != tablero[0].length){
            if(tablero[i][j].col == 0 && tablero[i][j].azul)
                cuentaColRec(tablero, i, j,0);
            i++;
        }
    }
    //recibe el tablero, la fila, la columna y el numero que ha contado de adyacentes en esa columna
    //la condicion de parada es encontrar una casilla roja o el final
    private int cuentaColRec(Casilla[][] tablero, int i, int j,int cont){
        if(i == tablero[0].length-1) {//si llegamos al final
            tablero[i][j].col = cont;
            return cont;
        }
        if (tablero[i+1][j].azul)//si el siguiente es azul, aumentar el contador de adyacentes
            tablero[i][j].col = cuentaColRec(tablero, i+1, j, ++cont);
        else//si el siguiente es rojo, dejo de aumentar adyacentes
            tablero[i][j].col = cont;
        //aqui se llega al encontrar un rojo.
        return cont;
    }

    Casilla[][] tablero;

}
class Casilla{
    public int fila = 0;
    public int col = 0;
    public boolean azul = false;
}

