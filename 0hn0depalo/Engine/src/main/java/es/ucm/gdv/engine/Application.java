package es.ucm.gdv.engine;

public interface Application {
    //TODO: ¿Necesitamos init/destroy? / start?

    public void handleInput(TouchEvents e);
    public void update(float deltaTime);
    public void render(Graphics g);
}