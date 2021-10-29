package es.ucm.gdv.engine.desktop;

import java.awt.Color;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

import es.ucm.gdv.engine.*;
import es.ucm.gdv.engine.desktop.*;

import javax.imageio.ImageIO;
import javax.swing.JFrame;
import javax.swing.JPanel;

public class DesktopGraphics extends AbstractGraphics {
    private JFrame _window;
    private JPanel _panel;
    private Color _currentColor;

    public DesktopGraphics(int x, int y, int virtualX, int virtualY){
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
    public void clear(int r, int g, int b, int a){
        _panel.invalidate();
        _panel.validate();
        _panel.repaint();
        _panel.setBackground(new Color(r, g, b, a));
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

        float rX = transX * _scale, rY = transY * _scale;   // Se ajusta a la escala puesta al canvas
        if(!_verticalCompensation) rX += (_window.getX() - _realX) / 2; // Que se salte la barra
        else rY += (_window.getY() - _realY) / 2;

        _window.getGraphics().drawImage(bi,
                (int)rX, (int)rY,
                (int)(bi.getWidth() * scaleX * _scale), (int)(bi.getHeight() * scaleY * _scale),
                null);
    }

    @Override
    public void setColor(int r, int g, int b, int a){
        _panel.getGraphics().setColor(new Color(r, g, b, a));
    }

    @Override
    public void fillCircle(float cx, float cy, float radius) {
        float rX = compensateX(cx, _window.getWidth());   // Se ajusta a la escala puesta al canvas
        float rY = compensateY(cy, _window.getHeight()); // Que se salte la barra
        /*_window.getGraphics().drawOval((int)rX, (int)rY,
                (int)radius * 2,(int)radius * 2); No necesario (sólo es circumferencia)*/
        _panel.getGraphics().fillOval((int)rX, (int)rY,
                (int)radius * 2,(int)radius * 2);
        //_panel.paint(_window.getGraphics());  // Necesario para que se vea????
    }

    @Override
    public void drawText(String text, float x, float y) {

    }

    public void createWindow(int x, int y) {
        _window = new JFrame("0hn0 de palo");
        _panel = new JPanel();
        _window.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        _window.setSize(x, y);
        _window.setVisible(true);
        _window.add(_panel);
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