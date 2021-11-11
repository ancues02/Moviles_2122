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
    public SquareColor prevState;
    public boolean showInRow = false;
    public boolean showInColumn = false;


    public float sizeMult = 1.2f;

    public final float animTime = 1.0f;
    public float alpha = 255.0f;

    public void update(float deltaTime){
        alpha -= deltaTime * (255.0f / animTime);
    }

    public void render(Graphics g, float px, float py, float rad){
        // draw prevState if fading
        int alphaVal = (int)alpha;
        if(alphaVal > 255) {
            g.setColor(prevState.r, prevState.g, prevState.b, (int) alpha);
            g.fillCircle(px, py, rad);
        }

        // draw currentState
        g.setColor(currentState.r, currentState.g, currentState.b, 255);
        g.fillCircle(px, py, rad);

        if(lock && solutionState == SquareColor.Blue){
            g.drawText(String.valueOf(total), px, py);
        }
    }

}
