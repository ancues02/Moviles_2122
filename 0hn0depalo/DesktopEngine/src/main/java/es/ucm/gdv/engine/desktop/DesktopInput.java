package es.ucm.gdv.engine.desktop;

import java.awt.event.MouseEvent;
import java.util.List;

import javax.swing.JFrame;

import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

public class DesktopInput implements Input,  java.awt.event.MouseListener, java.awt.event.MouseMotionListener{
    private DesktopGraphics _dGraphics;
    private List<TouchEvent> _touchEventList;

    public DesktopInput(DesktopGraphics dGraphics){
        _dGraphics = dGraphics;
        _dGraphics.getWindow().addMouseListener(this);
        _dGraphics.getWindow().addMouseMotionListener(this);
    }

    @Override
    synchronized public List<TouchEvent> getTouchEvents() {

        return _touchEventList;
    }

    private void mouseEvent(MouseEvent mouseEvent, TouchType type){
        float posX = _dGraphics.realToVirtualX(mouseEvent.getX());
        float posY = _dGraphics.realToVirtualY(mouseEvent.getY());
        TouchEvent event = new TouchEvent(type, posX, posY, mouseEvent.getID());
        synchronized(this) {
            _touchEventList.add(event);
        }
    }

    @Override
    public void mousePressed(MouseEvent mouseEvent) {

        mouseEvent(mouseEvent, TouchType.Press);
    }

    @Override
    public void mouseReleased(MouseEvent mouseEvent) {
        mouseEvent(mouseEvent, TouchType.Release);
    }

    @Override
    public void mouseEntered(MouseEvent mouseEvent) {}

    @Override
    public void mouseExited(MouseEvent mouseEvent) {}

    @Override
    public void mouseClicked(MouseEvent mouseEvent) {}


    @Override
    public void mouseDragged(MouseEvent mouseEvent) {
        mouseEvent(mouseEvent, TouchType.Drag);
    }

    @Override
    public void mouseMoved(MouseEvent mouseEvent) {  }



}
