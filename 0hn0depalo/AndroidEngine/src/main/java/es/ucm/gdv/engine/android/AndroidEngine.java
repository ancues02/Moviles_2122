package es.ucm.gdv.engine.android;

import es.ucm.gdv.engine.GenericEngine;

import androidx.appcompat.app.AppCompatActivity;

/**
 * Clase que implementa el motor para Android.
 *
 * Se encarga de propocionar las instancias del motor
 * de renderizado y el gestor de entrada. Ademas se ocupa
 * de toda la gestion del bucle principal del juego en
 * una hebra distinta y proporciona los metodos para
 * gestionar esta hebra teniendo el cuenta el ciclo de vida
 * de una actividad en android.
 */
public class AndroidEngine extends GenericEngine implements Runnable{
    // Atributos

    // Objeto Thread que ejecutara el bucle principal en una hebra distinta
    Thread _gameLoopTh;

    // Flag que indica si se esta ejecutando el hilo del bucle principal.
    // Se usa para sincronizacion entre threads y por eso es volatile.
    protected volatile boolean _running;

    /**
     * Constructor
     * Crea las instancias del motor grafico y el gestor de entrada especificas de Android
     *
     * @param activity La actividad asociada. Necesaria para la creacion del motor grafico
     * @param virtualHeight Anchura del canvas virtual deseado para la aplicacion
     * @param virtualHeight Altura del canvas virtual deseado para la aplicacion
     */
    public AndroidEngine(AppCompatActivity activity, int virtualWidth, int virtualHeight){
        super();
        _graphics = new AndroidGraphics(activity,virtualWidth,virtualHeight);
        _input = new AndroidInput((AndroidGraphics)_graphics);
        _running = false;
    }

    /**
     * Método llamado para solicitar que se continue con el
     * bucle principal del juego.
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

            //System.out.println("GameLoopThread initiated");
        }
    }

    /**
     * Método llamado para detener el bucle principal del juego.
     *
     * Se hace así intencionadamente, para bloquear la hebra de UI
     * temporalmente y evitar potenciales situaciones de carrera.
     */
    public void pause(){
        if (_running) {
            _running = false;
            while (true) {
                try {
                    _gameLoopTh.join();
                    _gameLoopTh = null;
                    //System.out.println("GameLoopThread terminated");
                    break;
                } catch (InterruptedException ie) {
                    // Esto no debería ocurrir nunca...
                }
            }
        }
    }

    // Metodo del interfaz Runnable

    /**
     * Método que implementa el bucle principal del juego y que será
     * ejecutado en otra hebra.
     * NO debe ser llamado desde el exterior.
     */
    @Override
    public void run() {
        if (_gameLoopTh != Thread.currentThread()) {
            // programacion defensiva
            throw new RuntimeException("run() should not be called directly");
        }
        while(_running && _graphics.getWidth() == 0)//sleep
            ;
        // Cuando ya se ha inicializado la vista, ajustamos el canvas virtual
        ((AndroidGraphics)_graphics).adjustCanvasToView();
        // Ahora si podemos lanzar el bucle
        _lastFrameTime = System.nanoTime();
        while(_running) {
            mainLoop(_scene);
        }
    }
}
