package es.ucm.gdv.engine.desktop;

import es.ucm.gdv.engine.AbstractEngine;

public class DesktopEngine extends AbstractEngine {

    public DesktopEngine(int width, int height, int virtualWidth, int virtualHeight){
        super();
        //TODO: No castear.
        _graphics = new DesktopGraphics(width, height, virtualWidth, virtualHeight);
        _input = new DesktopInput((DesktopGraphics)_graphics);
    }

    public void setSize(int width, int height) {
        ((DesktopGraphics)_graphics).setSize(width, height);
    }

}
