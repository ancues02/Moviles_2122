package es.ucm.gdv.engine.desktop;

import java.awt.event.MouseEvent;
import java.util.List;

import javax.swing.JFrame;

import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;

public class DesktopInput implements Input,  java.awt.event.MouseListener {
    private DesktopGraphics _dGraphics;

    public DesktopInput(DesktopGraphics dGraphics){
        _dGraphics = dGraphics;
        _dGraphics.getWindow().addMouseListener(this);
    }

    @Override
    public void mouseClicked(MouseEvent mouseEvent) { }

    @Override
    public void mousePressed(MouseEvent mouseEvent) {
        float posX = _dGraphics.realToVirtualX(mouseEvent.getX());
        float posY = _dGraphics.realToVirtualY(mouseEvent.getY());
        System.out.println("Input Pressed on canvas: " + posX + " " + posY + ".");
    }

    @Override
    public void mouseReleased(MouseEvent mouseEvent) {

    }

    @Override
    public void mouseEntered(MouseEvent mouseEvent) {

    }

    @Override
    public void mouseExited(MouseEvent mouseEvent) {

    }

    @Override
    public List<TouchEvent> getTouchEvents() {
        return null;
    }
}
