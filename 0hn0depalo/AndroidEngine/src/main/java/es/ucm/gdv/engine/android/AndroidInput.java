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
        _touchEventList = new ArrayList<TouchEvent>();
    }

    @Override
    synchronized public List<TouchEvent> getTouchEvents() {
        return _touchEventList;
    }

    @Override
    public boolean onTouch(View v, MotionEvent mEvent) {
        //TouchEvent event = new TouchEvent();
        float posX = _aGraphics.realToVirtualX(mEvent.getX());
        float posY = _aGraphics.realToVirtualY(mEvent.getY());
        if(mEvent.getAction() == MotionEvent.ACTION_DOWN) {
            TouchEvent event = new TouchEvent(TouchType.Press,
                    posX, posY,
                    mEvent.getActionIndex(),
                    true);
            synchronized (this) {
                System.out.println("hey");
                _touchEventList.add(event);
            }
            return true;
        }
        return false;
    }
}
