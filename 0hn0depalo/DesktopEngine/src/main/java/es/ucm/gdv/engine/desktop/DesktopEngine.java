package es.ucm.gdv.engine.desktop;

import es.ucm.gdv.engine.AbstractEngine;

public class DesktopEngine extends AbstractEngine {

    public DesktopEngine(int width, int height, int virtualWidth, int virtualHeight){
        super();
        DesktopGraphics dg = new DesktopGraphics(width, height, virtualWidth, virtualHeight);
        _graphics = dg;
        _input = new DesktopInput(dg);
    }

    public void setSize(int width, int height) {
        ((DesktopGraphics)_graphics).setSize(width, height);
    }

    public void run(){
        _lastFrameTime = System.nanoTime();
        while(true) {
            mainLoop(_scene);
        }
    }
}
