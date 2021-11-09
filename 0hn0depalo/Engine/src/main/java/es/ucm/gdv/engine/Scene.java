package es.ucm.gdv.engine;

import java.util.List;

public interface Scene {
    //TODO: ¿Necesitamos init/destroy? / start?

    void handleInput();
    void start();
    void update(float deltaTime);
    void render();
    void setEngine(Engine engine);
}