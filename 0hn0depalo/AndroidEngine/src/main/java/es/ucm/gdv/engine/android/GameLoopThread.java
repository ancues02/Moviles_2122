package es.ucm.gdv.engine.android;

public class GameLoopThread implements Runnable{

    Thread _glThread;
    volatile boolean _running;
    AndroidGraphics _graphics;

    public void resume(){
        //if(!runing) (...) _gameLoopTh = new Thread( this )
    }
    public void pause(){
        //(...)_gameLoopTh.join();
    }
    @Override
    public void run() {

    }
}
