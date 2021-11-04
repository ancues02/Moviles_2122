package es.ucm.gdv.engine.android;

import es.ucm.gdv.engine.Engine;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Input;

public class AndroidEngine implements Engine {
    private AndroidGraphics _graphics;  // has the SurfaceHolder
    private AndroidInput _input;
    private GameLoopThread _gameLoopTh; // implements runnable

    @Override
    public Graphics getGraphics() {
        return null; //_graphics;
    }

    @Override
    public Input getInput() {
        return null; //_input;
    }

    /*
    * A la constructora del engine le llega el contexto para crear
    * el SurfaceView desde Graphics.
    * Graphics tiene el SurfaceView y lo relacionado con el paint y canvas
    * GameLoopThread es el thread que implementa en su run el bucle principal
    * El engine se encarga de gestionar el thread cuando se destruye y se vuelve
    * a crear la applicacion de android
    *
    * class GameLoopThread implements Runnable{
    * AndroidGraphics _graphics;
    *
    * void run(){
    *   while(_running){
    *       update();   // calcula el deltaTime y llama al update del app
    *
    *       render();   // llama al render de Graphics que se encarga de hacer
    *                   // lockear el canvas, hacer el render del app y despues el flip
    *   }
    * }
    * }
    */



    public void launch(){
        // ¿es necesario?
    }

    public void resume(){
        _gameLoopTh.resume();
    }
    public void pause(){
        _gameLoopTh.pause();
    }
}
