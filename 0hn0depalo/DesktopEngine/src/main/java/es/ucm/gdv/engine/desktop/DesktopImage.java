package es.ucm.gdv.engine.desktop;

import es.ucm.gdv.engine.Image;
import sun.security.krb5.internal.crypto.Des;

import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;

public class DesktopImage implements Image {
    String _name = null;
    BufferedImage _bufferedImage;
    float _canvasWidth,_canvasHeight;

    public DesktopImage(){ }

    public boolean load(String s){
        try {
            _bufferedImage = ImageIO.read(new File(s));
        } catch (IOException e) {
            System.err.println("Error cargando la imagen: " + e);
            return false;
        }
        _name = s;
        return true;
    }

    public BufferedImage getBufferedImage() { return _bufferedImage; }

    @Override
    public String getName() {
        return _name;
    }

    @Override
    public float getWidth() {
        return _bufferedImage.getWidth();
    }

    @Override
    public float getHeight() {
        return _bufferedImage.getHeight();
    }

    @Override
    public float getCanvasHeight() {
        return _canvasHeight;
    }
    @Override
    public float getCanvasWidth(){
        return _canvasWidth;
    }
    public void setCanvasWidthHeight(float width, float height){
        _canvasHeight = height;
        _canvasWidth = width;
    }
}
