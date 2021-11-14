package es.ucm.gdv.engine;

import java.util.Collections;
import java.util.List;


/**
 * Clase abstracta que implementa Input.
 * Tiene la funcionalidad comun que implementan nuestros
 * gestores de entrada de Pc y Android.
 * Esta funcionalidad es:
 *   ·  La lista y el pool de eventos
 *   ·  Los metodos getTouchEvents y flushEvents
 */
public abstract class GenericInput implements Input{

    protected List<TouchEvent> _touchEventList;
    protected Pool<TouchEvent> _eventPool;

    /**
     * Devuelve los eventos capturados.
     *
     * Tiene que ser sincronizado porque la hebra de UI lo añade
     * pero la  hebra de logica los recoge.
     *
     * Devolvemos la lista solo de lectura, para que
     * desde la logica no pueda modificar y se queden eventos
     * sin liberar de la pool.
     *
     * @return Devuelve la lista de eventos solo de lectura
     */
    @Override
    synchronized public List<TouchEvent> getTouchEvents() {
        return Collections.unmodifiableList(_touchEventList);
    }

    /**
     * Elimina todos los eventos de entrada
     * de la lista y de la pool de eventos.
     * Se llama despues de cada handleInput de la escena,
     * asi nos aseguramos que la lista se vacia cada
     * frame y el pool se libera.
     *
     * Sincronizado para evitar accesos concurrentes
     * a la lista, por si se está añadiendo un evento mientras
     * se elimina toda la lista.
     */
     synchronized public void flushEvents() {
        int i = _touchEventList.size();
        while (i > 0) {
            i--;
            _eventPool.release(_touchEventList.get(i));
            _touchEventList.remove(i);
        }
    }
}
