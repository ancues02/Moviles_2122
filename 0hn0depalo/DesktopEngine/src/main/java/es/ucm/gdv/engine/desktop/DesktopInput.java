package es.ucm.gdv.engine.desktop;

import java.awt.event.MouseEvent;
import java.util.LinkedList;

import es.ucm.gdv.engine.GenericInput;
import es.ucm.gdv.engine.Pool;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

/**
 * Se encarga de recoger, analizar y devolver los eventos que ocurren en PC
 */
public class DesktopInput extends GenericInput implements java.awt.event.MouseListener, java.awt.event.MouseMotionListener{
    private DesktopGraphics _dGraphics;

    /**
     * Constructor
     *
     * Crea la lista y el pool y se pone a escuchar
     * los eventos de la ventana.
     *
     * @param dGraphics "Motor" grafico del que se adquiere la ventana
     */
    public DesktopInput(DesktopGraphics dGraphics){
        super();
        _dGraphics = dGraphics;
        _dGraphics.getWindow().addMouseListener(this);
        _dGraphics.getWindow().addMouseMotionListener(this);
        _touchEventList = new LinkedList<TouchEvent>();
        _eventPool = new Pool<TouchEvent>(50, () -> { return new TouchEvent(); } );
    }

    /**
     * Añade un evento a la lista de eventos
     *
     * Se sincroniza el acceso a la pool y a la lista
     * para evitar accesos peligrosos en las diferentes hebras.
     *
     * @param mouseEvent Evento de raton detectado
     * @param type Tipo de evento
     */
    private void addEvent(MouseEvent mouseEvent, TouchType type){
        TouchEvent event;
        synchronized (this) {
            event = _eventPool.obtain();
        }
        if(event != null) {
            float posX = _dGraphics.realToVirtualX(mouseEvent.getX());
            float posY = _dGraphics.realToVirtualY(mouseEvent.getY());
            event.set(type,
                    posX, posY,
                    mouseEvent.getID(),
                    (mouseEvent.getButton() == MouseEvent.BUTTON1));
            synchronized (this) {
                _touchEventList.add(event);
            }
        }
    }

    @Override
    public void mousePressed(MouseEvent mouseEvent) {
        addEvent(mouseEvent, TouchType.Press);
    }

    @Override
    public void mouseReleased(MouseEvent mouseEvent) {
        addEvent(mouseEvent, TouchType.Release);
    }

    @Override
    public void mouseEntered(MouseEvent mouseEvent) {}

    @Override
    public void mouseExited(MouseEvent mouseEvent) {}

    @Override
    public void mouseClicked(MouseEvent mouseEvent) {}


    @Override
    public void mouseDragged(MouseEvent mouseEvent) {
        addEvent(mouseEvent, TouchType.Drag);
    }

    @Override
    public void mouseMoved(MouseEvent mouseEvent) {  }

}
