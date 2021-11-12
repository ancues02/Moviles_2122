package es.ucm.gdv.engine.android;

import android.view.MotionEvent;
import android.view.View;

import java.util.ArrayList;
import java.util.List;

import es.ucm.gdv.engine.Input;
import es.ucm.gdv.engine.TouchEvent;
import es.ucm.gdv.engine.TouchType;

/**
 * Se encarga de recoger, analizar y devolver los eventos que ocurren en Android
 */
public class AndroidInput implements Input, View.OnTouchListener {

    private AndroidGraphics _aGraphics;
    private List<TouchEvent> _touchEventList;

    public AndroidInput(AndroidGraphics aGraphics){
        _aGraphics = aGraphics;
        _aGraphics.getSurfaceView().setOnTouchListener(this);
        _touchEventList = new ArrayList();
    }

    /**
     * Tiene que ser sincronizado porque la hebra de UI lo añade
     * pero la  hebra de logica los recoge.
     * @return Devuelve la lista de eventos
     */
    @Override
    synchronized public List<TouchEvent> getTouchEvents() {
        return _touchEventList;
    }

    /**
     * Analizamos el evento que llega o lo parseamos a lo que nos interesa.
     * Solo nos quedamos con eventos de pulsar, dejar de pulsar y mover.
     * Lo hemos implementado para que detecte varias pulsaciones a la vez
     * Para ello cogemos el pointerId relativo a cada dedo y analizamos que tipo
     * de evento es. Tambien parseamos la posicion donde se ha pulsado a nuestro canvas.
     * @param v
     * @param mEvent el evento a analizar
     * @return
     */
    @Override
    public boolean onTouch(View v, MotionEvent mEvent) {

        int action = mEvent.getAction() & MotionEvent.ACTION_MASK;

        int pointerIndex = (mEvent.getAction() & MotionEvent.ACTION_POINTER_ID_MASK) >> MotionEvent.ACTION_POINTER_INDEX_SHIFT;

        int pointerId = mEvent.getPointerId(pointerIndex);
        float posX = _aGraphics.realToVirtualX(mEvent.getX(pointerIndex)),
                posY = _aGraphics.realToVirtualY(mEvent.getY(pointerIndex));

        switch (action){
            case MotionEvent.ACTION_DOWN:
            case MotionEvent.ACTION_POINTER_DOWN:
                addEvent(posX, posY, pointerId, TouchType.Press);
                break;
            case MotionEvent.ACTION_UP:
            case MotionEvent.ACTION_POINTER_UP:
                addEvent(posX, posY, pointerId, TouchType.Release);
                break;
            //no funciona bien la lectura cuando se mueven los dedos
            case MotionEvent.ACTION_MOVE:
                /*
                //asi lo detecta siempre y con todos los dedos
                int pointerCount = mEvent.getPointerCount();
                for(int i = 0; i< pointerCount; ++i){
                    pointerIndex = i;
                    pointerId=mEvent.getPointerId(pointerIndex);
                    posX = _aGraphics.realToVirtualX(mEvent.getX(pointerIndex));
                    posY = _aGraphics.realToVirtualY(mEvent.getY(pointerIndex));
                    addEvent(posX, posY, pointerId, TouchType.Drag);

                }*/
                //asi lo detecta algunas veces y con el id 0 practicamente siempre
                addEvent(posX, posY, pointerId, TouchType.Drag);
                break;
        }
        return true;

    }

    /**
     * Creamos un evento nuestro en funcion a los parametros que le llega
     * y lo agregamos a la lista de eventos. Tiene que ser sincronizado
     * porque la hebra de UI lo añade pero la  hebra de logica los recoge.
     * @param posX posicion en x donde se ha pulsado, ya virtual
     * @param posY posicion en y donde se ha pulsado, ya virtual
     * @param pointerId el id del dedo que lo ha pulsado
     * @param type el tipo de pulsacion
     */
    private void addEvent(float posX, float posY, int pointerId,TouchType type ){
        TouchEvent event = new TouchEvent(type,
                posX, posY,
                pointerId,
                true);//en android siempre pulsamos con "click izquierdo"
        synchronized (this) {
            _touchEventList.add(event);
        }
    }
}
