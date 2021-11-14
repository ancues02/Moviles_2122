package es.ucm.gdv.engine.android;

import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;

import es.ucm.gdv.engine.Image;

/**
 * Clase que guarda la iamgen en un Bitmap junto al resto de atributos propios de una imagen
 */
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
            Log.e("Error", "No se pudo cargar la imagen: " + e);
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
    public float getWidthInCanvas() {
        return _canvasWidth;
    }
    @Override
    public float getHeightInCanvas() {
        return _canvasHeight;
    }

    public void setCanvasWidthHeight(float width, float height){
        _canvasHeight = height;
        _canvasWidth = width;
    }

}
