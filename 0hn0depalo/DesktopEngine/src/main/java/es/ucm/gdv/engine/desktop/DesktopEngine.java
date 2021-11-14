package es.ucm.gdv.engine.desktop;

import es.ucm.gdv.engine.GenericEngine;

/**
 * Clase que implementa el motor para Pc.
 *
 * Se encarga de propocionar las instancias del motor
 * de renderizado y el gestor de entrada.
 */
public class DesktopEngine extends GenericEngine {

    /**
     * Constructor
     */
    public DesktopEngine(){
        super();
    }

    /**
     * Crea e inicializa las instancias del motor grafico y el gestor de entrada especificas de Pc
     *
     * @param width Anchura de la ventana.
     * @param height Altura de la ventana.
     * @param virtualHeight Anchura del canvas virtual deseado para la aplicacion.
     * @param virtualHeight Altura del canvas virtual deseado para la aplicacion.
     */
    public boolean init(String windowName, int width, int height, int virtualWidth, int virtualHeight){
        DesktopGraphics dg = new DesktopGraphics();
        if(!dg.init(windowName, width, height, virtualWidth, virtualHeight)) return false;
        _graphics = dg;
        _input = new DesktopInput(dg);
        return true;
    }

    /**
     * Establece las dimensiones de la ventana
     *
     * @param width Anchura de la ventana.
     * @param height Altura de la ventana.
     */
    public void setSize(int width, int height) {
        ((DesktopGraphics)_graphics).setSize(width, height);
    }

    /**
     * Lanza el bucle principal del juego
     */
    public void run(){
        _lastFrameTime = System.nanoTime();
        while(true) {
            mainLoop(_scene);
        }
    }
}
