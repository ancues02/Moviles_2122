package es.ucm.gdv.engine;

enum TouchType { Press, Release, Drag }

public class TouchEvent {
    TouchType typeOf;
    int x, y;
    int senderId;
}
