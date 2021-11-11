package es.ucm.gdv.ohno;

import es.ucm.gdv.engine.Graphics;

public class Square {
    public enum SquareColor {
        Grey (240,240,240),
        Blue (10,180,235),
        Red(255,78,72);

        private final int r, g, b;

        SquareColor(int red, int green, int blue){
            this.r = red;
            this.g = green;
            this.b = blue;
        }

        public int getR() {
            return r;
        }

        public int getG() {
            return g;
        }

        public int getB() {
            return b;
        }
    }

    public int posX,posY; //posicion en el tablero

    public int row = 0;                 //adyacentes en esa fila
    public int column = 0;              //adyacentes en esa columna
    public int total = 0;               //adyacentes totales (suma de row + column)
    public SquareColor solutionState;   //tiene que ser rojo para ganar
    public boolean lock = false;        //los que ve el jugador


    public int playerRow = 0;           //adyacentes en esa fila
    public int playerColumn = 0;        //adyacentes en esa columna
    public SquareColor currentState;    //el color que tiene en ese momento (rojo, azul o gris)

    public boolean showInRow = false;
    public boolean showInColumn = false;

    public void render(Graphics g){

    }

}
