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

    public DesktopImage(String s){
        try {
            _bufferedImage = ImageIO.read(new File(s));
        } catch (IOException e) {
        }
        _name = s;
    }

    public BufferedImage get_bufferedImage() { return _bufferedImage; }

    @Override
    public String get_name() {
        return _name;
    }

    @Override
    public float get_width() {
        return _bufferedImage.getWidth();
    }

    @Override
    public float get_height() {
        return _bufferedImage.getHeight();
    }
}
