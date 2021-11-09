package es.ucm.gdv.engine;

public class TouchEvent {
    public TouchEvent ( TouchType typeOf, float x, float y, int fingerId, boolean leftMouse){
        _typeOf = typeOf;
        _x = x;
        _y = y;
        _fingerId = fingerId;
        _leftMouse = leftMouse;
    }
    TouchType _typeOf;
    float _x, _y;
    int _fingerId;
    boolean _leftMouse;

    public TouchType getType(){ return _typeOf; }
    public float getX(){ return _x; }
    public float getY(){ return _y; }
    public int getFinger(){ return _fingerId; }
    public boolean isRightMouse(){ return _leftMouse; }
}
