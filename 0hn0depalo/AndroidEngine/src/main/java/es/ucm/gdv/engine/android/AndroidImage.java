package es.ucm.gdv.engine.android;

import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;

import es.ucm.gdv.engine.Image;

public class AndroidImage implements Image {
    String _name = null;
    Bitmap _bitmap;
    float _canvasWidth,_canvasHeight;

    public AndroidImage(){ }

    public boolean load(String s, AssetManager as){
        try(InputStream is = as.open(s)) {
            _bitmap = BitmapFactory.decodeStream(is);
        }
        catch (IOException e) {
            //TODO: usar Logcat?
            System.err.println("Error cargando la imagen: " + e);
            return false;
        }
        _name = s;
        return true;
    }

    public Bitmap getBitmap() { return _bitmap; }

    @Override
    public String getName() {
        return _name;
    }

    @Override
    public float getWidth() {
        return _bitmap.getWidth();
    }

    @Override
    public float getHeight() {
        return _bitmap.getHeight();
    }

    @Override
    public float getCanvasWidth() {
        return _canvasWidth;
    }
    @Override
    public float getCanvasHeight() {
        return _canvasHeight;
    }

    public void setCanvasWidthHeight(float width, float height){
        _canvasHeight = height;
        _canvasWidth = width;
    }

}
