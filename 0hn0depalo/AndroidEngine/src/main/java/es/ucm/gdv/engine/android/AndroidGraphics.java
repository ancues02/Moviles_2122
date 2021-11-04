package es.ucm.gdv.engine.android;

import android.graphics.Canvas;
import android.graphics.Paint;
import android.view.SurfaceHolder;

import es.ucm.gdv.engine.Application;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Image;

public class AndroidGraphics implements Graphics {

    private SurfaceHolder _holder;
    private Paint _paint;

    @Override
    public float getWidth() {
        return 0;
    }

    @Override
    public float getHeight() {
        return 0;
    }

    @Override
    public void clear(int r, int g, int b, int a) {

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
    public Image newImage(String name) {
        return null;
    }

    @Override
    public void drawImage(Image image) {

    }

    @Override
    public void drawImage(Image image, float scaleX, float scaleY) {

    }

    @Override
    public void drawImage(Image image, float scaleX, float scaleY, float posX, float posY) {

    }

    @Override
    public void drawImage(Image image, int sizeX, int sizeY, float posX, float posY) {

    }

    @Override
    public Font newFont(String filename, int size, boolean isBold) {
        return null;
    }

    @Override
    public void setFont(Font font) {

    }

    @Override
    public void drawText(String text, int x, int y) {

    }

    @Override
    public void setColor(int r, int g, int b, int a) {

    }

    @Override
    public void fillCircle(float cx, float cy, float radius) {

    }

    void render(Application app){
        while (!_holder.getSurface().isValid());
        Canvas canvas = _holder.lockCanvas();
        app.render(this);
        _holder.unlockCanvasAndPost(canvas);
    }
}
