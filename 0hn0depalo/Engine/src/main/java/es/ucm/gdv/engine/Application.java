package es.ucm.gdv.engine;

public interface Application {
    //TODO: ¿Necesitamos init/destroy? / start?

    void handleInput(TouchEvent e);
    void update(float deltaTime);
    void render(Graphics g);
}