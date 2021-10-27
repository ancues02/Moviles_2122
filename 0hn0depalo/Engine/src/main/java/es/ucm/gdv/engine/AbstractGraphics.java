package es.ucm.gdv.engine;

public abstract class AbstractGraphics implements Graphics{
    protected float _virtualX = 1080, _virtualY = 1920; // Dimensiones del canvas virtual
    protected float _aspectRatio = _virtualX/_virtualY; // Relación de aspecto entre X e Y del canvas
    protected float _realX = 1080, _realY = 1920;   // Dimensiones del canvas ajustado a la ventana real
    protected boolean _verticalCompensation = true;   // Hacia dónde tiene que compensar poniendo barras
    protected float _scale = 1.0f;    // Multiplicador de tamaño del canvas virtual al real

    protected void adjustCanvasToSize(int x, int y){
        if(x/y <= _aspectRatio) {  // Hay que compensar por arriba
            _realX = x;
            _scale = _realX/_virtualX;
            _realY = y * _scale;
            _verticalCompensation = true;
        }else {  // Hay que compensar por abajo
            _realY = y;
            _scale = _realY/_virtualY;
            _realX = x * _scale;
            _verticalCompensation = false;
        }

        rescale();
    }

    protected abstract void rescale();
}
