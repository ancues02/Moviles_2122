package es.ucm.gdv.ohno;

import es.ucm.gdv.engine.AbstractScene;
import es.ucm.gdv.engine.Font;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Image;

public class OhnO_Menu extends AbstractScene {
    private Graphics _g;
    private float _fontSize;
    private Font _fontMolle, _fontJose;
    private float _widthImages, _heightImages;

    @Override
    public void start() {
        _fontSize= 120;
        _g = _engine.getGraphics();
        _fontMolle = _g.newFont("Molle-Regular.ttf", _fontSize, false);
        _fontJose = _g.newFont("JosefinSans-Bold.ttf", _fontSize, false);
    }

    @Override
    public void handleInput() {

    }

    @Override
    public void update(float deltaTime) {

    }

    @Override
    public void render() {

        //tamaño tablero
        _g.setColor(0, 0, 0, 255);
        _g.setFont(_fontMolle);


        String name = "Oh no";
        _g.drawText(name, 0.5f, 0.25f);

        _fontJose.setSize(_fontSize);
        _g.setFont(_fontJose);
        name = "Jugar";
        _g.drawText(name, 0.5f, 0.5f);


        _g.setColor(180, 180, 180, 255);

        _fontJose.setSize(_fontSize/4);
        _g.setFont(_fontJose);
        name = "Un juego copiado a Q42";
        _g.drawText(name, 0.5f, 0.75f);
        name = "Creado por Martin Kool";
        _g.drawText(name, 0.5f, 0.80f);

        Image im = _g.newImage("q42.png");
        _widthImages = im.getCanvasWidth();
        _heightImages = im.getCanvasHeight();
        _g.drawImage(im, 64,64, 0.5f, 0.90f);
    }
}
