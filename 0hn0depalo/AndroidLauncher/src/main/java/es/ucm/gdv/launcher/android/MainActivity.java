package es.ucm.gdv.launcher.android;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;

import es.ucm.gdv.engine.Scene;
import es.ucm.gdv.engine.android.AndroidEngine;
import es.ucm.gdv.ohno.OhnO;
import es.ucm.gdv.ohno.OhnO_Menu;
import es.ucm.gdv.ohno.OhnO_SelectSize;

public class MainActivity extends AppCompatActivity {

    /**
     * Método llamado por Android como parte del ciclo de vida de
     * la actividad. Se llama en el momento de lanzarla.
     *
     * @param savedInstanceState Información de estado de la actividad
     *                           previamente serializada por ella misma
     *                           para reconstruirse en el mismo estado
     *                           tras un reinicio. Será null la primera
     *                           vez.
     */
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.activity_main);
        _engine = new AndroidEngine(this, 600, 900);
        Scene a = new OhnO_Menu();
        _engine.setScene(a);
        /*int w = _engine.getContentView().getWidth(),
            h = _engine.getContentView().getHeight();*/
    }

    //--------------------------------------------------------------------

    @Override
    protected void onResume() {
        super.onResume();
        _engine.resume();
    }

    //--------------------------------------------------------------------

    @Override
    protected void onPause() {
        super.onPause();
        _engine.pause();
    }

    AndroidEngine _engine;
}


