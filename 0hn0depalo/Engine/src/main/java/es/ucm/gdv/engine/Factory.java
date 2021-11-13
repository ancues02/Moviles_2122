package es.ucm.gdv.engine;

/**
 * Interfaz auxiliar que representa un
 * metodo factoria. La usamos para implementar
 * los pools.
 */
public interface Factory<T> {

    /**
     * Devuelve una instancia del tipo T especificado
     *
     * @return La instancia de un objeto de tipo T
     */
    T newInstance();
}