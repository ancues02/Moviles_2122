package es.ucm.gdv.engine.desktop;


import es.ucm.gdv.engine.Application;
import es.ucm.gdv.engine.Engine;
import es.ucm.gdv.engine.Graphics;
import es.ucm.gdv.engine.Input;

public class DesktopEngine implements Engine {
    private DesktopGraphics _graphics;
    private DesktopInput _input;
    private Application _app;
    private long startTime;

    public DesktopEngine(){
        _graphics = new DesktopGraphics(1920, 1080, 600, 900);
        _input = new DesktopInput();
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

    public void render(){
        //_graphics.render(); -- le puede llegar el app como parametro
    };

    public void update(){
        lastFrameTime = System.nanoTime();

        //long informePrevio = lastFrameTime; // Informes de FPS
        //int frames = 0;
        // Bucle principal
        //while(true) {

        long currentTime = System.nanoTime();
        long nanoElapsedTime = currentTime - lastFrameTime;
        lastFrameTime = currentTime;
        double elapsedTime = (double) nanoElapsedTime / 1.0E9;

            //ventana.update(elapsedTime);
            // Informe de FPS
           /* if (currentTime - informePrevio > 1000000000l) {
                long fps = frames * 1000000000l / (currentTime - informePrevio);
                System.out.println("" + fps + " fps");
                frames = 0;
                informePrevio = currentTime;
            }
            ++frames;
            */
        //}
    };

    public void handleInput(){

    };

}
