package es.ucm.gdv.engine;

public class TouchEvent {
    public TouchEvent ( TouchType typeOf, float x, float y, int senderId){
        _typeOf = typeOf;
        _x = x;
        _y = y;
        _senderId = senderId;
    }
    TouchType _typeOf;
    float _x, _y;
    int _senderId;

    public TouchType getType(){ return _typeOf; }
    public float getX(){ return _x; }
    public float getY(){ return _y; }
    public int getId(){ return _senderId; }
}
