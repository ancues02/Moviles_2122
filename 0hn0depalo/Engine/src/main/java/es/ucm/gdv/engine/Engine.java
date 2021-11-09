package es.ucm.gdv.engine;

public interface Engine {
    Graphics getGraphics();
    Input getInput();
    void setScene(Scene scene);
}