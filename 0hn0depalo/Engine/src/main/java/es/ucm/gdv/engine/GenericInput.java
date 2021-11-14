package es.ucm.gdv.engine;

import java.util.List;

public class GenericInput implements Input{

    protected List<TouchEvent> _touchEventList;
    protected Pool<TouchEvent> _eventPool;

    /**
     * Tiene que ser sincronizado porque la hebra de UI lo añade
     * pero la  hebra de logica los recoge.
     * @return Devuelve la lista de eventos
     */
    @Override
    synchronized public List<TouchEvent> getTouchEvents() {
        return _touchEventList;
    }

    /**
     * Elimina un evento de input de la lista
     * y de la pool de eventos
     */
    protected void popEvent(TouchEvent touchEvent) {
        _touchEventList.remove(touchEvent);
        _eventPool.release(touchEvent);
    }

    /**
     * Elimina todos los evento de input de la
     * lista y de la pool de eventos
     */
     synchronized public void flushEvents() {
        TouchEvent touchEvent;
        int i = _touchEventList.size();
        while (i > 0) {
            i--;
            _eventPool.release(_touchEventList.get(i));
            _touchEventList.remove(i);
        }
    }
}
