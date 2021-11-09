package es.ucm.gdv.engine;

import java.util.List;

public interface Scene {
    void start();
    void handleInput();
    void update(float deltaTime);
    void render();
    void setEngine(Engine engine);
}