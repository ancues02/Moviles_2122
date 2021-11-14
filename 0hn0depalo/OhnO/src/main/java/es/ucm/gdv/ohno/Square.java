package es.ucm.gdv.ohno;

import es.ucm.gdv.engine.Graphics;

public class Square extends FadeObject {

    public Square(){
        super(255.0f, 0.2f);
        this.alphaTime = animTime;
        this.wiggleTime = this.animWiggleTime = 1f;
    }

    public Square(float iniAlpha, float animTime, float animWiggleTime){
        super(iniAlpha, animTime);
        this.alphaTime = animTime;
        this.wiggleTime = this.animWiggleTime = animWiggleTime;
    }


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
    public boolean drawBlack = false;   //para rodearlo de negro (pista, deshacer...)

    public int playerRow = 0;           //adyacentes en esa fila
    public int playerColumn = 0;        //adyacentes en esa columna
    public SquareColor currentState;    //el color que tiene en ese momento (rojo, azul o gris)
    public SquareColor prevState;
    public boolean showInRow = false;
    public boolean showInColumn = false;

    public float sizeMult = 1.0f;
    private float animWiggleTime;
    private float wiggleTime;
    private float alphaTime;
    private boolean disapear = false;

    @Override
    public void update(float deltaTime) {
        // Animar fadeOut
        if(alphaTime < animTime) {
            alphaTime += deltaTime;
            fadeOut(deltaTime);
        }

        // Animar la vibracion
        if(wiggleTime < animWiggleTime){
            wiggleTime += deltaTime;
            wiggleTime = Math.min(wiggleTime, animWiggleTime); // para que no se pase
            sizeMult = (float)(0.2 * Math.sin(6.28319 * (wiggleTime / animWiggleTime)) + 1);
        }
    }

    public void render(Graphics g, float px, float py, float rad){

        if(drawBlack){
            g.setColor(0, 0, 0, 255);
            g.fillCircle(px, py, rad * sizeMult *1.1f);
        }

        if(!disapear) {
            // draw currentState
            g.setColor(currentState.r, currentState.g, currentState.b, 255);
            g.fillCircle(px, py, rad * sizeMult);

            // draw prevState if fading
            if (alphaTime < animTime) {
                if (prevState != null)
                    g.setColor(prevState.r, prevState.g, prevState.b, (int) alpha);
                else//programacion defensiva
                    g.setColor(currentState.r, currentState.g, currentState.b, (int) alpha);
                g.fillCircle(px, py, rad * sizeMult);
            }
        }
        else{
            g.setColor(currentState.r, currentState.g, currentState.b, (int)alpha);
            g.fillCircle(px, py, rad * sizeMult);
        }

        if(lock && solutionState == SquareColor.Blue){
            if(!disapear)
                g.setColor(255,255,255,255);
            else
                g.setColor(255,255,255,(int)alpha);

            g.drawText(String.valueOf(total), px, py);
        }
    }

    // Restea el timer de alpha para que se haga
    public void beginFading(){
        alphaTime = 0;
        alpha = 255.0f;
    }

    // Restea el timer de alpha para que se haga
    public void beginFading2(){
        alphaTime = 0;
        alpha = 255.0f;
        disapear = true;
    }

    // resetea el timer de vibración para que se haga
    public void beginVibration(){
        wiggleTime = 0;
    }
}
