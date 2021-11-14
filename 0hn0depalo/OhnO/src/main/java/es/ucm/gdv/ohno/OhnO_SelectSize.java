package es.ucm.gdv.ohno;

import java.util.List;

import es.ucm.gdv.engine.GenericScene;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Image;
import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

public class OhnO_SelectSize extends GenericScene {
    private Graphics _graphics;
    private Input _input;
    private float _fontSize;
    private Font _fontMolle, _fontJose;
    private Image _img;
    private float _widthImages, _heightImages;
    private float _aspectRatio;

    private int _numCircles;
    private float _yBoardOffset;
    private float _xBoardOffset;
    private float _boardCircleRad;
    private float _extraCircle;

    private float _sceneFadeTime = 0.5f;
    private float _sceneAlpha = 255.0f;
    private int _sceneFadeFactor = -1;    // 1 = fadeIn // -1 = fadeOut

    @Override
    public void start() {
        _fontSize= 120;
        _graphics = _engine.getGraphics();
        _input = _engine.getInput();
        _fontMolle = _graphics.newFont("assets/fonts/Molle-Regular.ttf", _fontSize, false);
        _fontJose = _graphics.newFont("assets/fonts/JosefinSans-Bold.ttf", _fontSize, false);
        _img = _graphics.newImage("assets/images/close.png");
        _graphics.setBackground(255, 255, 255, 255);
        _aspectRatio = _graphics.getAspectRatio();
    }

    @Override
    public void handleInput() {
        List<TouchEvent> events = _input.getTouchEvents();
        while(events.size() > 0) {
            TouchEvent touchEvent = events.get(0);
            processInput(touchEvent);
            _input.popEvent(touchEvent);
        }
    }

    @Override
    public void update(float deltaTime) {
        //fade-in al inicio
        if(_sceneAlpha>0) {
            _sceneAlpha += _sceneFadeFactor * deltaTime * (255.0f / _sceneFadeTime);
            _sceneAlpha = clamp(_sceneAlpha, 0.0f, 255.0f);
        }

    }
    /**
     * Funcion auxiliar para restringir un valor
     * dentro de unos limites
     *
     * @param value Valor que se quiere restringir
     * @param min Valor minimo al que se limita el valor
     * @param max Valor maximo al que se limita el valor
     * @return El valor dentro de los limites
     */
    private float clamp(float value, float min, float max){
        float ret = value;
        ret = Math.max(min, ret);
        ret = Math.min(ret, max);
        return ret;
    }

    @Override
    public void render() {

        _fontSize= 120;
        Font f = _fontMolle;
        f.setSize(_fontSize);
        //tamaño tablero
        _graphics.setColor(0, 0, 0, 255);
        _graphics.setFont(f);

        String name = "Oh no";
        _graphics.drawText(name, 0.5f, 0.25f);

        _fontSize /= 4;
        f = _fontJose;
        f.setSize(_fontSize);
        _graphics.setFont(f);

        String tam = "Elige el tamaño a jugar";
        _graphics.drawText(tam, 0.5f, 0.333f);

        //---------------------pintar los circulos----------------------------
        f.setSize(_fontSize*2);
        _graphics.setFont(f);

        _yBoardOffset = 0.4f;   //donde empieza a pintarse el tablero
        _xBoardOffset = (1 - 2 * _yBoardOffset);

        _numCircles = 3;
        _extraCircle = 1f;   // Círculo fantasma extra para el offset
        _boardCircleRad = (1 - _xBoardOffset * 2) / (_numCircles + _extraCircle);
        float yPos = _yBoardOffset + ((_boardCircleRad * _aspectRatio) / 2);
        float xPos = _xBoardOffset + (_boardCircleRad / 2);
        int cont = 4;
        for (int i = 0; i < 2; ++i) {
            for (int j = 0; j < 3; ++j) {
                if((j + i) % 2 == 1)
                    _graphics.setColor(255,78,72, 255);//rojo
                else
                    _graphics.setColor(10,180,235, 255);//azul

                _graphics.fillCircle(xPos, yPos, _boardCircleRad / 2f);
                _graphics.setColor(255,255,255,255);


                String num = ""+(cont++);
                _graphics.drawText(num, xPos, yPos);
                xPos += _boardCircleRad + (_extraCircle / (_numCircles - 1)) * _boardCircleRad;
            }
            xPos = _xBoardOffset + (_boardCircleRad / 2);
            yPos += (_boardCircleRad * _aspectRatio) + ((_extraCircle / (_numCircles - 1)) * _boardCircleRad * _aspectRatio);
        }//circulos pintados

        //
        float yOffset = 5f * 0.1666f;
        float xOffset = 0.5f;

        _widthImages = _img.getCanvasWidth();
        _heightImages = _img.getCanvasHeight();
        _graphics.drawImage(_img, 1.0f,1.0f, xOffset, yOffset);

        _yBoardOffset = 0.4f;   //donde empieza a pintarse el tablero
        _xBoardOffset = (1 - 2 * _yBoardOffset);

        _numCircles = 3;
        _extraCircle = 1f;   // Círculo fantasma extra para el offset
        _boardCircleRad = (1 - _xBoardOffset * 2) / (_numCircles + _extraCircle);

        //fade-in inicial de la escena, la tapamos entera
        // y le bajamos el alpha del circulo que la tapa
        if(_sceneAlpha>0) {
            _graphics.setColor(255, 255, 255, (int) _sceneAlpha);
            _graphics.fillCircle(0.5f, 0.5f, 1);
        }
    }

    private void processInput(TouchEvent e){
        if(e.getType() != TouchType.Press) return;

        float X = e.getX();
        float Y = e.getY();

        float col2=_xBoardOffset + _boardCircleRad + (_extraCircle / (_numCircles - 1)) * _boardCircleRad;
        float col3=col2 +_boardCircleRad + (_extraCircle / (_numCircles - 1)) * _boardCircleRad;
        if (Y >= _yBoardOffset && Y <= _yBoardOffset + (_boardCircleRad * _aspectRatio)) {
            if (X >= _xBoardOffset && X <= _xBoardOffset + _boardCircleRad ) {

                _engine.setScene(new OhnO_Game(4));
            } else if (X >= col2 && X <= col2+_boardCircleRad) {
                _engine.setScene(new OhnO_Game(5));
            } else if (X >= col3 && X <= col3 + _boardCircleRad) {
                _engine.setScene(new OhnO_Game(6));
            }
            //_currState = GameState.GAME;
        } else if (Y >= _yBoardOffset + (_boardCircleRad * _aspectRatio) + ((_extraCircle / (_numCircles - 1)) * _boardCircleRad * _aspectRatio)
                && Y <= _yBoardOffset + (2*_boardCircleRad * _aspectRatio) + ((_extraCircle / (_numCircles - 1)) * _boardCircleRad * _aspectRatio)) {
            if (X >= _xBoardOffset && X <= _xBoardOffset + _boardCircleRad ) {
                _engine.setScene(new OhnO_Game(7));
            } else if (X >= col2 && X <= col2+_boardCircleRad) {
                _engine.setScene(new OhnO_Game(8));

            } else if (X >= col3 && X <= col3 + _boardCircleRad) {
                _engine.setScene(new OhnO_Game(9));

            }
        }//fin detectar input en circulos

        //detectar input en la cruz
        float yOffset = 5f * 0.1666f;
        float xOffset = 0.5f;
        if(Y >= yOffset - _heightImages/2 && Y <= yOffset + _heightImages/2) {
            if (X >=  xOffset - _widthImages / 2 && X <=  xOffset + _widthImages / 2) {
                _engine.setScene(new OhnO_Menu());

            }
        }
    }
}
