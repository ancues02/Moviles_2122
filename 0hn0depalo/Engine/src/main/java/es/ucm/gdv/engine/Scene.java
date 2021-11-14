package es.ucm.gdv.engine;

/**
 * Interfaz que representa las escenas del juego.
 * La logica del juego debera implementar esta clase.
 */
public interface Scene {

    /**
     * Inicializacion de la escena.
     *
     * Se puede usar para la carga de recursos
     */
    void start();

    /**
     * Manejo de la entrada
     */
    void handleInput();

    /**
     * Actualizacion de la logica
     *
     * @param deltaTime el deltaTime
     */
    void update(float deltaTime);

    /**
     * El dibujado de la escena
     */
    void render();

    /**
     * Establece el motor de la escena
     *
     * @param engine el motor
     */
    void setEngine(Engine engine);
}