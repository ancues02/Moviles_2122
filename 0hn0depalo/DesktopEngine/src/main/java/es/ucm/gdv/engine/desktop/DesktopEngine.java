package es.ucm.gdv.engine.desktop;

import es.ucm.gdv.engine.Engine;
import es.ucm.gdv.engine.Application;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Input;

public class DesktopEngine implements Engine {
    private DesktopGraphics _graphics;
    private DesktopInput _input;
    private Application _app;
    private long _lastFrameTime;

    public DesktopEngine(){
        _graphics = new DesktopGraphics(1920, 1080, 600, 900);
        _input = new DesktopInput(_graphics);
    }

    public void setApplication(Application app){
        _app = app;
    }
    public Graphics getGraphics(){
        return _graphics;
    };

    public Input getInput(){
        return _input;
    };

    public void setSize(int width, int height) {
        _graphics.setSize(width, height);
    }
        //bucle ppal del motor
    public void run(){
        _lastFrameTime = System.nanoTime();
        while(true) {
            handleInput();
            update();
            render();
        }
    }

    private void handleInput(){
        //_app.handleInput();
    };

    // Calcula el deltaTime y se lo pasa al update de la aplicacion
    private void update(){
        long currentTime = System.nanoTime();
        long nanoElapsedTime = currentTime - _lastFrameTime;
        _lastFrameTime = currentTime;
        float elapsedTime = (float) (nanoElapsedTime / 1.0E9);

        _app.update(elapsedTime);
    };

    private void render(){
        _graphics.render(_app);
    };
}
