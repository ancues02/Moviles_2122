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
    public JFrame _window;
    private Color _currentColor;

    @Override
    protected void rescale() {
        _window.setSize((int)_realX, (int)_realY);
    }

    @Override
    public Image newImage(String name) {
        Image img = new DesktopImage();
        img.load(name);
        return img;
    }

    @Override
    public Font newFont(String filename, float size, boolean isBold) {
        return null;
    }

    @Override
    public void clear(int r, int g, int b, int a){
        _window.getContentPane().invalidate();
        _window.getContentPane().validate();
        _window.getContentPane().repaint();
        _window.setBackground(new Color(r, g, b, a));
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
        BufferedImage img = null;
        try {
            img = ImageIO.read(new File(image.get_name()));
        } catch (IOException e) {
        }
        _window.getGraphics().drawImage(img, 0, 0, null);
    }

    @Override
    public void drawImage(Image image, float scaleX, float scaleY) {

    }

    @Override
    public void drawImage(Image image, float scaleX, float scaleY, float transX, float transY){

    }

    @Override
    public void setColor(int r, int g, int b, int a){
        _window.getGraphics().setColor(new Color(r, g, b, a));
    }

    @Override
    public void fillCircle(float cx, float cy, float radius) {
        float rX = cx * _scale, rY = cy * _scale;   // Se ajusta a la escala puesta al canvas
        if(!_verticalCompensation) rX += (_window.getX() - _realX) / 2; // Que se salte la barra
        else rY += (_window.getY() - _realY) / 2;
        /*_window.getGraphics().drawOval((int)rX, (int)rY,
                (int)radius * 2,(int)radius * 2); No necesario (sólo es circumferencia)*/
        _window.getGraphics().fillOval((int)rX, (int)rY,
                (int)radius * 2,(int)radius * 2);
        _window.getContentPane().paint(_window.getGraphics());  // Necesario para que se vea????
    }

    @Override
    public void drawText(String text, float x, float y) {

    }

    public void createWindow(int x, int y) {
        //_realX = x; _realY = y;
        _window = new JFrame("0hn0 de palo");
        _window.setSize(x, y);
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
