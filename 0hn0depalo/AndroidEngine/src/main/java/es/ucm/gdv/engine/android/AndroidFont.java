package es.ucm.gdv.engine.android;

import android.content.res.AssetManager;
import android.graphics.BitmapFactory;
import android.graphics.Typeface;

import java.io.IOException;
import java.io.InputStream;

import es.ucm.gdv.engine.Font;

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
            //TODO: usar Logcat?
            System.err.println("Error cargando la fuente: " + e);
            return false;
        }
        _filename = s;
        return true;
    }

    public Typeface get_font(){
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
