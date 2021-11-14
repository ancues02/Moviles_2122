package es.ucm.gdv.engine;

import java.util.HashMap;

/**
 * Clase abstracta que implementa Graphics.
 * Tiene la funcionalidad comun que implementan los
 * motores graficos de Pc y Android:
 *  ·   El calculo a mano de las dimensiones del canvas virtual en pantalla.
 *  ·   Los atributos y metodos para el calculo del canvas virtual y el escalado.
 *  ·   HashMaps para mantener un registro de las imagenes y fuentes que estan cargadas.
 *  ·   Color con el que se hara el clear y las barras para el canvas virtual
 *  ·   El metodo render que deberan implementar
 *      las clases que lo extiendan, para renderizar
 *      la escena proporcionada por el motor.
 *
 *  Para ajustar el canvas virtual, pintamos barras verticales
 *  u horizontales despues de hacer le renderizado.
 *  Estas barras se pintaran del mismo color que se usa en el clear
 *  y se puede configurar en el metodo del interfaz Graphics "setBackground"
 *
 *  Como hacemos los calculos para el canvas virtual a mano, no implementamos
 *  los metodos translate, scale, save y restore porque no son necesarios.
 *
 *  Los parámetros de coordenadas en los métodos de dibujo son recibidos en forma de
 *  procentaje, y luego ajustados a píxeles según el canvas virtual, su relación de aspecto
 *  y su escala en la ventana actual.
 */
public abstract class GenericGraphics implements Graphics{
    // Atributos

    // Dimensiones del canvas virtual
    protected float _virtualX, _virtualY;

    // Relación de aspecto entre X e Y del canvas
    protected float _aspectRatio;

    // Dimensiones del canvas ajustado a la ventana real
    protected float _realX, _realY;

    // Hacia dónde tiene que compensar poniendo barras
    protected boolean _verticalCompensation;

    // Multiplicador de tamaño del canvas virtual al real
    protected float _scale;

    //Fonts cargadas
    protected HashMap<String, Font> _fonts;

    //Imagenes cargadas
    protected HashMap<String, Image> _images;

    // Color para clear y las barras
    protected int _bR, _bG, _bB, _bA;

    /**
     * Constructor
     *
     * Crea los mapas para imagenes y fonts cargadas
     * e inicializa el resto de atributos
     */
    protected GenericGraphics(){
        _virtualX = _virtualY = 1.0f;
        _aspectRatio = 1.0f;
        _scale = 1.0f;
        _realX = _realY = 1.0f;
        _verticalCompensation = true;
        _bR = 255;
        _bG = 255;
        _bB = 255;
        _bA = 255;

        _fonts = new HashMap<>();
        _images = new HashMap<>();
    }

    public float getAspectRatio(){
        return _aspectRatio;
    }

    /**
     * Establece las dimensiones del canvas virtual
     * y calcula el aspectRatio segun estas dimensiones
     *
     * @param vX Ancho del canvas virtual
     * @param vY Alto del canvas virtual
     */
    protected void setCanvasDimensions(float vX, float vY){
        _virtualX = vX;
        _virtualY = vY;
        _aspectRatio = _virtualX/_virtualY;
    }

    /**
     * Adapta las medidas reales del canvas virtual.
     *
     * @param x Ancho de la pantalla al que ajustarse
     * @param y Alto de la pantalla al que ajustarse
     */
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

    /**
     * Devuelve la coordenada de x real
     * segun el canvas virtual
     *
     * @param targetX Coordenada de x virtual
     * @return La coordenada x real
     */
    protected float virtualToRealX(float targetX){
        float rX = targetX * _scale;
        float windowW = getWidth();
        float canvasW = getCanvasWidth();
        if(!_verticalCompensation) rX += (windowW - _realX) / 2;
        return rX;
    }

    /**
     * Devuelve la coordenada de y real
     * segun el canvas virtual
     *
     * @param targetY Coordenada de x virtual
     * @return La coordenada y real
     */
    protected float virtualToRealY(float targetY){
        float rY = targetY * _scale;
        float windowH = getHeight();
        float canvasH = getCanvasHeight();
        if(_verticalCompensation) rY += (windowH - _realY) / 2;
        return rY;
    }

    /**
     * Devuelve la coordenada x virtual
     * transformando la coordenada real
     *
     * @param targetX Coordenada de x real
     * @return La coordenada x virtual
     */
    public float realToVirtualX(float targetX){
        float rX = targetX;
        float windowW = getWidth();
        if(!_verticalCompensation) rX -= (windowW - _realX) / 2;
        rX /= _scale;
        return rX / _virtualX;
    }

    /**
     * Devuelve la coordenada y virtual
     * transformando la coordenada real
     *
     * @param targetY Coordenada de y real
     * @return La coordenada y virtual
     */
    public float realToVirtualY(float targetY){
        float rY = targetY;
        float windowH = getHeight();
        if(_verticalCompensation) rY -= (windowH - _realY) / 2;
        rY /= _scale;
        return rY / _virtualY;
    }

    /**
     * Establece la el color con el que se hara clear
     * se rellenaran las barras para compensar el canvas
     *
     * @param r, Componente rojo del color
     * @param g, Componente verde del color
     * @param b, Componente azul del color
     * @param a, Componente alpha del color
     */
    public void setBackground(int r, int g, int b, int a){
        _bR = r;
        _bG = g;
        _bB = b;
        _bA = a;
    }

    /**
     * Renderiza una escena.
     * Es un metodo abstracto porque dependera de la plataforma,
     * ya que se encarga de hacer el renderizado activo.
     *
     * @param scene La escena que se renderiza
     */
    abstract public void render(Scene scene);


    //TODO: Poner estos metodos aqui y quitarlos de cada implementacion
    /**
     * Rellena los margenes del color con el que
     * se hace el clear para ajustar el canvas.
     * Considera si hay que poner barras en los
     * bordes horizontales o verticales.
     */
    abstract protected void fillOffsets();

    /**
     * Rellena los margenes horizontales
     */
    abstract protected void fillHorizontalOffsets();

    /**
     * Rellena los margenes verticales
     */
    abstract protected void fillVerticalOffsets();
}
