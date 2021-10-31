package es.ucm.gdv.engine.desktop;

import org.graalvm.compiler.lir.amd64.AMD64MathIntrinsicBinaryOp;

import java.io.FileInputStream;
import java.io.InputStream;

import es.ucm.gdv.engine.Font;

public class DesktopFont implements Font {
    private java.awt.Font _font;

    private String _filename;

    public DesktopFont(){ }

    public boolean load(String s){
        try (InputStream is = new FileInputStream(s)) {
            _font = java.awt.Font.createFont(java.awt.Font.TRUETYPE_FONT, is);
        }
        catch (Exception e) {
            System.err.println("Error cargando la fuente: " + e);
            return false;
        }
        _filename = s;
        return true;
    }

    @Override
    public String getFilename() {
        return _filename;
    }

    public java.awt.Font get_font(){
        return _font;
    }

    @Override
    public void setBold(boolean bold){
        if(bold)
            _font = _font.deriveFont(java.awt.Font.BOLD);
        else
            _font = _font.deriveFont(java.awt.Font.PLAIN);
    }

    @Override
    public void setSize(int size){
        _font = _font.deriveFont(size);
    }

}
