package es.ucm.gdv.engine;

import java.util.List;

/**
 * Interfaz que representa el gestor de entrada
 * y permite abstraer la implementacion de la plataforma.
 */
public interface Input {

    /**
     * Devuelve una lista con los eventos capturados.
     *
     * @return la lista con los eventos
     */
    List<TouchEvent> getTouchEvents();
}