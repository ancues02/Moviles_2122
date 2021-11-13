package es.ucm.gdv.engine;

/**
 *  Interfaz que representa nuestro "motor"
 *
 *  Agrupa los recursos necesarios para el
 *  funcionamiento multiplataforma.
 */
public interface Engine {
    /**
     *  Devuelve el "motor" grafico
     *
     * @return la instancia del "motor" grafico
     */
    Graphics getGraphics();

    /**
     *  Devuelve el gestor de entrada
     *
     * @return la instancia del gestor de entrada
     */
    Input getInput();

    /**
     * Establece la escena que se ejecuta.
     * Si se hace durante la ejecucion, el cambio
     * no es efectivo hasta el siguiente frame
     *
     * @param scene La escena que se va a ejecutar
     */
    void setScene(Scene scene);
}