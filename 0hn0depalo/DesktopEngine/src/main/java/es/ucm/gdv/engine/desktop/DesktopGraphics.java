package es.ucm.gdv.engine.desktop;

import java.awt.Color;
import java.awt.image.BufferedImage;
import java.awt.image.BufferStrategy;

import es.ucm.gdv.engine.*;

import javax.swing.JFrame;
import javax.swing.JPanel;

public class DesktopGraphics extends AbstractGraphics {
    private JFrame _window;
    private BufferStrategy _strategy;
    private java.awt.Graphics _graphics;

    public DesktopGraphics(int x, int y, int virtualX, int virtualY) {
        super(virtualX, virtualY);
        createWindow(x, y);
    }

    public void setSize(int width, int height){
        _window.setSize(width, height);
        adjustCanvasToSize(width, height);
        //_window.setResizable(true);
        //_window.getContentPane().addComponentListener(_window);
    }
    @Override
    protected void rescale() {
        //_window.setSize((int)_realX, (int)_realY);  //TESTING: PARA VER EL CANVAS
    }

    @Override
    public Image newImage(String name) {
        return new DesktopImage(name);
    }

    @Override
    public Font newFont(String filename, float size, boolean isBold) {
        return null;
    }

    @Override
    public void clear(int r, int g, int b, int a) {
        _graphics.setColor(new Color(r, g, b, a));
        _graphics.fillRect(0, 0, _window.getWidth(), _window.getHeight());

        float rX = compensateX(0, _window.getWidth());     // Se ajusta a la escala puesta al canvas
        float rY = compensateY(0, _window.getHeight());
        _graphics.setColor(new Color(180, 180, 180, 255));
        _graphics.fillRect((int) rX, (int) rY,
                (int) _realX, (int) _realY);
    }

    private void fillOffsets(){ // TODO: Hacer la parte de compensacion vertical
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
    public void drawImage(Image image) {
        drawImage(image, 1, 1, 0, 0);
    }

    @Override
    public void drawImage(Image image, float scaleX, float scaleY) {
        drawImage(image, scaleX, scaleY, 0, 0);
    }

    @Override
    public void drawImage(Image image, float scaleX, float scaleY, float transX, float transY){
        BufferedImage bi = ((DesktopImage)image).get_bufferedImage();

        float rX = compensateX(transX, _window.getWidth());     // Se ajusta a la escala puesta al canvas
        float rY = compensateY(transY, _window.getHeight());

        _graphics.drawImage(bi,
                (int)rX, (int)rY,
                (int)(bi.getWidth() * scaleX * _scale), (int)(bi.getHeight() * scaleY * _scale),
                null);
    }

    @Override
    public void setColor(int r, int g, int b, int a){
        _graphics.setColor(Color.RED);
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

    @Override
    public void drawText(String text, float x, float y) {

    }

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
    }

    @Override
    public float getWidth(){
        return _virtualX;
    };

    @Override
    public float getHeight(){
        return _virtualY;
    }
}
