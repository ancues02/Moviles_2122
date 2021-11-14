package es.ucm.gdv.ohno;

import java.util.List;

import es.ucm.gdv.engine.GenericScene;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Image;
import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

public class OhnO_Menu extends GenericScene {
    private Graphics _graphics;
    private Input _input;
    private float _fontSize;
    private Font _fontMolle, _fontJose;
    private Image _img;
    // Tiempo que tarda la escena en hacer fade
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
        //imagen final
        _img = _graphics.newImage("assets/images/q42.png");
        _graphics.setBackground(255, 255, 255, 255);
    }

    @Override
    public void handleInput() {
        List<TouchEvent> events = _input.getTouchEvents();
        while(events.size() > 0) {
            TouchEvent touchEvent = events.get(0);
            events.remove(0);
            processInput(touchEvent);
        }
    }

    private void processInput(TouchEvent e){
        if (e.getType() != TouchType.Press) return;

        float X = e.getX();
        float Y = e.getY();

        if(Y >= 0.44f && Y <= 0.58f ){
            if(X >= 0.25f && X <= 0.75f)
                _engine.setScene(new OhnO_SelectSize());
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
        //Nombre juego
        _graphics.setColor(0, 0, 0, 255);
        _fontMolle.setSize((_fontSize));
        _graphics.setFont(_fontMolle);

        String name = "Oh no";
        _graphics.drawText(name, 0.5f, 0.25f);

        //Jugar
        _fontJose.setSize(_fontSize);
        _graphics.setFont(_fontJose);
        name = "Jugar";
        _graphics.drawText(name, 0.5f, 0.5f);

        //copiado
        _graphics.setColor(180, 180, 180, 255);

        _fontJose.setSize(_fontSize/4);
        _graphics.setFont(_fontJose);
        name = "Un juego copiado a Q42";
        _graphics.drawText(name, 0.5f, 0.65f);
        name = "Creado por Martin Kool";
        _graphics.drawText(name, 0.5f, 0.70f);

        //imagen final
        _graphics.drawImage(_img, 90,128, 0.5f, 0.87f);

        //fade-in inicial de la escena, la tapamos entera
        // y le bajamos el alpha del circulo que la tapa
        if(_sceneAlpha>0) {
            _graphics.setColor(255, 255, 255, (int) _sceneAlpha);
            _graphics.fillCircle(0.5f, 0.5f, 1);
        }
    }
}
