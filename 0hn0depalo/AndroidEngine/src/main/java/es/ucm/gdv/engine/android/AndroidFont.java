package es.ucm.gdv.engine.android;

import android.content.res.AssetManager;
import android.graphics.Typeface;
import android.util.Log;

import java.io.IOException;
import java.io.InputStream;

import es.ucm.gdv.engine.Font;

/**
 * Clase que guarda la fuente en un Typeface junto al resto de atributos propios de una fuente
 */
public class AndroidFont implements Font{
    private Typeface _font;
    private String _filename;
    private boolean _bold;
    private float _size;

    public AndroidFont () { }

    public boolean load(String s, AssetManager as){
        try(InputStream is = as.open(s)) {
            _font = Typeface.createFromAsset(as, s);
        }
        catch (IOException e) {
            Log.e("ERROR", "No se pudo cargar la fuente: " + e);
            return false;
        }
        _filename = s;
        return true;
    }

    public Typeface getFont(){
        return _font;
    }

    @Override
    public String getFilename() {
        return _filename;
    }

    @Override
    public void setBold(boolean bold) {
        _bold = bold;
    }

    @Override
    public void setSize(float size) {
        _size = size;
    }

    public boolean getBold(){
        return _bold;
    }

    public float getSize(){
        return _size;
    }
}
