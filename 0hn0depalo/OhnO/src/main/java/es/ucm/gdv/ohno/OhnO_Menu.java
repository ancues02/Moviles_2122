package es.ucm.gdv.ohno;

import java.util.List;

import es.ucm.gdv.engine.AbstractScene;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Image;
import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

public class OhnO_Menu extends AbstractScene {
    private Graphics _g;
    private Input _input;
    private float _fontSize;
    private Font _fontMolle, _fontJose;
    // Tiempo que tarda la escena en hacer fade
    private float _sceneFadeTime = 0.5f;
    private float _sceneAlpha = 0.0f;
    private int _sceneFadeFactor = 1;    // 1 = fadeIn // -1 = fadeOut


    @Override
    public void start() {
        _fontSize= 120;
        _g = _engine.getGraphics();
        _input = _engine.getInput();
        _fontMolle = _g.newFont("Molle-Regular.ttf", _fontSize, false);
        _fontJose = _g.newFont("JosefinSans-Bold.ttf", _fontSize, false);
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
        _sceneAlpha += _sceneFadeFactor * deltaTime * (255.0f / _sceneFadeTime);
        _sceneAlpha = clamp(_sceneAlpha, 0.0f, 255.0f);

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
        _g.setColor(0, 0, 0, (int)_sceneAlpha);
        _fontMolle.setSize((_fontSize));
        _g.setFont(_fontMolle);

        String name = "Oh no";
        _g.drawText(name, 0.5f, 0.25f);

        //Jugar
        _fontJose.setSize(_fontSize);
        _g.setFont(_fontJose);
        name = "Jugar";
        _g.drawText(name, 0.5f, 0.5f);

        //copiado
        _g.setColor(180, 180, 180, (int)_sceneAlpha);

        _fontJose.setSize(_fontSize/4);
        _g.setFont(_fontJose);
        name = "Un juego copiado a Q42";
        _g.drawText(name, 0.5f, 0.65f);
        name = "Creado por Martin Kool";
        _g.drawText(name, 0.5f, 0.70f);

        //imagen final
        Image im = _g.newImage("q42.png");

        _g.drawImage(im, 90,128, 0.5f, 0.87f);
    }
}
