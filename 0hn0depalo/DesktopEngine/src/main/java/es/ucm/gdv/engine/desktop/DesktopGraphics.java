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
    private Application _app;
    private java.awt.Graphics _graphics;

    public DesktopGraphics(int x, int y, int virtualX, int virtualY) {
        super(virtualX, virtualY);
        createWindow(x, y);
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
        _strategy.getDrawGraphics().setColor(new Color(r, g, b, a));
        _strategy.getDrawGraphics().fillRect(0, 0, _window.getWidth(), _window.getHeight());
    }

    public void render() {
        do {
            do {
                _graphics = _strategy.getDrawGraphics();
                try {
                    //_app.;
                    fillCircle(0,0, 100);
                    fillCircle(200,200, 100);
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
        setColor(255, 0, 0, 255);
        float rX = compensateX(cx, _window.getWidth());     // Se ajusta a la escala puesta al canvas
        float rY = compensateY(cy, _window.getHeight());
        _graphics.fillOval((int)(rX - radius), (int)(rY - radius),
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
    public float get_windowX(){
        return _window.getX();
    };

    @Override
    public float get_windowY(){
        return _window.getY();
    }
}
