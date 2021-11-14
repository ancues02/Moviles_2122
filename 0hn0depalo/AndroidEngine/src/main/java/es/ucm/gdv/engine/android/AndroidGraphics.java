package es.ucm.gdv.engine.android;

import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Rect;
import android.view.SurfaceHolder;
import android.view.SurfaceView;

import androidx.appcompat.app.AppCompatActivity;

import es.ucm.gdv.engine.GenericGraphics;
import es.ucm.gdv.engine.Scene;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Image;


/**
 * Clase que implementa motor grafico en Android
 */
public class AndroidGraphics extends GenericGraphics {
    // Manager de los assets para crear las imagenes y fuentes
    private AssetManager _assetsManager;

    // Vista principal de la actividad
    private SurfaceView _view;
    private SurfaceHolder _holder;

    // Objeto para realizar las operaciones de
    private Paint _paint;

    // Canvas de android
    private Canvas _canvas;

    // Rect para colocar el texto correctamente
    Rect _textBounds;

    /**
     * Constructor
     * Crea los atributos y ajusta el canvas segun el canvas virtual indicado.
     * Establece la vista principal de la actividad.
     *
     * @param activity La actividad asociada
     * @param virtualHeight Anchura del canvas virtual deseado para la aplicacion.
     * @param virtualHeight Altura del canvas virtual deseado para la aplicacion.
     */
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

    @Override
    public float getWidth() {
        return _view.getWidth();
    }


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

    @Override
    public void drawImage(Image image) {
        if(image == null) return;

        AndroidImage ai = (AndroidImage)image;

        _canvas.drawBitmap(ai.getBitmap(), 0, 0, _paint);
    }

    @Override
    public void drawImage(Image image, float scaleX, float scaleY) {
        drawImage(image, scaleX, scaleY, 0, 0);
    }

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

    @Override
    public void setFont(Font font) {
        if(font == null) return;

        AndroidFont af = (AndroidFont)font;
        _paint.setTypeface(af.get_font());
        _paint.setFakeBoldText(af.getBold());
        _paint.setTextSize(af.getSize()*_scale);
    }

    @Override
    public void drawText(String text, float pX, float pY) {

        float rX = virtualToRealX(pX * _virtualX);     // Se ajusta a la escala puesta al canvas
        float rY = virtualToRealY(pY * _virtualY);

        _paint.getTextBounds(text, 0, text.length(), _textBounds);

        _canvas.drawText(text, rX - _textBounds.exactCenterX(), rY - _textBounds.exactCenterY(), _paint);
    }

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

    // Metodos de la clase

    /**
     * Ajusta el canvas virtual a la vista principal
     */
    public void adjustCanvasToView(){
        adjustCanvasToSize(_view.getWidth(), _view.getHeight());
    }

    /**
     * Funcion auxiliar para pasar el color a un int ARGB
     *
     * @param r, Componente rojo del color
     * @param g, Componente verde del color
     * @param b, Componente azul del color
     * @param a, Componente alpha del color
     */
    private int colorBitshift(int r, int g, int b, int a){
        return (a << 24) | (r << 16) | (g << 8) | b << 0;
    }

   /**
     * Gestiona los buffers de renderizado y el swap
     * para tener renderizado activo y renderizar el estado de la aplicacion.
     *
     * @param scene Escena que se renderiza
     */
    @Override
    public void render(Scene scene){
        while (!_holder.getSurface().isValid());
        _canvas = _holder.lockCanvas();
        clear();
        scene.render();
        fillOffsets();
        _holder.unlockCanvasAndPost(_canvas);
    }

    /**
     * Devuelve el SurfaceView que es la vista principal de la actividad
     * Lo usamos para que el engine establezca este view en la actividad
     *
     * @return La vista principal
     */
    SurfaceView getSurfaceView(){
        return _view;
    }

    @Override
    protected void fillOffsets(){
        _paint.setColor(colorBitshift(_bR, _bG, _bB, _bA));
        if(_verticalCompensation)
            fillVerticalOffsets();
        else
            fillHorizontalOffsets();
    }

    //Rellena de blanco por los lados
    @Override
    protected void fillHorizontalOffsets(){
        _canvas.drawRect(0,0,(int) virtualToRealX(0),
                _canvas.getHeight(),
                _paint);

        _canvas.drawRect((_canvas.getWidth() - _realX)/ 2 + (int)_realX,
                0,_canvas.getWidth(),
                _canvas.getHeight(),
                _paint);


    }

    @Override
    protected void fillVerticalOffsets(){
        _canvas.drawRect(0,0,_canvas.getWidth(),
                (int) virtualToRealY(0),
                _paint);

        _canvas.drawRect(0,
                (_canvas.getHeight() - _realY)/ 2 + (int)_realY,
                _canvas.getWidth(), _canvas.getHeight(),
                _paint);
    }
}
