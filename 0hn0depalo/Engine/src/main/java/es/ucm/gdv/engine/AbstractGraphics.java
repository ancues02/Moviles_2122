package es.ucm.gdv.engine;

import java.util.HashMap;

public abstract class AbstractGraphics implements Graphics{
    protected float _virtualX, _virtualY; // Dimensiones del canvas virtual
    protected float _aspectRatio;       // Relación de aspecto entre X e Y del canvas
    protected float _realX, _realY;   // Dimensiones del canvas ajustado a la ventana real
    protected boolean _verticalCompensation = true;   // Hacia dónde tiene que compensar poniendo barras
    protected float _scale = 1.0f;    // Multiplicador de tamaño del canvas virtual al real
    protected HashMap<String, Font> _fonts; //Fonts cargadas
    protected HashMap<String, Image> _images; //Imagenes cargadas

    protected AbstractGraphics(){
        _fonts = new HashMap<>();
        _images = new HashMap<>();
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
}
