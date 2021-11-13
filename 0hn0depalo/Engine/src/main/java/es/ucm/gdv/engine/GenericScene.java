package es.ucm.gdv.engine;


/**
 * Clase abstracta para representar escenas.
 * Tiene un atributo _engine para acceder a los
 * metodos y atributos del Engine desde la propia escena.
 */
public abstract class GenericScene implements Scene {
    protected Engine _engine;

    @Override
    public void setEngine(Engine engine){
        _engine = engine;
    }
}
