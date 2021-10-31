package es.ucm.gdv.engine;

public abstract class AbstractGraphics implements Graphics{
    protected float _virtualX, _virtualY; // Dimensiones del canvas virtual
    protected float _aspectRatio; // Relación de aspecto entre X e Y del canvas
    protected float _realX, _realY;   // Dimensiones del canvas ajustado a la ventana real
    protected boolean _verticalCompensation = true;   // Hacia dónde tiene que compensar poniendo barras
    protected float _scale = 1.0f;    // Multiplicador de tamaño del canvas virtual al real

    protected AbstractGraphics(float vX, float vY){
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

    protected float compensateX(float targetX, float windowW){
        float rX = targetX * _scale;
        if(!_verticalCompensation) rX += (windowW - _realX) / 2;
        return rX;
    }

    protected float compensateY(float targetY, float windowH){
        float rY = targetY * _scale;
        if(_verticalCompensation) rY += (windowH - _realY) / 2;
        return rY;
    }
}
