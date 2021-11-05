package es.ucm.gdv.engine.android;

import android.content.Context;
import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Rect;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import es.ucm.gdv.engine.AbstractGraphics;
import es.ucm.gdv.engine.Application;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Image;

public class AndroidGraphics extends AbstractGraphics {
    private AssetManager _assetsManager;

    private SurfaceView _view;
    private SurfaceHolder _holder;  // TODO: esta referencia es innecesaria creo
    private Paint _paint;
    private Canvas _canvas;

    public AndroidGraphics(Context context){
        super();
        _view = new SurfaceView(context);
        int w = _view.getWidth(), h = _view.getHeight();
        _holder = _view.getHolder();
        _paint = new Paint();

        _assetsManager = context.getAssets();
    }

    public void adjustCanvasToView(){
        setCanvasDimensions(_view.getWidth(), _view.getHeight());
        adjustCanvasToSize(_view.getWidth(), _view.getHeight());
    }

    /**
     * Funcion auxiliar para pasar el color a un int ARGB
     * @param r, Componente rojo del color
     * @param g, Componente verde del color
     * @param b, Componente azul del color
     * @param a, Componente alpha del color
     */
    private int colorBitshift(int r, int g, int b, int a){
        return (a >> 24) | (r >> 16) | (g >> 8) | b;
    }

    /**
     * Devuelve el ancho de la pantalla
     */
    @Override
    public float getWidth() {
        return _view.getWidth();
    }

    /**
     * Devuelve el alto de la pantalla
     */
    @Override
    public float getHeight() {
        return _view.getHeight();
    }

    /**
     * Borra lo que haya en pantalla con el color indicado
     *
     * @param r Componente roja del color
     * @param g Componente verde del color
     * @param b Componente azul del color
     * @param a Componente alpha del color
     */
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

    /**
     * Devuelve una imagen
     *
     * @param filename Nombre del archivo de la imagen
     *
     * @return  Image Con la imagen indicada cargada
     *          null Si la imagen no se pudo cargar
     */
    @Override
    public Image newImage(String filename) {
        AndroidImage ai = new AndroidImage();
        if(!ai.load(filename, _assetsManager)){
            return null;
        }

        return ai;
    }

    /**
     * Dibuja una imagen en la posicion por defecto(0,0)
     *
     * @param image La imagen que se quiere dibujar
     */
    @Override
    public void drawImage(Image image) {
        if(image == null) return;

        AndroidImage ai = (AndroidImage)image;

        _canvas.drawBitmap(ai.get_bitmap(), 0, 0, _paint);
    }

    /**
     * Dibuja una imagen escalada en la posicion por defecto(0,0)
     *
     * @param image La imagen que se quiere dibujar
     * @param scaleX La escala en X de la imagen
     * @param scaleY la escala en Y de la imagen
     */
    @Override
    public void drawImage(Image image, float scaleX, float scaleY) {
        drawImage(image, scaleX, scaleY, 0, 0);
    }

    /**
     * Dibuja una imagen escalada en la posicion dada
     *
     * @param image La imagen que se quiere dibujar
     * @param scaleX La escala en X de la imagen
     * @param scaleY la escala en Y de la imagen
     * @param posX Coordenada x de la posicion en la que se quiere dibujar la imagen (left)
     * @param posY Coordenada y de la posicion en la que se quiere dibujar la imagen (top)
     */
    @Override
    public void drawImage(Image image, float scaleX, float scaleY, float posX, float posY) {
        if(image == null) return;

        AndroidImage ai = (AndroidImage)image;
        Bitmap b = ai.get_bitmap();

        _canvas.drawBitmap(b,
                new Rect(0,0, b.getWidth(), b.getHeight()),
                new Rect((int)posX, (int)posY, (int)(b.getWidth() * scaleX), (int)(b.getWidth() * scaleY)),
                _paint);
    }

    /**
     * Dibuja una imagen del tamaño indicado en la posicion dada
     *
     * @param image La imagen que se quiere dibujar
     * @param sizeX El ancho de la imagen en pantalla
     * @param sizeY El alto de la imagen en pantalla
     * @param posX Coordenada x de la posicion en la que se quiere dibujar la imagen (left)
     * @param posY Coordenada y de la posicion en la que se quiere dibujar la imagen (top)
     */
    @Override
    public void drawImage(Image image, int sizeX, int sizeY, float posX, float posY) {
        if(image == null) return;

        AndroidImage ai = (AndroidImage)image;
        Bitmap b = ai.get_bitmap();
        _canvas.drawBitmap(b,
                new Rect(0,0,b.getWidth(), b.getHeight()),
                new Rect((int)posX, (int)posY, sizeX, sizeY),
                _paint);
    }

    /**
     * Devuelve una fuente
     *
     * @param filename Nombre del archivo de la fuente
     *
     * @return  Font Con la fuente indicada cargada
     *          null Si la fuente no se pudo cargar
     */
    @Override
    public Font newFont(String filename, int size, boolean isBold) {
        AndroidFont af = new AndroidFont();
        if(!af.load(filename, _assetsManager)){
            return null;
        }

        af.setBold(isBold);
        af.setSize(size);
        return af;
    }

    /**
     * Establece la fuente con la que se escribiran los textos
     *
     * @param font La nueva fuente que se utilizara
     */
    @Override
    public void setFont(Font font) {
        if(font == null) return;

        AndroidFont af = (AndroidFont)font;
        _paint.setTypeface(af.get_font());
        _paint.setFakeBoldText(af.get_bold());
        _paint.setTextSize(af.get_size());
    }

    /**
     * Escribe el texto en pantalla usando la fuente establecida
     *
     * @param text El texto que se escribe en pantalla
     * @param x Coordenada x de la posicion en la que se quiere empezar a escribir (left)
     * @param y Coordenada y de la posicion en la que se quiere empezar a escribir (top)
     */
    @Override
    public void drawText(String text, int x, int y) {
        int rX = (int)compensateX(x, _canvas.getWidth());     // Se ajusta a la escala puesta al canvas
        int rY = (int)compensateY(y, _canvas.getHeight());

        _canvas.drawText(text, rX, rY, _paint);
    }

    /**
     * Establece la el color con el que se realizaran las operaciones de pintado
     *
     * @param r, Componente rojo del color
     * @param g, Componente verde del color
     * @param b, Componente azul del color
     * @param a, Componente alpha del color
     */
    @Override
    public void setColor(int r, int g, int b, int a) {
        _paint.setColor(colorBitshift(r, g, b, a));
    }

    /**
     * Dibuja un circulo en pantalla
     *
     * @param cx Coordenada x del centro del circulo
     * @param cy Coordenada y del centro del circulo
     * @param radius Radio del circulo
     */
    @Override
    public void fillCircle(float cx, float cy, float radius) {
        _canvas.drawCircle(cx, cy, radius, _paint);
    }

    /**
     * Gestiona los buffers de renderizado y el swap
     * para tener renderizado activo y renderizar el estado de la aplicacion.
     *
     * @param app Aplicacion que se renderiza
     */
    void render(Application app){
        while (!_holder.getSurface().isValid());
        _canvas = _holder.lockCanvas();
        clear(255,255,255,255);
        //app.render(this);
        _holder.unlockCanvasAndPost(_canvas);
    }

    /**
     * Devuelve el SurfaceView que es la vista principal de la actividad
     * Lo usamos para que el engine establezca este view en la actividad
     *
     * @return _view La vista principal
     */
    SurfaceView getSurfaceView(){
        return _view;
    }
}
