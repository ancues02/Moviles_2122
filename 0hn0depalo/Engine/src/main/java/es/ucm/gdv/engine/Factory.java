package es.ucm.gdv.engine;


public interface Factory<T> {
    T newInstance();
}