package es.ucm.gdv.engine;

/**
 * Clase abstracta que implementa Engine.
 * Tiene la funcionalidad comun que implementan
 * nuestros motores de Pc y Android.
 * Esta funcionalidad es:
 *   ·  Las instancias del motor grafico,
 *      el gestor de entrada y la escena.
 *   ·  El bucle principal y el calculo del deltaTime para
 *      el update.
 */
public abstract class GenericEngine implements Engine{
    // Atributos

    protected GenericGraphics _graphics;   // Instancia del motor grafico (tiene que tener un metodo render)
    protected GenericInput _input;         // Instancia del gestor de entrada
    protected Scene _scene;
    protected long _lastFrameTime;


    // Metodos del inerfaz Engine

    /**
     * Devuelve la instancia del "motor" grafico
     *
     * @return Graphics para gestionar el renderizado
     */
    @Override
    public Graphics getGraphics(){
        return _graphics;
    };

    /**
     * Devuelve la instancia del gestor de entrada
     *
     * @return Input para gestionar la entrada
     */
    @Override
    public Input getInput(){
        return _input;
    };


    /**
     * Cambia la escena sobra la que se ejecuta el bucle principal.
     * El cambio se hara efectivo en la siguiente iteracion.
     *
     * @param scene La nueva escena
     */
    @Override
    public void setScene(Scene scene){
        _scene = scene;
        _scene.setEngine(this);
        _scene.start();
    }

    // Metodos de la clase

    /**
     * Bucle ppal del motor.
     * Pasamos la escena para que si hay un cambio de escena
     * se termine la ejecucion del frame en la escena actual y se produzca
     * el cambio de escena en el siguiente frame.
     */
    public void mainLoop(Scene currentScene){
        currentScene.handleInput();
        _input.flushEvents();
        update();
        _graphics.render(currentScene);
    }

    /**
     * Calcula el deltaTime y se lo pasa al update de la aplicacion
     */
    protected void update(){
        long currentTime = System.nanoTime();
        long nanoElapsedTime = currentTime - _lastFrameTime;
        _lastFrameTime = currentTime;
        float elapsedTime = (float) (nanoElapsedTime / 1.0E9);

        _scene.update(elapsedTime);
    };

}
