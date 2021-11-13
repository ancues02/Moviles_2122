package es.ucm.gdv.engine;

import java.util.HashMap;

/**
 * Clase abstracta que implementa Graphics.
 * Tiene la funcionalidad comun que implementan los
 * motores graficos de Pc y Android.
 * Los metodos para el calculo del canvas virtual y el escalado.
 * Ademas tiene un metodo render que deberan implementar
 * las clases que lo extiendan, para renderizar la escena proporcionada
 * por el motor.
 */
public abstract class AbstractGraphics implements Graphics{
    protected float _virtualX, _virtualY; // Dimensiones del canvas virtual
    protected float _aspectRatio;       // Relación de aspecto entre X e Y del canvas
    protected float _realX, _realY;   // Dimensiones del canvas ajustado a la ventana real
    protected boolean _verticalCompensation = true;   // Hacia dónde tiene que compensar poniendo barras
    protected float _scale = 1.0f;    // Multiplicador de tamaño del canvas virtual al real
    protected HashMap<String, Font> _fonts; //Fonts cargadas
    protected HashMap<String, Image> _images; //Imagenes cargadas
    protected int _bR, _bG, _bB, _bA;

    protected AbstractGraphics(){
        _fonts = new HashMap<>();
        _images = new HashMap<>();
        _bR = 255;
        _bG = 255;
        _bB = 255;
        _bA = 255;
    }

    protected void setCanvasDimensions(float vX, float vY){
        _virtualX = vX;
        _virtualY = vY;
        _aspectRatio = _virtualX/_virtualY;
    }

    protected void adjustCanvasToSize(int x, int y){
        if((float)x/(float)y <= _aspectRatio) {  // Hay que compensar por arriba
            _realX = x;
            _scale = _realX/_virtualX;
            _realY = _virtualY * _scale;
            _verticalCompensation = true;
        }else {  // Hay que compensar por los lados
            _realY = y;
            _scale = _realY/_virtualY;
            _realX = _virtualX * _scale;
            _verticalCompensation = false;
        }
    }

    protected float virtualToRealX(float targetX){
        float rX = targetX * _scale;
        float windowW = getWidth();
        float canvasW = getCanvasWidth();
        if(!_verticalCompensation) rX += (windowW - _realX) / 2;
        return rX;
    }

    protected float virtualToRealY(float targetY){
        float rY = targetY * _scale;
        float windowH = getHeight();
        float canvasH = getCanvasHeight();
        if(_verticalCompensation) rY += (windowH - _realY) / 2;
        return rY;
    }

    public float realToVirtualX(float targetX){
        float rX = targetX;
        float windowW = getWidth();
        if(!_verticalCompensation) rX -= (windowW - _realX) / 2;
        rX /= _scale;
        return rX / _virtualX;
    }

    public float realToVirtualY(float targetY){
        float rY = targetY;
        float windowH = getHeight();
        if(_verticalCompensation) rY -= (windowH - _realY) / 2;
        rY /= _scale;
        return rY / _virtualY;
    }

    abstract public void render(Scene app);

    public void setBackground(int r, int g, int b, int a){
        _bR = r;
        _bG = g;
        _bB = b;
        _bA = a;
    }
}
