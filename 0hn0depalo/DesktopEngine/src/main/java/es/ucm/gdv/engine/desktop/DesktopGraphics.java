package es.ucm.gdv.engine.desktop;

import java.awt.Color;
import java.awt.Graphics2D;
import java.awt.event.ComponentEvent;
import java.awt.event.ComponentListener;
import java.awt.font.FontRenderContext;
import java.awt.geom.AffineTransform;
import java.awt.geom.Rectangle2D;
import java.awt.image.BufferStrategy;

import es.ucm.gdv.engine.*;

import javax.swing.JFrame;

public class DesktopGraphics extends AbstractGraphics {
    private JFrame _window;
    private BufferStrategy _strategy;
    private java.awt.Graphics _graphics;
    private AffineTransform _savedTransform;

    public DesktopGraphics(int x, int y, int virtualX, int virtualY) {
        super();
        setCanvasDimensions(virtualX,virtualY);
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
    public float getCanvasWidth(){
        return _realX;
    }

    @Override
    public float getCanvasHeight(){
        return _realY;
    }

    @Override
    public void clear(int r, int g, int b, int a) {
        _graphics.setColor(new Color(r, g, b, a));
        _graphics.fillRect(0, 0, _window.getWidth(), _window.getHeight());

        // TODO: Quitar esto para no ver el canvas virtual
        float rX = virtualToRealX(0);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(0);
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
        _savedTransform = ((Graphics2D)_graphics).getTransform();
    }

    @Override
    public void restore() {
        ((Graphics2D)_graphics).setTransform(_savedTransform);
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
    public void drawImage(Image image, float scaleX, float scaleY, float percentX, float percentY){
        if(image == null) return;

        DesktopImage bi = (DesktopImage)image;

        float rX = virtualToRealX(percentX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);

        float sX = (bi.getWidth() * scaleX * _scale);
        float sY = (bi.getHeight() * scaleY * _scale);

        //save();
        //_graphics.translate((int)(rX  - (sX/2)), (int)(rY - (sY/2)));

        _graphics.drawImage(bi.get_bufferedImage(),
                (int)(rX  - (sX/2)), (int)(rY - (sY/2)),
                (int)(sX),
                (int)(sY),
                null);

        //restore();
        //_graphics.translate((int)-(rX  - (sX/2)), (int)-(rY - (sY/2)));
    }

    // Dibuja una imagen del tamaño dado en la posicion dada
    @Override
    public void drawImage(Image image, int sizeX, int sizeY, float percentX, float percentY){
        if(image == null) return;

        DesktopImage bi = (DesktopImage)image;

        float rX = virtualToRealX(percentX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);

        _graphics.translate((int)(rX - (sizeX * _scale)/2), (int)(rY - (sizeY * _scale)));

        _graphics.drawImage(bi.get_bufferedImage(), 0, 0,
                (int)(sizeX * _scale), (int)(sizeY * _scale),
                null);

        _graphics.translate((int) -(rX - (sizeX * _scale)/2), (int) -(rY - (sizeY * _scale)));
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
    public void drawText(String text, float pX, float pY) {
        float rX = virtualToRealX(pX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(pY * _virtualY);

        FontRenderContext frc = new FontRenderContext(null, true, true);
        Rectangle2D r2D = _graphics.getFont().getStringBounds(text, frc);

        int rWidth = (int) Math.round(r2D.getWidth());
        int rHeight = (int) Math.round(r2D.getHeight());
        int tX = (int) Math.round(r2D.getX());
        int tY = (int) Math.round(r2D.getY());

        int a = (int)(rX - (rWidth / 2) - tX);
        int b = (int)(rY - (rHeight / 2) - tY);

        _graphics.drawString(text, a, b);
    }

    @Override
    public void setColor(int r, int g, int b, int a){
        _graphics.setColor(new Color(r, g, b, a));
    }

    @Override
    public void fillCircle(float percentX, float percentY, float radius) {
        //setColor(255, 0, 0, 255);
        float rX = virtualToRealX(percentX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);
        float radiusReal = radius  * _virtualX;
        _graphics.fillOval((int)(rX - (radiusReal * _scale)),
                (int)(rY - (radiusReal * _scale)),
                (int)(radiusReal * 2 * _scale), (int)(radiusReal * 2 * _scale));
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

        //_window.addMouseListener(new DesktopInput(_window)) ;

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

    public JFrame getWindow(){
        return _window;
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
                    fillOffsets(Color.white);
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

    private void fillOffsets(Color c){
        if(_verticalCompensation)
            fillVerticalOffsets(c);
        else
            fillHorizontalOffsets(c);
    }

    //Rellena de blanco por los lados
    private void fillHorizontalOffsets(Color c){
        _graphics.setColor(c);
        _graphics.fillRect(0, 0,
                (int) virtualToRealX(0),
                _window.getHeight());
        _graphics.fillRect((int)(_window.getWidth() - _realX) / 2 + (int)_realX, 0,
                _window.getWidth(), _window.getHeight());
    }

    private void fillVerticalOffsets(Color c){
        _graphics.setColor(c);
        _graphics.fillRect(0, 0,
                _window.getWidth(),
                (int) virtualToRealY(0));

        _graphics.fillRect(0, (int)(_window.getHeight() - _realY) / 2 + (int)_realY,
                _window.getWidth(), _window.getHeight());
    }
}
