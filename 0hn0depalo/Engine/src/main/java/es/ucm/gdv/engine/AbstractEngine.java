package es.ucm.gdv.engine;


/**
 * Clase abstracta que implementa Engine.
 * Tiene la funcionalidad comun que implementan los
 * motores de Pc y Android.
 */
public abstract class AbstractEngine implements Engine{
    protected AbstractGraphics _graphics;
    protected Input _input;
    protected Scene _scene;
    protected long _lastFrameTime;

    /**
     * Cambia la escena sobra la que se ejecuta el bucle principal.
     * El cambio se hara efectivo en la siguiente iteracion.
     *
     * @param scene La nueva escena
     */
    public void setScene(Scene scene){
        _scene = scene;
        _scene.setEngine(this);
        _scene.start();
    }

    /**
     * Devuelve la instancia del "motor" grafico
     *
     * @return Graphics para gestionar el renderizado
     */
    public Graphics getGraphics(){
        return _graphics;
    };

    /**
     * Devuelve la instancia del gestor de entrada
     *
     * @return Input para gestionar la entrada
     */
    public Input getInput(){
        return _input;
    };

    /**
     * Bucle ppal del motor.
     * Pasamos la escena para que si hay un cambio de escena
     * se termine la ejecucion del frame en la escena actual y se produzca
     * el cambio de escena en el siguiente frame.
     */
    public void mainLoop(Scene currentScene){
        currentScene.handleInput();
        update();
        _graphics.render(currentScene);
    }

    /**
     * Calcula el deltaTime y se lo pasa al update de la aplicacion
     */
    private void update(){
        long currentTime = System.nanoTime();
        long nanoElapsedTime = currentTime - _lastFrameTime;
        _lastFrameTime = currentTime;
        float elapsedTime = (float) (nanoElapsedTime / 1.0E9);

        _scene.update(elapsedTime);
    };

}
