package es.ucm.gdv.engine;


/**
 * Clase abstracta para representar escenas.
 * Tiene un atributo _engine para acceder a los
 * metodos y atributos del Engine.
 */
public abstract class AbstractScene implements Scene {
    Engine _engine;

    @Override
    public void setEngine(Engine engine){
        _engine = engine;
    }
}
