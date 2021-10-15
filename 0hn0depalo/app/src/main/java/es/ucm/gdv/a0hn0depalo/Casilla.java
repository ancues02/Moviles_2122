package es.ucm.gdv.a0hn0depalo;

public class Casilla {
    public enum ColorCasilla{
        Rojo, Azul, Gris
    }

    public Casilla(){

    }
    public int fila = 0;//adyacentes en esa fila
    public int column = 0;//adyacentes en esa columna
    public ColorCasilla estadoSolucion;//tiene que ser rojo para ganar
    public boolean lock = false;


    public int filaPlayer = 0;//adyacentes en esa fila
    public int columnPlayer = 0;//adyacentes en esa columna
    public ColorCasilla estadoActual;//el color que tiene en ese momento (rojo, azul o gris)
}
