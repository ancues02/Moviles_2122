package es.ucm.gdv.engine.desktop;

import java.awt.event.MouseEvent;
import java.util.LinkedList;
import java.util.List;

import es.ucm.gdv.engine.GenericInput;
import es.ucm.gdv.engine.Pool;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

/**
 * Se encarga de recoger, analizar y devolver los eventos que ocurren en PC
 */
public class DesktopInput extends GenericInput implements java.awt.event.MouseListener, java.awt.event.MouseMotionListener{
    private DesktopGraphics _dGraphics;

    public DesktopInput(DesktopGraphics dGraphics){
        super();
        _dGraphics = dGraphics;
        _dGraphics.getWindow().addMouseListener(this);
        _dGraphics.getWindow().addMouseMotionListener(this);
        _touchEventList = new LinkedList<TouchEvent>();
        _eventPool = new Pool<TouchEvent>(50, () -> { return new TouchEvent(); } );
    }

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
