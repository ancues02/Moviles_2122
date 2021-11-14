package es.ucm.gdv.engine.desktop;

import java.awt.Color;
import java.awt.event.ComponentEvent;
import java.awt.event.ComponentListener;
import java.awt.font.FontRenderContext;
import java.awt.geom.Rectangle2D;
import java.awt.image.BufferStrategy;
import javax.swing.JFrame;

import es.ucm.gdv.engine.*;


/**
 * Clase que implementa motor grafico en Pc
 */
public class DesktopGraphics extends GenericGraphics {
    // Atributos

    private JFrame _window;
    private BufferStrategy _strategy;
    private java.awt.Graphics _graphics;

    public DesktopGraphics() {
        super();
    }

    /**
     * Llama a la creación de la ventana y establece
     * las dimensiones del canvas.
     *
     * @param windowName Nombre de la ventana
     * @param x Ancho de la ventana
     * @param y Alto de la ventana
     * @param virtualX Ancho del canvas virtual
     * @param virtualY Alto del canvas virtual
     * @return Devuelve si ha fallado o no la inicialización
     */
    public boolean init(String windowName, int x, int y, int virtualX, int virtualY){
        setCanvasDimensions(virtualX,virtualY);
        return createWindow(windowName, x, y);
    }

    // Metodos del interfaz Graphics

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
    public void clear() {
        _graphics.setColor(new Color(_bR, _bG, _bB, _bA));
        _graphics.fillRect(0, 0, _window.getWidth(), _window.getHeight());
    }

    // Metodos vacios porque no son necesarios para nuestra implementacion

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

    @Override
    public Image newImage(String filename) {
        DesktopImage di = null;
        if(!_images.containsKey(filename)) {
            di = new DesktopImage();
            if (!di.load(filename )) {
                return null;
            }
            _images.put(filename,di);
        }
        else
            di=(DesktopImage) _images.get(filename);

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

    @Override
    public void drawImage(Image image, float scaleX, float scaleY, float percentX, float percentY){
        if(image == null) return;

        DesktopImage bi = (DesktopImage)image;

        float rX = virtualToRealX(percentX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);

        float sX = (bi.getWidth() * scaleX * _scale);
        float sY = (bi.getHeight() * scaleY * _scale);

        bi.setCanvasWidthHeight(sX/getCanvasWidth()  ,sY/getCanvasHeight() );
        _graphics.drawImage(bi.getBufferedImage(),
                (int)(rX  - (sX/2)), (int)(rY - (sY/2)),
                (int)(sX),
                (int)(sY),
                null);
    }

    @Override
    public void drawImage(Image image, int sizeX, int sizeY, float percentX, float percentY){
        if(image == null) return;

        DesktopImage bi = (DesktopImage)image;

        float rX = virtualToRealX(percentX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);

        float sX = (sizeX * _scale);
        float sY = (sizeY * _scale);


        _graphics.drawImage(bi.getBufferedImage(), (int)(rX  - (sX/2)), (int)(rY - (sY/2)),
                (int)(sX),
                (int)(sY),
                null);

    }

    @Override
    public Font newFont(String filename, float size, boolean isBold) {
        DesktopFont df = null;
        if(!_fonts.containsKey(filename)) {
            df = new DesktopFont();

            if (!df.load(filename)) {
                return null;
            }
            _fonts.put(filename, df);
        }
        else
            df=(DesktopFont)_fonts.get(filename);


        df.setBold(isBold);
        df.setSize(size);
        return df;
    }

    @Override
    public void setFont(Font font){
        if(font == null) return;

        DesktopFont df = (DesktopFont)font;
        df.setSize(df.getSize()*_scale);
        df.setBold(df.getBold());
        _graphics.setFont(df.getFont());
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
    public void drawCircle(float percentX, float percentY, float radius) {
        float rX = virtualToRealX(percentX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);
        float radiusReal = radius  * _virtualX;
        _graphics.drawOval((int)(rX - (radiusReal * _scale)),
                (int)(rY - (radiusReal * _scale)),
                (int)(radiusReal * 2 * _scale),
                (int)(radiusReal * 2 * _scale));
    }

    @Override
    public void fillCircle(float percentX, float percentY, float radius) {
        float rX = virtualToRealX(percentX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);
        float radiusReal = radius  * _virtualX;
        _graphics.fillOval((int)(rX - (radiusReal * _scale)),
                (int)(rY - (radiusReal * _scale)),
                (int)(radiusReal * 2 * _scale), (int)(radiusReal * 2 * _scale));
    }

    // Metodos propios de la clase

    /**
     * Crea la ventana y el strategy buffer
     * para el renderizado activo.
     * Tambien se registra la ventana para que escuche
     * los eventos de reescalado y pueda reaccionar a ellos.
     *
     * @param windowName Nombre de la ventana
     * @param x Ancho de la ventana
     * @param y Alto de la ventana
     * @return Devuelve si ha fallado o no la incialización
     */
    public boolean createWindow(String windowName, int x, int y) {
        _window = new JFrame(windowName);

        _window.setSize(x, y);
        _window.setIgnoreRepaint(true);
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
            return false;
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
        return true;
    }

    /**
     * Devuelve la ventana
     *
     * @return La ventana
     */
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

    @Override
    public void render(Scene app) {
        do {
            do {
                _graphics = _strategy.getDrawGraphics();
                try {
                    //System.out.println(_window.getX() + " ahora height " + get_windowY());
                    clear();
                    app.render();
                    fillOffsets();
                } finally {
                    _graphics.dispose();
                }
            } while (_strategy.contentsRestored());
            _strategy.show();
        } while (_strategy.contentsLost());
    }

    @Override
    protected void fillOffsets(){
        _graphics.setColor(new Color(_bR, _bG, _bB, _bA));
        if(_verticalCompensation)
            fillVerticalOffsets();
        else
            fillHorizontalOffsets();
    }

    //Rellena de blanco por los lados
    @Override
    protected void fillHorizontalOffsets(){
        _graphics.fillRect(0, 0,
                (int) virtualToRealX(0),
                _window.getHeight());
        _graphics.fillRect((int)(_window.getWidth() - _realX) / 2 + (int)_realX, 0,
                _window.getWidth(), _window.getHeight());
    }

    @Override
    protected void fillVerticalOffsets(){
        _graphics.fillRect(0, 0,
                _window.getWidth(),
                (int) virtualToRealY(0));

        _graphics.fillRect(0, (int)(_window.getHeight() - _realY) / 2 + (int)_realY,
                _window.getWidth(), _window.getHeight());
    }
}
