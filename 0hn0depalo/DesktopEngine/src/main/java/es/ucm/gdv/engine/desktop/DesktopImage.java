package es.ucm.gdv.engine.desktop;

import es.ucm.gdv.engine.Image;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;

public class DesktopImage implements Image {
    String _name = null;
    float _width, _height;

    @Override
    public void load(String path) {
        _name = path;
    }

    @Override
    public String get_name() {
        return _name;
    }

    @Override
    public float get_width() {
        return _width;
    }

    @Override
    public float get_height() {
        return _height;
    }
}
