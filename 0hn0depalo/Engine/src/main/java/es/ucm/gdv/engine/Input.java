package es.ucm.gdv.engine;

import java.util.List;

public interface Input {
    List<TouchEvent> getTouchEvents();

    /** Elimina un evento de input de la lista
     * y de la pool de eventos
     * */
    void popEvent(TouchEvent touchEvent);

    /** Elimina todos los evento de input de la
     * lista y de la pool de eventos
     * */
    void flushEvents();
}