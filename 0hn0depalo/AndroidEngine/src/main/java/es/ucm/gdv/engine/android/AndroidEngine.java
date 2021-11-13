package es.ucm.gdv.engine.android;

import es.ucm.gdv.engine.AbstractEngine;

import androidx.appcompat.app.AppCompatActivity;

public class AndroidEngine extends AbstractEngine implements Runnable{
    Thread _gameLoopTh;
    protected volatile boolean _running;

    public AndroidEngine(AppCompatActivity activity, int virtualWidth, int virtualHeight){
        super();
        _graphics = new AndroidGraphics(activity,virtualWidth,virtualHeight);
        _input = new AndroidInput((AndroidGraphics)_graphics);
        _running = false;
    }

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


    @Override
    public void run() {
        if (_gameLoopTh != Thread.currentThread()) {
            // programacion defensiva
            throw new RuntimeException("run() should not be called directly");
        }
        while(_running && _graphics.getWidth() == 0)//sleep
            ;
        ((AndroidGraphics)_graphics).adjustCanvasToView();
        // Ahora si podemos lanzar el bucle
        _lastFrameTime = System.nanoTime();
        while(_running) {
            mainLoop(_scene);
        }
    }
}
