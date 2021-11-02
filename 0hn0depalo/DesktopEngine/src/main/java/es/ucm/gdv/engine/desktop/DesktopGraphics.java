package es.ucm.gdv.engine.desktop;

import java.awt.Color;
import java.awt.event.ComponentEvent;
import java.awt.event.ComponentListener;
import java.awt.image.BufferStrategy;

import es.ucm.gdv.engine.*;

import javax.swing.JFrame;

public class DesktopGraphics extends AbstractGraphics {
    private JFrame _window;
    private BufferStrategy _strategy;
    private java.awt.Graphics _graphics;

    public DesktopGraphics(int x, int y, int virtualX, int virtualY) {
        super(virtualX, virtualY);
        createWindow(x, y);
    }

    // Metodos de la interfaz Graphics

    @Override
    public float getWidth(){
        return _window.getWidth();
    };

    @Override
    public float getHeight(){
        return _window.getHeight();
    }

    @Override
    public void clear(int r, int g, int b, int a) {
        _graphics.setColor(new Color(r, g, b, a));
        _graphics.fillRect(0, 0, _window.getWidth(), _window.getHeight());

        // TODO: Quitar esto para no ver el canvas virtual
        float rX = compensateX(0, _window.getWidth());     // Se ajusta a la escala puesta al canvas
        float rY = compensateY(0, _window.getHeight());
        _graphics.setColor(new Color(180, 180, 180, 255));
        _graphics.fillRect((int) rX, (int) rY,
                (int) _realX, (int) _realY);
    }

    @Override
    public void translate(float x, int y) {

    }

    @Override
    public void scale(float x, float y) {

    }

    @Override
    public void save() {

    }

    @Override
    public void restore() {

    }

    // Metodos sobre imagenes

    // Metodo factoria que crea una imagen, devuelve null si no ha podido crearla
    @Override
    public Image newImage(String filename) {
        DesktopImage di = new DesktopImage();
        if(!di.load(filename)){
            return null;
        }

        return di;
    }

    @Override
    public void drawImage(Image image) {
        drawImage(image, 1, 1, 0, 0);
    }

    @Override
    public void drawImage(Image image, float scaleX, float scaleY) {
        drawImage(image, scaleX, scaleY, 0, 0);
    }

    // Dibuja una imagen escalada en la posicion dada
    @Override
    public void drawImage(Image image, float scaleX, float scaleY, float transX, float transY){
        if(image == null) return;

        DesktopImage bi = (DesktopImage)image;

        float rX = compensateX(transX, _window.getWidth());     // Se ajusta a la escala puesta al canvas
        float rY = compensateY(transY, _window.getHeight());

        float sX = (bi.getWidth() * scaleX * _scale);
        float sY = (bi.getHeight() * scaleY * _scale);
        _graphics.drawImage(bi.get_bufferedImage(),
                (int)(rX  - (sX/2)), (int)(rY - (sY/2)),
                (int)(sX),
                (int)(sY),
                null);
    }

    // Dibuja una imagen del tamaño dado en la posicion dada
    @Override
    public void drawImage(Image image, int sizeX, int sizeY, float transX, float transY){
        if(image == null) return;

        DesktopImage bi = (DesktopImage)image;

        float rX = compensateX(transX, _window.getWidth());     // Se ajusta a la escala puesta al canvas
        float rY = compensateY(transY, _window.getHeight());

        _graphics.drawImage(bi.get_bufferedImage(),
                (int)(rX - (sizeX * _scale)/2), (int)(rY - (sizeY * _scale)),
                (int)(sizeX * _scale), (int)(sizeY * _scale),
                null);
    }

    // Metodos sobre fonts

    // Metodo factoria que crea una font, devuelve null si no ha podido crearla
    @Override
    public Font newFont(String filename, int size, boolean isBold) {

        DesktopFont df = new DesktopFont();
        if(!df.load(filename)){
            return null;
        }

        df.setBold(isBold);
        df.setSize(size);
        return df;
    }

    @Override
    public void setFont(Font font){
        if(font == null) return;

        DesktopFont df = (DesktopFont)font;
        _graphics.setFont(df.get_font());
    }

    @Override
    public void drawText(String text, int x, int y) {
        int rX = (int)compensateX(x, _window.getWidth());     // Se ajusta a la escala puesta al canvas
        int rY = (int)compensateY(y, _window.getHeight());
        _graphics.drawString(text, rX, rY);
    }

    @Override
    public void setColor(int r, int g, int b, int a){
        _graphics.setColor(new Color(r, g, b, a));
    }

    @Override
    public void fillCircle(float cx, float cy, float radius) {
        //setColor(255, 0, 0, 255);
        float rX = compensateX(cx, _window.getWidth());     // Se ajusta a la escala puesta al canvas
        float rY = compensateY(cy, _window.getHeight());
        _graphics.fillOval((int)(rX - (radius * _scale)),
                (int)(rY - (radius * _scale)),
                (int)(radius * 2 * _scale),(int)(radius * 2 * _scale));
    }

    // Metodos propios de la clase

    public void createWindow(int x, int y) {
        _window = new JFrame("0hn0 de palo");

        _window.setSize(x, y);
        _window.setIgnoreRepaint(true);
        //_window.setExtendedState(JFrame.MAXIMIZED_BOTH);  // FULLSCREEN
        //_window.setUndecorated(true);
        _window.setVisible(true);
        _window.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);

        int intentos = 100;
        while(intentos-- > 0) {
            try {
                _window.createBufferStrategy(2);
                break;
            }
            catch(Exception e) {
            }
        } // while pidiendo la creación de la buffeStrategy
        if (intentos == 0) {
            System.err.println("No pude crear la BufferStrategy");
            return;
        }

        _strategy = _window.getBufferStrategy();
        adjustCanvasToSize(x, y);

        // Clase anonima para detectar los eventos de reescalado
        _window.addComponentListener(new ComponentListener() {
            @Override
            public void componentMoved(ComponentEvent componentEvent) {}
            @Override
            public void componentShown(ComponentEvent componentEvent){}
            @Override
            public void componentHidden(ComponentEvent componentEvent) {}
            @Override
            public void componentResized(ComponentEvent componentEvent) {
                int nWidth = componentEvent.getComponent().getWidth();
                int nHeight = componentEvent.getComponent().getHeight();
                adjustCanvasToSize(nWidth, nHeight);
            }
        });
    }

    /**
     * Cambia el tamaño de la ventana
     *
     * @param width ancho de la ventana
     * @param height alto de la ventana
     */
    public void setSize(int width, int height){
        _window.setSize(width, height);
        adjustCanvasToSize(width, height);
    }

    public void render(Application app) {
        do {
            do {
                _graphics = _strategy.getDrawGraphics();
                try {
                    //System.out.println(_window.getX() + " ahora height " + get_windowY());
                    clear(255, 255, 255, 255);
                    app.render(this);
                    fillOffsets();
                } finally {
                    _graphics.dispose();
                }
            } while (_strategy.contentsRestored());
            _strategy.show();
        } while (_strategy.contentsLost());
    }

    /*public float getRealX(){
        return _realX;
    }

    public float getRealY(){
        return _realY;
    }*/

    private void fillOffsets(){
        if(_verticalCompensation)
            fillVerticalOffsets();
        else
            fillHorizontalOffsets();
    }

    //Rellena de blanco por los lados
    private void fillHorizontalOffsets(){
        _graphics.setColor(new Color(255, 255, 255, 255));
        _graphics.fillRect(0, 0,
                (int)compensateX(0, _window.getWidth()),
                _window.getHeight());
        _graphics.fillRect((int)(_window.getWidth() - _realX) / 2 + (int)_realX, 0,
                _window.getWidth(), _window.getHeight());
    }

    private void fillVerticalOffsets(){
        _graphics.setColor(new Color(255, 255, 255, 255));
        _graphics.fillRect(0, 0,
                _window.getWidth(),
                (int)compensateY(0, _window.getHeight()));

        _graphics.fillRect(0, (int)(_window.getHeight() - _realY) / 2 + (int)_realY,
                _window.getWidth(), _window.getHeight());
    }
}
