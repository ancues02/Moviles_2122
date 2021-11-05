package es.ucm.gdv.engine;

import java.util.List;

public interface Application {
    //TODO: ¿Necesitamos init/destroy? / start?

    void handleInput(List<TouchEvent> e);
    void update(float deltaTime);
    void render(Graphics g);
}