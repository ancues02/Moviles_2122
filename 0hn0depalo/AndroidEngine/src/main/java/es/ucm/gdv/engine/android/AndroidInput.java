package es.ucm.gdv.engine.android;

import android.text.method.Touch;
import android.view.MotionEvent;
import android.view.View;

import java.util.ArrayList;
import java.util.List;

import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

public class AndroidInput implements Input, View.OnTouchListener {

    private AndroidGraphics _aGraphics;
    private List<TouchEvent> _touchEventList;

    public AndroidInput(AndroidGraphics aGraphics){
        _aGraphics = aGraphics;
        _aGraphics.getSurfaceView().setOnTouchListener(this);
        _touchEventList = new ArrayList();
    }

    @Override
    synchronized public List<TouchEvent> getTouchEvents() {
        return _touchEventList;
    }

    @Override
    public boolean onTouch(View v, MotionEvent mEvent) {
        if(mEvent.getAction() == MotionEvent.ACTION_DOWN) {
            float posX = _aGraphics.realToVirtualX(mEvent.getX());
            float posY = _aGraphics.realToVirtualY(mEvent.getY());
            TouchEvent event = new TouchEvent(TouchType.Press,
                    posX, posY,
                    mEvent.getActionIndex(),
                    true);//en android siempre pulsamos con "click izquierdo"
            synchronized (this) {
                _touchEventList.add(event);
            }
            return true;
        }
        return false;
    }
}
