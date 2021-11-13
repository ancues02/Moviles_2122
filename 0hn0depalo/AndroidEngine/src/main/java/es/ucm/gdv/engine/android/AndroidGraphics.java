package es.ucm.gdv.engine.android;

import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Rect;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import androidx.appcompat.app.AppCompatActivity;

import es.ucm.gdv.engine.AbstractGraphics;
import es.ucm.gdv.engine.Scene;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Image;

public class AndroidGraphics extends AbstractGraphics {
    private AssetManager _assetsManager;

    private SurfaceView _view;
    private SurfaceHolder _holder;  // TODO: esta referencia es innecesaria creo
    private Paint _paint;
    private Canvas _canvas;
    Rect _textBounds;   // Para colocar el texto correctamente

    public AndroidGraphics(AppCompatActivity activity, int virtualWidth, int virtualHeight){
        super();
        _view = new SurfaceView(activity);
        _holder = _view.getHolder();
        _paint = new Paint();

        setCanvasDimensions(virtualWidth, virtualHeight);

        _assetsManager = activity.getAssets();
        activity.setContentView(_view);

        _textBounds = new Rect();
    }

    public void adjustCanvasToView(){
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
        return (a << 24) | (r << 16) | (g << 8) | b << 0;
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

    @Override
    public float getCanvasWidth() {
        return _realX;
    }

    @Override
    public float getCanvasHeight() {
        return _realY;
    }

    /**
     * Borra lo que haya en pantalla con el color de fondo indicado
     */
    @Override
    public void clear(){
        _canvas.drawColor(colorBitshift(_bR, _bG, _bB, _bA));
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
     * Devuelve una imagen. Solo la creamos una vez y la guardamos en un mapa
     *
     * @param filename Nombre del archivo de la imagen
     *
     * @return  Image Con la imagen indicada cargada
     *          null Si la imagen no se pudo cargar
     */
    @Override
    public Image newImage(String filename) {
        AndroidImage ai = null;
        if(!_images.containsKey(filename)) {
            ai = new AndroidImage();
            if (!ai.load(filename, _assetsManager)) {
                return null;
            }
            _images.put(filename,ai);
        }
        else
            ai=(AndroidImage) _images.get(filename);

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

        _canvas.drawBitmap(ai.getBitmap(), 0, 0, _paint);
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
        Bitmap b = ai.getBitmap();

        float rX = virtualToRealX(posX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(posY * _virtualY);

        float sX = (ai.getWidth() * scaleX * _scale);
        float sY = (ai.getHeight() * scaleY * _scale);
        int aux = (int)(rX  - (sX/2)), aux2=(int)(rY - (sY/2));
        ai.setCanvasWidthHeight(sX/getCanvasWidth()  ,sY/getCanvasHeight() );

        _canvas.drawBitmap(b,
                null,
                new Rect(aux,aux2, aux + (int)sX, aux2 + (int)sY),
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
        Bitmap b = ai.getBitmap();

        float rX = virtualToRealX(posX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(posY * _virtualY);

        float sX = (sizeX* _scale);
        float sY = (sizeY * _scale);
        int aux = (int)(rX  - (sX/2)), aux2=(int)(rY - (sY/2));
        ai.setCanvasWidthHeight(sX/getCanvasWidth()  ,sY/getCanvasHeight() );
        _canvas.drawBitmap(b,
                null,
                new Rect(aux,aux2, aux + (int)sX, aux2 + (int)sY),
                _paint);
    }

    /**
     * Devuelve una fuente. Solo la creamos una vez y la guardamos en un mapa
     *
     * @param filename Nombre del archivo de la fuente
     *
     * @return  Font Con la fuente indicada cargada
     *          null Si la fuente no se pudo cargar
     */
    @Override
    public Font newFont(String filename, float size, boolean isBold) {
        AndroidFont af = null;
        if(!_fonts.containsKey(filename)) {
            af = new AndroidFont();

            if (!af.load(filename, _assetsManager)) {
                return null;
            }
            _fonts.put(filename, af);
        }
        else
            af=(AndroidFont)_fonts.get(filename);


        af.setBold(isBold);
        af.setSize(size );
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
        _paint.setFakeBoldText(af.getBold());
        _paint.setTextSize(af.getSize()*_scale);
    }

    /**
     * Escribe el texto en pantalla usando la fuente establecida
     *
     * @param text El texto que se escribe en pantalla
     * @param pX Coordenada x de la posicion en la que se quiere empezar a escribir (left)
     * @param pY Coordenada y de la posicion en la que se quiere empezar a escribir (top)
     */
    @Override
    public void drawText(String text, float pX, float pY) {

        float rX = virtualToRealX(pX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(pY * _virtualY);


        _paint.getTextBounds(text, 0, text.length(), _textBounds);

        //rY +=_paint.getTextSize() / 6.f * _scale;
        _canvas.drawText(text, rX - _textBounds.exactCenterX(), rY - _textBounds.exactCenterY(), _paint);
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

    @Override
    public void drawCircle(float percentX, float percentY, float radius) {
        float rX = virtualToRealX(percentX * _virtualX);// Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);
        float radiusReal;
        if(_aspectRatio < 1)
            radiusReal = radius  * _virtualX;
        else
            radiusReal = radius  * _virtualY;

        Paint.Style prevStyle = _paint.getStyle();
        _paint.setStyle(Paint.Style.STROKE);
        
        _canvas.drawCircle((int)(rX ),
                (int)(rY ),
                (int)(radiusReal *  _scale), _paint);

        _paint.setStyle(prevStyle);
    }

    /**
     * Dibuja un circulo en pantalla
     *
     * @param percentX Coordenada x del centro del circulo
     * @param percentY Coordenada y del centro del circulo
     * @param radius Radio del circulo
     */
    @Override
    public void fillCircle(float percentX, float percentY, float radius) {
        float rX = virtualToRealX(percentX * _virtualX);// Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(percentY * _virtualY);
        float radiusReal;
        if(_aspectRatio < 1)
            radiusReal = radius  * _virtualX;
        else
            radiusReal = radius  * _virtualY;
        _canvas.drawCircle((int)(rX ),
                (int)(rY ),
                (int)(radiusReal *  _scale), _paint);

    }

   /**
     * Gestiona los buffers de renderizado y el swap
     * para tener renderizado activo y renderizar el estado de la aplicacion.
     *
     * @param app Aplicacion que se renderiza
     */
    @Override
    public void render(Scene app){
        while (!_holder.getSurface().isValid());
        _canvas = _holder.lockCanvas();
        clear();
        app.render();
        fillOffsets();
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

    private void fillOffsets(){
        int c = colorBitshift(_bR, _bG, _bB, _bA);
        if(_verticalCompensation)
            fillVerticalOffsets(c);
        else
            fillHorizontalOffsets(c);
    }

    //Rellena de blanco por los lados
    private void fillHorizontalOffsets(int c){
        _paint.setColor(c);
        _canvas.drawRect(0,0,(int) virtualToRealX(0),
                _canvas.getHeight(),
                _paint);

        _canvas.drawRect((_canvas.getWidth() - _realX)/ 2 + (int)_realX,
                0,_canvas.getWidth(),
                _canvas.getHeight(),
                _paint);


    }

    private void fillVerticalOffsets(int c){

        _paint.setColor(c);
        _canvas.drawRect(0,0,_canvas.getWidth(),
                (int) virtualToRealY(0),
                _paint);

        _canvas.drawRect(0,
                (_canvas.getHeight() - _realY)/ 2 + (int)_realY,
                _canvas.getWidth(), _canvas.getHeight(),
                _paint);
    }
}
