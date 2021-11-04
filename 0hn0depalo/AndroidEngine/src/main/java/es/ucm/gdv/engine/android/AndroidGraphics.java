package es.ucm.gdv.engine.android;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import es.ucm.gdv.engine.Application;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Image;

public class AndroidGraphics implements Graphics {

    private SurfaceView _view;
    private SurfaceHolder _holder;  // TODO: esta referencia es innecesaria creo
    private Paint _paint;
    private Canvas _canvas;

    public AndroidGraphics(Context context){
        _view = new SurfaceView(context);
        _holder = _view.getHolder();
        _paint = new Paint();
    }

    /**
     * Funcion auxiliar para pasar el color a un int ARGB
     * @param r, componente rojo del color
     * @param g, componente verde del color
     * @param b, componente azul del color
     * @param a, componente alpha del color
     */
    private int colorBitshift(int r, int g, int b, int a){
        return (a >> 24) | (r >> 16) | (g >> 8) | b;
    }

    @Override
    public float getWidth() {
        return _view.getWidth();
    }

    @Override
    public float getHeight() {
        return _view.getHeight();
    }

    @Override
    public void clear(int r, int g, int b, int a){
        _canvas.drawColor(colorBitshift(r, g, b, a));
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
        //_paint.setTypeface();
    }

    @Override
    public void drawText(String text, int x, int y) {
        _canvas.drawText(text, x, y, _paint);
    }

    @Override
    public void setColor(int r, int g, int b, int a) {
        _paint.setColor(colorBitshift(r, g, b, a));
    }

    @Override
    public void fillCircle(float cx, float cy, float radius) {
        _canvas.drawCircle(cx, cy, radius, _paint);
    }

    void render(Application app){
        while (!_holder.getSurface().isValid());
        _canvas = _holder.lockCanvas();
        app.render(this);
        _holder.unlockCanvasAndPost(_canvas);
    }

    SurfaceView getSurfaceView(){
        return _view;
    }
}
