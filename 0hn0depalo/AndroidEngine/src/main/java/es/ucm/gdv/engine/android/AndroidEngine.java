package es.ucm.gdv.engine.android;

import android.content.Context;
import android.content.res.AssetManager;
import android.view.View;

import es.ucm.gdv.engine.Application;
import es.ucm.gdv.engine.Engine;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Input;

public class AndroidEngine implements Engine, Runnable{
    Thread _gameLoopTh;
    volatile boolean _running;

    private AndroidGraphics _graphics;  // has the SurfaceHolder
    private AndroidInput _input;
    private Application _app;

    private long _lastFrameTime;

    public AndroidEngine(Context context){
        _graphics = new AndroidGraphics(context);
        _input = new AndroidInput();
    }

    @Override
    public Graphics getGraphics() {
        return _graphics;
    }

    @Override
    public Input getInput() {
        return _input;
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

    public void resume(){
        if (!_running) {
            // Solo hacemos algo si no nos estábamos ejecutando ya
            // (programación defensiva, nunca se sabe quién va a
            // usarnos...)
            _running = true;
            // Lanzamos la ejecución de nuestro método run()
            // en una hebra nueva.
            _gameLoopTh = new Thread(this);
            _gameLoopTh.start();

            System.out.println("GameLoopThread initiated");

        }
    }

    public void pause(){
        if (_running) {
            _running = false;
            while (true) {
                try {
                    _gameLoopTh.join();
                    _gameLoopTh = null;
                    System.out.println("GameLoopThread terminated");
                    break;
                } catch (InterruptedException ie) {
                    // Esto no debería ocurrir nunca...
                }
            }
        }
    }
    private void handleInput(){

    }

    private void update(){
        long currentTime = System.nanoTime();
        long nanoElapsedTime = currentTime - _lastFrameTime;
        _lastFrameTime = currentTime;
        float elapsedTime = (float) (nanoElapsedTime / 1.0E9);

        _app.update(elapsedTime);
    }

    private void render(){
        _graphics.render(_app);
    }

    @Override
    public void run() {
        if (_gameLoopTh != Thread.currentThread()) {
            // programacion defensiva
            throw new RuntimeException("run() should not be called directly");
        }
        while(_running && _graphics.getWidth() == 0)//sleep
            ;
        _graphics.adjustCanvasToView();
        // Ahora si podemos lanzar el bucle
        _lastFrameTime = System.nanoTime();
        while(_running) {
            handleInput();
            update();
            render();
        }
    }

    public View getContentView(){
        return _graphics.getSurfaceView();
    }

    public void setApplication(Application app){
        _app = app;
    }
}
