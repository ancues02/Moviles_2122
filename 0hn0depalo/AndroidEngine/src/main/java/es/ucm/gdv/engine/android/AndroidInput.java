package es.ucm.gdv.engine.android;

import android.view.MotionEvent;
import android.view.View;

import java.util.ArrayList;
import java.util.List;

import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;

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
    public boolean onTouch(View v, MotionEvent event) {
        return false;
    }
    /*
    * _view.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {

                return false;
            }
        });
    * */
}
